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
        private IScenarioService _scenarioService;
        public ScenarioController(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("scenarioIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return BindingCore(viewModel);
        }


        PartialViewResult BindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                GetDataRowCount,
                GetData
                );
            return PartialView("_IndexGridPartial", gridViewModel);
        }


        private static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Desc");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;

        }


        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridScenarioIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }



        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = _scenarioService.GetScenarios(new GetScenariosRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _scenarioService.GetScenarios(new GetScenariosRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Scenarios;
        }


        public ActionResult Create()
        {
            var viewModel = new ScenarioViewModel();

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
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var scenario = _scenarioService.GetScenarios(new GetScenariosRequest
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
	}
}