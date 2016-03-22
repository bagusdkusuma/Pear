using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.EnvironmentScanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.ViewModels.EnvironmentScanning;

namespace DSLNG.PEAR.Web.Controllers
{
    public class EnvironmentScanningController : BaseController
    {
        private readonly IEnvironmentScanningService _environmentScanningService;
        public EnvironmentScanningController(IEnvironmentScanningService environmentScanningService)
        {
            _environmentScanningService = environmentScanningService;
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EnvironmentScanningViewModel.CreateViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveEnvironmentScanningRequest>();
            var response = _environmentScanningService.SaveEnvironmentScanning(request);
            var data = new
            {
                id = response.Id,
                description = response.Description,
                type = viewModel.Type
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(EnvironmentScanningViewModel.DeleteViewModel viewModel)
        {
            var response = _environmentScanningService.DeleteEnvironmentScanning(new DeleteEnvironmentScanningRequest { Id = viewModel.Id });
            return Json(new { success = response.IsSuccess });
        }

        [HttpPost]
        public ActionResult DeleteEnvironmental(EnvironmentScanningViewModel.DeleteViewModel viewModel)
        {
            var response = _environmentScanningService.DeleteEnvironmentalScanning(new DeleteEnvironmentScanningRequest { Id = viewModel.Id });
            return Json(new { success = response.IsSuccess });
        }


        [HttpPost]
        public ActionResult CreateEnvironmental(EnvironmentScanningViewModel.CreateEnvironmentalViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveEnvironmentalScanningRequest>();
            var response = _environmentScanningService.SaveEnvironmentalScanning(request);
            var data = new
            {
                id = response.Id,
                description = response.Description,
                type = viewModel.Type
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

       
        [HttpPost]
        public ActionResult DeleteConstraint(int id)
        {
            var response = _environmentScanningService.DeleteConstraint(new DeleteConstraintRequest { Id = id });
            return Json(new { success = response.IsSuccess });
        }



        [HttpPost]
        public ActionResult DeleteChallenge(int id)
        {
            var response = _environmentScanningService.DeleteChallenge(new DeleteChallengeRequest { Id = id });
            return Json(new { success = response.IsSuccess });
        }

        [HttpPost]
        public ActionResult CreateConstraint(EnvironmentScanningViewModel.Constraint viewModel)
        {
            var request = viewModel.MapTo<SaveConstraintRequest>();
            var response = _environmentScanningService.SaveConstraint(request);
            var data = new
            {
                id = response.Id,
                type = response.Type,
                category = response.Category,
                categoryId = response.CategoryId,
                definition = response.Definition,
                relationids = response.RelationIds,
                threatIds = response.ThreatIds,
                opportunityIds = response.OpportunityIds,
                weaknessIds = response.WeaknessIds,
                strengthIds = response.StrengthIds
            };

            return Json(data, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult CreateChallenge(EnvironmentScanningViewModel.Challenge viewModel)
        {
            var request = viewModel.MapTo<SaveChallengeRequest>();
            var response = _environmentScanningService.SaveChallenge(request);
            var data = new
            {
                id = response.Id,
                type = response.Type,
                categoryId = response.CategoryId,
                category = response.Category,
                definition = response.Definition,
                relationids = response.RelationIds,
                threatIds = response.ThreatIds,
                opportunityIds = response.OpportunityIds,
                weaknessIds = response.WeaknessIds,
                strengthIds = response.StrengthIds
            };

            return Json(data, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public PartialViewResult ShowConstraint(int id)
        {
            var viewModel = _environmentScanningService.GetConstraint(new GetConstraintRequest { Id = id }).MapTo<GetConstraintViewModel>();

            return PartialView("_showConstraint", viewModel);
        }


       [HttpPost]
        public PartialViewResult ShowChallenge(int id)
        {
            var viewModel = _environmentScanningService.GetChallenge(new GetChallengeRequest { Id = id }).MapTo<GetChallengeViewModel>();
            return PartialView("_showChallenge", viewModel);
        }
	}
}