using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PmsSummary;
using DSLNG.PEAR.Web.ViewModels.PmsConfig;
using DSLNG.PEAR.Web.ViewModels.PmsConfigDetails;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PmsConfigController : BaseController
    {
        private readonly IPmsSummaryService _pmsSummaryService;
        private readonly IDropdownService _dropdownService;

        public PmsConfigController(IPmsSummaryService pmsSummaryService, IDropdownService dropdownService)
        {
            _pmsSummaryService = pmsSummaryService;
            _dropdownService = dropdownService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Configuration", "PmsSummary");
        }

        public ActionResult Create(int id)
        {
            var viewModel = new CreatePmsConfigViewModel();
            viewModel.PmsSummaryId = id;
            viewModel.Pillars = _dropdownService.GetPillars(id).MapTo<SelectListItem>();
            viewModel.ScoringTypes = _dropdownService.GetScoringTypes().MapTo<SelectListItem>();
            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreatePmsConfigViewModel viewModel)
        {
            var request = viewModel.MapTo<CreatePmsConfigRequest>();
            var response = _pmsSummaryService.CreatePmsConfig(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Details", "PmsSummary", new { id = viewModel.PmsSummaryId });
        }

        public ActionResult Update(int id)
        {
            var response = _pmsSummaryService.GetPmsConfig(id);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<UpdatePmsConfigViewModel>();
                if (!response.ScoreIndicators.Any())
                {
                    viewModel.ScoreIndicators.Add(new ViewModels.Common.PmsSummary.ScoreIndicatorViewModel
                    {
                        Id = 0,
                        Color = null,
                        Expression = null
                    });
                }
                viewModel.ScoringTypes = _dropdownService.GetScoringTypes().MapTo<SelectListItem>();
                return PartialView("_Update", viewModel);    
            }

            return base.ErrorPage(response.Message);
        }

        [HttpPost]
        public ActionResult Update(UpdatePmsConfigViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdatePmsConfigRequest>();
            var response = _pmsSummaryService.UpdatePmsConfig(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Details", "PmsSummary", new { id = response.PmsSummaryId });
        }

        [HttpPost]
        public ActionResult Delete(int id, int pmsSummaryId)
        {
            var response = _pmsSummaryService.DeletePmsConfig(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Details", "PmsSummary", new { id = pmsSummaryId });
        }

        public JsonResult ScoreIndicator(int id)
        {
            var response = _pmsSummaryService.GetScoreIndicators(new GetScoreIndicatorRequest { PmsConfigId = id });
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<ScoreIndicatorDetailsViewModel>();
                return Json(new { isSuccess = true, data = viewModel.ScoreIndicators }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
	}
}