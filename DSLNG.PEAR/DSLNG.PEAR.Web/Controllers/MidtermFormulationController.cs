using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Web.ViewModels.MidtermFormulation;
using System;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.MidtermFormulation;

namespace DSLNG.PEAR.Web.Controllers
{
    public class MidtermFormulationController : BaseController
    {
        private readonly IMidtermFormulationService _midtermFormulationService;
        public MidtermFormulationController(IMidtermFormulationService midtermFormulationService) {
            _midtermFormulationService = midtermFormulationService;
        }
        [HttpPost]
        public ActionResult AddStage(MidtermPhaseStageViewModel viewModel) {
            var resp = _midtermFormulationService.AddStage(viewModel.MapTo<AddStageRequest>());
            return Json(resp);
        }

        [HttpPost]
        public ActionResult AddDefinition(MidtermStageDefinitionViewModel viewModel) {
            var resp = _midtermFormulationService.AddDefinition(viewModel.MapTo<AddDefinitionRequest>());
            return Json(resp);
        }
	}
}