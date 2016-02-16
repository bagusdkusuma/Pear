using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PlanningBlueprint;
using DSLNG.PEAR.Services.Responses.PlanningBlueprint;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.PlanningBlueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.BusinessPosture;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PlanningBlueprintController : BaseController
    {
        private readonly IPlanningBlueprintService _planningBlueprintService;
        private readonly IBusinessPostureIdentificationService _businessPostureIdentification;
        public PlanningBlueprintController(IPlanningBlueprintService planningBlueprintService,
            IBusinessPostureIdentificationService businessPostureIdentification) {
            _planningBlueprintService = planningBlueprintService;
            _businessPostureIdentification = businessPostureIdentification;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var planningBlueprintData = _planningBlueprintService.GetPlanningBlueprints(new GetPlanningBlueprintsRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                Search = gridParams.Search,
                SortingDictionary = gridParams.SortingDictionary
            });
            IList<GetPlanningBlueprintsResponse.PlanningBlueprint> DatasResponse = planningBlueprintData.PlanningBlueprints;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = planningBlueprintData.TotalRecords,
                iTotalRecords = planningBlueprintData.PlanningBlueprints.Count,
                aaData = DatasResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Create()
        {
            return View(new PlanningBlueprintViewModel());
        }

        [HttpPost]
        public ActionResult Create(PlanningBlueprintViewModel viewModel)
        {
            var request = viewModel.MapTo<SavePlanningBlueprintRequest>();
            var response = _planningBlueprintService.SavePlanningBlueprint(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public ActionResult EnvironmentsScanning(int id) {
            return View(new EnvironmentsScanningViewModel());
        }

        public ActionResult BusinessPostureIdentification(int id) {
           
            return View(_businessPostureIdentification.Get(new GetBusinessPostureRequest{Id = id}).MapTo<BusinessPostureViewModel>());
        }
	}
}