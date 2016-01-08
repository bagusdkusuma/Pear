using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PmsSummary;
using DSLNG.PEAR.Web.ViewModels.PmsConfigDetails;
using DSLNG.PEAR.Web.ViewModels.PmsSummary;
using DSLNG.PEAR.Web.ViewModels.Common.PmsSummary;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PmsConfigDetailsController : BaseController
    {
        private readonly IPmsSummaryService _pmsSummaryService;
        private readonly IDropdownService _dropdownService;

        public PmsConfigDetailsController(IPmsSummaryService pmsSummaryService, IDropdownService dropdownService)
        {
            _pmsSummaryService = pmsSummaryService;
            _dropdownService = dropdownService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(int id)
        {
            int pmsConfigId = id;
            var viewModel = new CreatePmsConfigDetailsViewModel();
            viewModel.PmsConfigId = pmsConfigId;
            viewModel.Kpis = _dropdownService.GetKpisForPmsConfigDetails(pmsConfigId).MapTo<SelectListItem>();
            viewModel.ScoringTypes = _dropdownService.GetScoringTypes().MapTo<SelectListItem>();
            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreatePmsConfigDetailsViewModel viewModel)
        {
            var request = viewModel.MapTo<CreatePmsConfigDetailsRequest>();
            var response = _pmsSummaryService.CreatePmsConfigDetails(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Details", "PmsSummary", new { id = response.PmsSummaryId });
        }

        public ActionResult Update(int id)
        {
            var response = _pmsSummaryService.GetPmsConfigDetails(id);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<UpdatePmsConfigDetailsViewModel>();
                if (!response.ScoreIndicators.Any())
                {
                    var scoreIndicator = new List<ScoreIndicatorViewModel>();
                    scoreIndicator.Add(new ScoreIndicatorViewModel { Id = 0 });
                    viewModel.ScoreIndicators = scoreIndicator;
                }
                viewModel.ScoringTypes = _dropdownService.GetScoringTypes().MapTo<SelectListItem>();
                //viewModel.Kpis = _dropdownService.GetKpis().MapTo<SelectListItem>();
                viewModel.Kpis = _dropdownService.GetKpisForPmsConfigDetailsUpdate(viewModel.PmsConfigId, response.KpiId).MapTo<SelectListItem>();
                viewModel.KpiId = response.KpiId;
                viewModel.Target = response.Target;
                viewModel.Targets = response.ScoreIndicators.Select(x => new SelectListItem{Text = x.Expression, Value = x.Expression}).ToList();
                return PartialView("_Update", viewModel);
            }

            return base.ErrorPage(response.Message);
        }

        [HttpPost]
        public ActionResult Update(UpdatePmsConfigDetailsViewModel viewModel)
        {
            
            var request = viewModel.MapTo<UpdatePmsConfigDetailsRequest>();
            var response = _pmsSummaryService.UpdatePmsConfigDetails(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Details", "PmsSummary", new { id = response.PmsSummaryId });
        }

        public ActionResult ScoreIndicatorDetails(int id)
        {
            int pmsConfigDetailsId = id;
            var response = _pmsSummaryService.GetScoreIndicators(new GetScoreIndicatorRequest { PmsConfigDetailId = pmsConfigDetailsId });
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<ScoreIndicatorDetailsViewModel>();
                return PartialView("_ScoreIndicatorDetails", viewModel);
            }

            return base.ErrorPage(response.Message);
        }

        public JsonResult ScoreIndicator(int id)
        {
            var response = _pmsSummaryService.GetScoreIndicators(new GetScoreIndicatorRequest { PmsConfigDetailId = id });
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<ScoreIndicatorDetailsViewModel>();
                return Json(new {isSuccess = true, data = viewModel.ScoreIndicators}, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int id, int pmsSummaryId)
        {
            var response = _pmsSummaryService.DeletePmsConfigDetails(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Details", "PmsSummary", new { id = pmsSummaryId });
        }
    }
}