using DSLNG.PEAR.Common.Contants;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Buyer;
using DSLNG.PEAR.Services.Requests.Vessel;
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Web.ViewModels;
using DSLNG.PEAR.Web.ViewModels.VesselSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.Select;
using System.Data.SqlClient;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class VesselScheduleController : BaseController
    {
        private readonly IVesselScheduleService _vesselScheduleService;
        private readonly IVesselService _vesselService;
        private readonly IBuyerService _buyerService;
        private readonly ISelectService _selectService;
        public VesselScheduleController(IVesselScheduleService vesselScheduleService,
            IVesselService vesselService,
            IBuyerService buyerService,
            ISelectService selectService)
        {
            _vesselScheduleService = vesselScheduleService;
            _vesselService = vesselService;
            _buyerService = buyerService;
            _selectService = selectService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridVesselScheduleIndex");
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
            viewModel.Columns.Add("Buyer");
            viewModel.Columns.Add("Location");
            viewModel.Columns.Add("SalesType");
            viewModel.Columns.Add("Type");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridVesselScheduleIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).VesselSchedules;
        }

        public ActionResult VesselList(string term)
        {
            var vessels = _vesselService.GetVessels(new GetVesselsRequest {
                Skip = 0,
                Take = 20, 
                Term = term,
            }).Vessels;
            return Json(new { results = vessels }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuyerList(string term)
        {
            var buyers = _buyerService.GetBuyers(new GetBuyersRequest {
                Skip = 0,
                Take = 20, 
                Term = term,
            }).Buyers;
            return Json(new { results = buyers }, JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /VesselSchedule/Create
        public ActionResult Create()
        {
            var viewModel = new VesselScheduleViewModel();
            viewModel.SalesTypes = _selectService.GetSelect(new GetSelectRequest { Name = "vessel-schedule-sales-types" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            viewModel.IsActive = true;
            return View(viewModel);
        }

        //
        // POST: /VesselSchedule/Create
        [HttpPost]
        public ActionResult Create(VesselScheduleViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveVesselScheduleRequest>();
            _vesselScheduleService.SaveVesselSchedule(req);
            return RedirectToAction("Index");
        }

        //
        // GET: /VesselSchedule/Edit/5
        public ActionResult Edit(int id)
        {
            var vesselSchedule = _vesselScheduleService.GetVesselSchedule(new GetVesselScheduleRequest { Id = id });
            var viewModel = vesselSchedule.MapTo<VesselScheduleViewModel>();
            viewModel.SalesTypes = _selectService.GetSelect(new GetSelectRequest { Name = "vessel-schedule-sales-types" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            return View(viewModel);
        }

        //
        // POST: /VesselSchedule/Edit/5
        [HttpPost]
        public ActionResult Edit(VesselScheduleViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveVesselScheduleRequest>();
            _vesselScheduleService.SaveVesselSchedule(req);
            return RedirectToAction("Index");
        }

        // GET: /VesselSchedule/Edit/5
        public ActionResult Manage(int id)
        {
            var vesselSchedule = _vesselScheduleService.GetVesselSchedule(new GetVesselScheduleRequest { Id = id });
            var viewModel = vesselSchedule.MapTo<VesselScheduleViewModel>();
            viewModel.SalesTypes = _selectService.GetSelect(new GetSelectRequest { Name = "vessel-schedule-sales-types" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            return View(viewModel);
        }

        //
        // POST: /VesselSchedule/Edit/5
        [HttpPost]
        public ActionResult Manage(VesselScheduleViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveVesselScheduleRequest>();
            if (viewModel.AsNew) {
                req.Id = 0;
            }
            _vesselScheduleService.SaveVesselSchedule(req);
            return RedirectToAction("Display", "Highlight");
        }
        //
        // POST: /VesselSchedule/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _vesselScheduleService.Delete(new DeleteVesselScheduleRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }


        public ActionResult Grid(GridParams gridParams)
        {
            var vessel = _vesselScheduleService.GetVesselSchedulesForGrid(new GetVesselSchedulesRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = vessel.TotalRecords,
                iTotalRecords = vessel.VesselSchedules.Count,
                aaData = vessel.VesselSchedules.Select(x => new
                    {
                        x.Vessel,
                        ETA = x.ETA.HasValue ? x.ETA.Value.ToString(DateFormat.DateForGrid) : string.Empty,
                        ETD = x.ETD.HasValue ? x.ETD.Value.ToString(DateFormat.DateForGrid) : string.Empty,
                        x.Buyer,
                        x.Location,
                        x.SalesType,
                        x.Type,
                        x.Cargo, 
                        x.IsActive,
                        x.id
                    })
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
