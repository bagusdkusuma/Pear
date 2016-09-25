﻿using DSLNG.PEAR.Services.Interfaces;
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

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerLoadingScheduleController : BaseController
    {

        private readonly IVesselScheduleService _vesselScheduleService;
        private readonly IVesselService _vesselService;
        private readonly IBuyerService _buyerService;
        private readonly ISelectService _selectService;

        public DerLoadingScheduleController(IVesselScheduleService vesselScheduleService, IVesselService vesselService, IBuyerService buyerService, ISelectService selectService) {
            _vesselScheduleService = vesselScheduleService;
            _vesselService = vesselService;
            _buyerService = buyerService;
            _selectService = selectService;
        }
        // GET: DerLoadingSchedule
        public ActionResult Choose()
        {
            var vesselSchedules = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest
            {
                allActiveList = true,
                Skip = 0,
                Take = 20,
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
            }).Buyers.Select(x => new SelectListItem { Text = x.Name, Value = x.id.ToString() }).ToList();
            viewModel.Vessels = _vesselService.GetVessels(new GetVesselsRequest
            {
                Skip = 0,
                Take = 100
            }).Vessels.Select(x => new SelectListItem { Text = x.Name, Value = x.id.ToString() }).ToList();
            viewModel.IsActive = true;
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Create(VesselScheduleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var req = viewModel.MapTo<SaveVesselScheduleRequest>();
                var resp = _vesselScheduleService.SaveVesselSchedule(req);
                return Json(new { isSuccess = true, data= resp });
            }
            else {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();
                return Json(new { isSuccess = false, message = errorList });
            }
        }
    }
}