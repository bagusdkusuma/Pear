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

namespace DSLNG.PEAR.Web.Controllers
{
    public class NLSController : Controller
    {
        private readonly INLSService _nlsService;
        private readonly IVesselScheduleService _vesselScheduleService;
        public NLSController(INLSService nlsService, IVesselScheduleService vesselScheduleService) {
            _nlsService = nlsService;
            _vesselScheduleService = vesselScheduleService;
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

        public ActionResult VesselScheduleList(string term) {
            var vesselSchedules = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest {Skip = 0, Take = 20, Term = term,
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
            _nlsService.SaveNLS(req);
            return RedirectToAction("Index");
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
            _nlsService.SaveNLS(req);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _nlsService.Delete(new DeleteNLSRequest { Id = id });
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult InVesselSchedule(int id) {
            var nlsList = _nlsService.GetNLSList(new GetNLSListRequest { VesselScheduleId = id });
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
                aaData = NLS.NLSList
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
