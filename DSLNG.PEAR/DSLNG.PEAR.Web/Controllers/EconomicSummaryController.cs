using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.EconomicSummary;
using DSLNG.PEAR.Web.ViewModels.EconomicSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class EconomicSummaryController : BaseController
    {
        private readonly IEconomicSummaryService _economicSummaryService;
        public EconomicSummaryController(IEconomicSummaryService economicSummaryService)
        {
            _economicSummaryService = economicSummaryService;
        }
        
        public ActionResult Index()
        {
            var response = _economicSummaryService.GetEconomicSummaryReport();
            var viewModel = response.MapTo<EconomicSummaryReportViewModel>();
            return View(viewModel);
        }

        public ActionResult Config()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new EconomicSummaryCreateViewModel();
            viewModel.Scenarios.Insert(0, new EconomicSummaryCreateViewModel.Scenario());
            viewModel.IsActive = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(EconomicSummaryCreateViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveEconomicSummaryRequest>();
            var response = _economicSummaryService.SaveEconomicSummary(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Create", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var viewModel = _economicSummaryService.GetEconomicSummary(new GetEconomicSummaryRequest { Id = id }).MapTo<EconomicSummaryCreateViewModel>();
            viewModel.Scenarios.Insert(0, new EconomicSummaryCreateViewModel.Scenario());
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EconomicSummaryCreateViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveEconomicSummaryRequest>();
            var response = _economicSummaryService.SaveEconomicSummary(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Edit", viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _economicSummaryService.DeleteEconomicSummary(new DeleteEconomicSummaryRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Config");
            }
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var economic = _economicSummaryService.GetEconomicSummariesForGrid(new GetEconomicSummariesRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = economic.TotalRecords,
                iTotalRecords = economic.EconomicSummaries.Count,
                aaData = economic.EconomicSummaries
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateResult()
        {
            _economicSummaryService.UpdateEconomicSummary();
            return RedirectToAction("Index");
        }
        /*public ActionResult EconomicSummaries(string term)
        {
            var results = _economicSummaryService.GetEconomicSummaries(new GetEconomicSummariesRequest() { Take = 20, Term = term });
            return Json(new { results = results.EconomicSummaries }, JsonRequestBehavior.AllowGet);
        }*/
	}
}