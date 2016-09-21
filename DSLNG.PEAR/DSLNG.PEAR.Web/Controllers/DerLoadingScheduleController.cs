using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Web.ViewModels.DerLoadingSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerLoadingScheduleController : BaseController
    {

        private readonly IVesselScheduleService _vesselScheduleService;
        public DerLoadingScheduleController(IVesselScheduleService vesselScheduleService) {
            _vesselScheduleService = vesselScheduleService;
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
    }
}