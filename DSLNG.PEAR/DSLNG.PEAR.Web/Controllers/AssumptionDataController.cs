using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.AssumptionConfig;
using DSLNG.PEAR.Services.Requests.AssumptionData;
using DSLNG.PEAR.Services.Requests.Scenario;
using DSLNG.PEAR.Web.ViewModels.AssumptionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Services.Responses.AssumptionData;
using DSLNG.PEAR.Services.Requests.AssumptionCategory;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Web.Controllers
{
    public class AssumptionDataController : BaseController
    {
        private IAssumptionDataService _assumptionDataService;
        private IScenarioService _scenarioService;
        private IAssumptionCategoryService _assumptionCategoryService;
        public AssumptionDataController(IAssumptionDataService assumptionDataService, IScenarioService scenarioService, IAssumptionCategoryService assumptionCategoryService)
        {
            _assumptionDataService = assumptionDataService;
            _scenarioService = scenarioService;
            _assumptionCategoryService = assumptionCategoryService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssumptionDataViewModel();
            var selectList = _assumptionDataService.GetAssumptionDataConfig();
            viewModel.Scenarios = selectList.Scenarios.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.Configs = selectList.AssumptionDataConfigs.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name + "(" + x.Measurement + ")" }).ToList();


            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Create(AssumptionDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveAssumptionDataRequest>();
            var response = _assumptionDataService.SaveAssumptionData(request);
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
            var viewModel = _assumptionDataService.GetAssumptionData(new GetAssumptionDataRequest { Id = id }).MapTo<AssumptionDataViewModel>();
            var selectList = _assumptionDataService.GetAssumptionDataConfig();
            viewModel.Scenarios = selectList.Scenarios.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.Configs = selectList.AssumptionDataConfigs.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name + "(" + x.Measurement + ")" }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(AssumptionDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveAssumptionDataRequest>();
            var response = _assumptionDataService.SaveAssumptionData(request);
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
            var response = _assumptionDataService.DeleteAssumptionData(new DeleteAssumptionDataRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;

            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var assumptionData = _assumptionDataService.GetAssumptionDatas(new GetAssumptionDatasRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                Search = gridParams.Search,
                SortingDictionary = gridParams.SortingDictionary
            });
            IList<GetAssumptionDatasResponse.AssumptionData> DatasResponse = assumptionData.AssumptionDatas;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = assumptionData.TotalRecords,
                iTotalRecords = assumptionData.AssumptionDatas.Count,
                aaData = DatasResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Input(int scenarioId)
        {
            return View(GetAssumptionDataInputViewModel(scenarioId));
        }

        public ActionResult DetailPartial(int scenarioId)
        {
            return View("_DetailPartial", GetAssumptionDataInputViewModel(scenarioId));
        }

        [HttpPost]
        public ActionResult Save(AssumptionDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveAssumptionDataRequest>();
            var response = _assumptionDataService.SaveAssumptionData(request);
            return Json(response, JsonRequestBehavior.DenyGet);
        }

        private AssumptionDataInputViewModel GetAssumptionDataInputViewModel(int scenarioId)
        {
            var scenario = _scenarioService.GetScenario(new GetScenarioRequest { Id = scenarioId });
            var viewModel = new AssumptionDataInputViewModel();
            viewModel.Scenario = scenario.MapTo<AssumptionDataInputViewModel.ScenarioViewModel>();
            viewModel.KeyAssumptionCategories = _assumptionCategoryService.GetAssumptionCategories(new GetAssumptionCategoriesRequest { Take = -1, SortingDictionary = new Dictionary<string, SortOrder>(), IncludeAssumptionList = true })
                .AssumptionCategorys.MapTo<AssumptionDataInputViewModel.AssumptionCategoryViewModel>();
            viewModel.AssumptionDataList = _assumptionDataService.GetAssumptionDatas(new GetAssumptionDatasRequest
            {
                Take = -1,
                SortingDictionary = new SortedDictionary<string, SortOrder> { },
                ScenarioId = scenarioId
            }).AssumptionDatas.MapTo<AssumptionDataInputViewModel.AssumptionDataViewModel>();

            return viewModel;
        }


    }
}