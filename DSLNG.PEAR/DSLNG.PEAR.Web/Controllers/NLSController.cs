using DSLNG.PEAR.Common.Contants;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.NLS;
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Web.ViewModels.NLS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using System.Data.SqlClient;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Services.Requests.HighlightOrder;

namespace DSLNG.PEAR.Web.Controllers
{
    public class NLSController : BaseController
    {
        private readonly INLSService _nlsService;
        private readonly IVesselScheduleService _vesselScheduleService;
        private readonly IHighlightOrderService _highlightOrderService;
        public NLSController(INLSService nlsService, IVesselScheduleService vesselScheduleService, IHighlightOrderService highlightOrderService)
        {
            _nlsService = nlsService;
            _vesselScheduleService = vesselScheduleService;
            _highlightOrderService = highlightOrderService;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridNLSIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return BindingCore(viewModel);
        }

        PartialViewResult BindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                GetDataRowCount,
                GetData
            );
            return PartialView("_IndexGridPartial", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("Vessel");
            viewModel.Columns.Add("ETA");
            viewModel.Columns.Add("ETD");
            viewModel.Columns.Add("Remark");
            viewModel.Columns.Add("CreatedAt");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridNLSIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _nlsService.GetNLSList(new GetNLSListRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _nlsService.GetNLSList(new GetNLSListRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).NLSList;
        }

        public ActionResult VesselScheduleList(string term)
        {
            var vesselSchedules = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest
            {
                Skip = 0,
                Take = 20,
                Term = term,
            }).VesselSchedules;
            return Json(new { results = vesselSchedules }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /NLS/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /NLS/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /NLS/Create
        [HttpPost]
        public ActionResult Create(NLSViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveNLSRequest>();
            var response = _nlsService.SaveNLS(req);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Create", viewModel);
        }

        //
        // GET: /NLS/Edit/5
        public ActionResult Edit(int id)
        {
            var nls = _nlsService.GetNLS(new GetNLSRequest { Id = id });
            var viewModel = nls.MapTo<NLSViewModel>();
            return View(viewModel);
        }

        //
        // POST: /NLS/Edit/5
        [HttpPost]
        public ActionResult Edit(NLSViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveNLSRequest>();
            var response = _nlsService.SaveNLS(req);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Edit", viewModel);
        }

        //
        // GET: /NLS/Edit/5
        public ActionResult Manage()
        {
            var viewModel = new NLSViewModel();
            var id = string.IsNullOrEmpty(Request.QueryString["nlsId"]) ? 0 : int.Parse(Request.QueryString["nlsId"]);
            if (id != 0)
            {
                var nls = _nlsService.GetNLS(new GetNLSRequest { Id = id });
                viewModel = nls.MapTo<NLSViewModel>();
            }
            else {
                var vesselScheduleId = int.Parse(Request.QueryString["vsId"]);
                viewModel.VesselScheduleId = vesselScheduleId;
                viewModel.VesselName = _vesselScheduleService.GetVesselSchedule(new GetVesselScheduleRequest { Id = vesselScheduleId }).VesselName;
            }
            return View(viewModel);
        }

        //
        // POST: /NLS/Edit/5
        [HttpPost]
        public ActionResult Manage(NLSViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveNLSRequest>();
            var response = _nlsService.SaveNLS(req);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Display", "Highlight");
            }
            return View("Manage", viewModel);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {

            var response = _nlsService.Delete(new DeleteNLSRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();

        }

        public ActionResult InVesselSchedule(int id)
        {
            var nlsList = _nlsService.GetNLSList(new GetNLSListRequest { VesselScheduleId = id });
            var staticHighlightResp = _highlightOrderService.GetStaticHighlights(new GetStaticHighlightOrdersRequest { Take = -1 });
            ViewBag.IsAllowedToManage = staticHighlightResp.HighlightOrders.First(x => x.Name == "Vessel Schedule").RoleGroupIds.Contains(UserProfile().RoleId);
            ViewBag.VesselScheduleId = id;
            return PartialView("_RemarkList", nlsList.NLSList.MapTo<NLSViewModel>());
        }


        public ActionResult Grid(GridParams gridParams)
        {
            var NLS = _nlsService.GetNLSListForGrid(new GetNLSListRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = NLS.TotalRecords,
                iTotalRecords = NLS.NLSList.Count,
                aaData = NLS.NLSList.Select(x => new
                    {
                        x.Vessel,
                        CreatedAt = x.CreatedAt.ToString(DateFormat.DateForGrid),
                        x.Remark,
                        x.Id
                    })
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
