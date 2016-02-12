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
using DSLNG.PEAR.Services.Requests.EnvironmentScanning;
using DSLNG.PEAR.Web.ViewModels.EnvironmentScanning;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PlanningBlueprintController : BaseController
    {
        private readonly IPlanningBlueprintService _planningBlueprintService;
        private readonly IEnvironmentScanningService _environmentScanningService;
        public PlanningBlueprintController(IPlanningBlueprintService planningBlueprintService, IEnvironmentScanningService environmentScanningService)
        {
            _planningBlueprintService = planningBlueprintService;
            _environmentScanningService = environmentScanningService;
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

        public ActionResult EnvironmentsScanning(int id) 
        {
            var viewModel = _environmentScanningService.GetEnvironmentsScanning(new GetEnvironmentsScanningRequest { Id = id }).MapTo<EnvironmentScanningViewModel>();
            var ListType = new List<SelectListItem>();
            var type1 = new SelectListItem() { Text="Internal", Value="Internal"};
            ListType.Add(type1);
            var type2 = new SelectListItem() { Text = "External", Value = "External" };
            ListType.Add(type2);

            var listCategory = new List<SelectListItem>();
            var category1 = new SelectListItem() { Text = "Politic", Value = "Politic" };
            listCategory.Add(category1);
            var category2 = new SelectListItem() { Text = "Economic", Value = "Economic" };
            listCategory.Add(category2);

            viewModel.Types = ListType;
            viewModel.Categories = listCategory;

            return View(viewModel);
        }

	}
}