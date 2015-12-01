using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.ViewModels.Scenario;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Services.Responses.Scenario;

namespace DSLNG.PEAR.Web.Controllers
{
    public class ScenarioController : BaseController
    {
        private readonly IScenarioService _scenarioService;
        public ScenarioController(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new ScenarioViewModel();
            viewModel.IsActive = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(ScenarioViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveScenarioRequest>();
            var response = _scenarioService.SaveScenario(request);
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
            var viewModel = _scenarioService.GetScenario(new GetScenarioRequest { Id = id }).MapTo<ScenarioViewModel>();
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Edit(ScenarioViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveScenarioRequest>();
            var response = _scenarioService.SaveScenario(request);
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
            var response = _scenarioService.DeleteScenario(new DeleteScenarioRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;

            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var scenario = _scenarioService.GetScenariosForGrid(new GetScenariosRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    SortingDictionary = gridParams.SortingDictionary,
                    Search = gridParams.Search
                });
            IList<GetScenariosResponse.Scenario> scenarioResponse = scenario.Scenarios;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = scenario.TotalRecords,
                iTotalRecords = scenario.Scenarios.Count,
                aaData = scenarioResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string term)
        {
            var results = _scenarioService.GetScenarios(new GetScenariosRequest { Take = 20, Term = term });
            return Json(new { results = results.Scenarios.Select(x => new { id = x.Id, x.Name }) }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Detail(int id)
        {
            var viewModel = new ScenarioDetailViewModel();
            viewModel.ScenarioId = id;
            return View(viewModel);
        }
    }
}