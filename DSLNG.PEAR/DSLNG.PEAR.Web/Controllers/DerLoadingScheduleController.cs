using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Web.ViewModels.DerLoadingSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.Vessel;
using DSLNG.PEAR.Services.Requests.Buyer;
using DSLNG.PEAR.Web.ViewModels.VesselSchedule;
using DSLNG.PEAR.Services.Requests.Select;
using System.Data.SqlClient;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Web.ViewModels.Vessel;
using DSLNG.PEAR.Web.ViewModels.Buyer;
using DSLNG.PEAR.Services.Requests.NLS;
using DSLNG.PEAR.Services.Requests.HighlightOrder;
using DSLNG.PEAR.Web.ViewModels.NLS;
using System.Globalization;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerLoadingScheduleController : BaseController
    {

        private readonly IVesselScheduleService _vesselScheduleService;
        private readonly IVesselService _vesselService;
        private readonly IBuyerService _buyerService;
        private readonly ISelectService _selectService;
        private readonly IMeasurementService _measurementService;
        private readonly INLSService _nlsService;
        private readonly IHighlightOrderService _highlightOrderService;
        private readonly IDerLoadingScheduleService _derLoadingScheduleService;

        public DerLoadingScheduleController(IVesselScheduleService vesselScheduleService, 
            IVesselService vesselService, 
            IBuyerService buyerService, 
            ISelectService selectService, 
            IMeasurementService measurementService,
            INLSService nlsService,
            IHighlightOrderService highlightOrderService,
            IDerLoadingScheduleService derLoadingScheduleService)
        {
            _vesselScheduleService = vesselScheduleService;
            _vesselService = vesselService;
            _buyerService = buyerService;
            _selectService = selectService;
            _measurementService = measurementService;
            _nlsService = nlsService;
            _highlightOrderService = highlightOrderService;
            _derLoadingScheduleService = derLoadingScheduleService;
        }
        // GET: DerLoadingSchedule
        public ActionResult Choose(string date)
        {
            var remarkDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var vesselSchedules = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest
            {
                allActiveList = true,
                RemarkDate = remarkDate,
                OrderByETDDesc = true
            });
            var viewModel = new LoadingSchedulesViewModel();
            viewModel.Schedules = vesselSchedules.VesselSchedules.MapTo<LoadingSchedulesViewModel.LoadingScheduleViewModel>();
            return PartialView(viewModel);
        }

        public ActionResult VesselList(string term)
        {
            var vessels = _vesselService.GetVessels(new GetVesselsRequest
            {
                Skip = 0,
                Take = 20,
                Term = term,
            }).Vessels;
            return Json(new { results = vessels }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuyerList(string term)
        {
            var buyers = _buyerService.GetBuyers(new GetBuyersRequest
            {
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
            viewModel.Buyers = _buyerService.GetBuyers(new GetBuyersRequest
            {
                Skip = 0,
                Take = 100
            }).Buyers.OrderBy(x=>x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.id.ToString() }).ToList();
            viewModel.Vessels = _vesselService.GetVessels(new GetVesselsRequest
            {
                Skip = 0,
                Take = 100
            }).Vessels.OrderBy(x=>x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.id.ToString() }).ToList();
            viewModel.IsActive = true;
            return PartialView(viewModel);
        }

        public ActionResult GetBuyers()
        {
            var response = _buyerService.GetBuyers(new GetBuyersRequest
            {
                Skip = 0,
                Take = 100
            }).Buyers.Select(x => new SelectListItem { Text = x.Name, Value = x.id.ToString() }).ToList();
            return Json(new { result = response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVessels()
        {
            var response = _vesselService.GetVessels(new GetVesselsRequest
            {
                Skip = 0,
                Take = 100
            }).Vessels.OrderBy(x=>x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.id.ToString() }).ToList();
            return Json(new { result = response }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(VesselScheduleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var req = viewModel.MapTo<SaveVesselScheduleRequest>();
                var resp = _vesselScheduleService.SaveVesselSchedule(req);
                return Json(resp);
            }
            else
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();
                return Json(new { IsSuccess = false, Message = errorList });
            }
        }

        public ActionResult Edit(int id)
        {
            var vesselSchedule = _vesselScheduleService.GetVesselSchedule(new GetVesselScheduleRequest { Id = id });
            var viewModel = vesselSchedule.MapTo<VesselScheduleViewModel>();
            viewModel.SalesTypes = _selectService.GetSelect(new GetSelectRequest { Name = "vessel-schedule-sales-types" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            viewModel.Buyers = _buyerService.GetBuyers(new GetBuyersRequest
            {
                Skip = 0,
                Take = 100
            }).Buyers.Select(x => new SelectListItem { Text = x.Name, Value = x.id.ToString() }).ToList();
            viewModel.Vessels = _vesselService.GetVessels(new GetVesselsRequest
            {
                Skip = 0,
                Take = 100
            }).Vessels.Select(x => new SelectListItem { Text = x.Name, Value = x.id.ToString() }).ToList();
            return PartialView(viewModel);
        }

        //
        // POST: /VesselSchedule/Edit/5
        [HttpPost]
        public ActionResult Edit(VesselScheduleViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var req = viewModel.MapTo<SaveVesselScheduleRequest>();
                var resp = _vesselScheduleService.SaveVesselSchedule(req);
                return Json(resp);
            }
            else
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();
                return Json(new { IsSuccess = false, Message = errorList });
            }
        }


        public ActionResult AddVessel()
        {
            var viewModel = new VesselViewModel();
            viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            }).Measurements
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult AddVessel(VesselViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var req = viewModel.MapTo<SaveVesselRequest>();
                var resp = _vesselService.SaveVessel(req);
                return Json(resp);
            }
            else
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();
                return Json(new { IsSuccess = false, Message = errorList });
            }
        }

        public ActionResult AddBuyer()
        {
            var viewModel = new BuyerViewModel() { IsActive = true };
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult AddBuyer(BuyerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var req = viewModel.MapTo<SaveBuyerRequest>();
                var resp = _buyerService.SaveBuyer(req);
                return Json(resp);
            }
            else
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();
                return Json(new { IsSuccess = false, Message = errorList });
            }
        }

        public ActionResult Remarks(int id) {
            var nlsList = _nlsService.GetNLSList(new GetNLSListRequest { VesselScheduleId = id });
            var staticHighlightResp = _highlightOrderService.GetStaticHighlights(new GetStaticHighlightOrdersRequest { Take = -1 });
            ViewBag.IsAllowedToManage = staticHighlightResp.HighlightOrders.First(x => x.Name == "Vessel Schedule").RoleGroupIds.Contains(UserProfile().RoleId);
            ViewBag.VesselScheduleId = id;
            return PartialView(nlsList.NLSList.MapTo<NLSViewModel>());
        }

        public ActionResult ManageRemark() {

            var viewModel = new NLSViewModel();
            var id = string.IsNullOrEmpty(Request.QueryString["nlsId"]) ? 0 : int.Parse(Request.QueryString["nlsId"]);
            if (id != 0)
            {
                var nls = _nlsService.GetNLS(new GetNLSRequest { Id = id });
                viewModel = nls.MapTo<NLSViewModel>();
            }
            else
            {
                var vesselScheduleId = int.Parse(Request.QueryString["vsId"]);
                viewModel.VesselScheduleId = vesselScheduleId;
                viewModel.VesselName = _vesselScheduleService.GetVesselSchedule(new GetVesselScheduleRequest { Id = vesselScheduleId }).VesselName;
            }
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult ManageRemark(NLSViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveNLSRequest>();
            var response = _nlsService.SaveNLS(req);
            return Json(response);
        }

        [HttpPost]
        public ActionResult SaveSchedules(int[] ids, string date) {
            var dateTime = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            return Json(_derLoadingScheduleService.SaveSchedules(ids, dateTime));
        }

        [HttpPost]
        public ActionResult DeleteRemark(int id)
        {
            var response = _nlsService.Delete(new DeleteNLSRequest { Id = id });
            return Json(response);

        }
    }
}