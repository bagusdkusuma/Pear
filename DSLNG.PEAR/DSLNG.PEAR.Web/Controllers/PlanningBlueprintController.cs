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
using DSLNG.PEAR.Services.Requests.EnvironmentScanning;
using DSLNG.PEAR.Web.ViewModels.EnvironmentScanning;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PlanningBlueprintController : BaseController
    {
        private readonly IPlanningBlueprintService _planningBlueprintService;
        private readonly IEnvironmentScanningService _environmentScanningService;
        private readonly IBusinessPostureIdentificationService _businessPostureIdentification;
        public PlanningBlueprintController(IPlanningBlueprintService planningBlueprintService,
            IBusinessPostureIdentificationService businessPostureIdentification,
            IEnvironmentScanningService environmentScanningService)
        {
            _planningBlueprintService = planningBlueprintService;
            _businessPostureIdentification = businessPostureIdentification;
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
                //if (viewModel.IsLocked) {
                //    return RedirectToAction("Index");
                //}
            var ListType = new List<SelectListItem>();
            var type1 = new SelectListItem() { Text = "Internal", Value = "Internal" };
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

        [HttpPost]
        public ActionResult SubmitEnvironmentsScanning(int id) {
            var resp = _environmentScanningService.SubmitEnvironmentsScanning(id);
            if (resp.IsSuccess) {
                return RedirectToAction("BusinessPostureIdentification", new { id = resp.BusinessPostureId});
            }
            return RedirectToAction("EnvironmentsScanning", new { id = id });
        }

        public ActionResult BusinessPostureIdentification(int id)
        {

            return View(_businessPostureIdentification.Get(new GetBusinessPostureRequest { Id = id }).MapTo<BusinessPostureViewModel>());
        }

        [HttpPost]
        public ActionResult SubmitBusinessPosture(int id)
        {
            var resp = _businessPostureIdentification.SubmitBusinessPosture(id);
            if (resp.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("BusinessPostureIdentification", new { id = id });
        }

        [HttpPost]
        public ActionResult ApproveVoyagePlan(int id) {
            var resp = _planningBlueprintService.ApproveVoyagePlan(id);
            if (resp.IsSuccess) {
                return RedirectToAction("VoyagePlanApproval");
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult VoyagePlan()
        {
            var resp = _planningBlueprintService.GetVoyagePlan();
            if (resp != null)
                return View(resp.MapTo<VoyagePlanViewModel>());
            return View();
        }

        public ActionResult VoyagePlanApproval() {
            return View();
        }
    }
}