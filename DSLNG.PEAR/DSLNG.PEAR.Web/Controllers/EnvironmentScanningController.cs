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
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(EnvironmentScanningViewModel.CreateViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveEnvironmentScanningRequest>();
            var response = _environmentScanningService.SaveEnvironmentScanning(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            var data = new
            {
                id = response.Id,
                description = response.Description,
                type = response.Type
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(EnvironmentScanningViewModel.DeleteViewModel viewModel)
        {
            var response = _environmentScanningService.DeleteEnvironmentScanning(new DeleteEnvironmentScanningRequest { Id = viewModel.Id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return Json(new { success = response.IsSuccess });
        }

        [HttpPost]
        public ActionResult DeleteEnvironmental(EnvironmentScanningViewModel.DeleteViewModel viewModel)
        {
            var response = _environmentScanningService.DeleteEnvironmentalScanning(new DeleteEnvironmentScanningRequest { Id = viewModel.Id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return Json(new { success = response.IsSuccess });
        }
        
	}
}