using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.EconomicConfig;
using DSLNG.PEAR.Web.ViewModels.EconomicConfigDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class EconomicConfigController : Controller
    {
        private IEconomicConfigService _economicConfigService;
        public EconomicConfigController(IEconomicConfigService economicConfigService)
        {
            _economicConfigService = economicConfigService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridEconomicConfigIndex");
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
            viewModel.Columns.Add("Scenario");
            viewModel.Columns.Add("EconomicSummary");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;

        }


        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridEconomicConfigIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }



        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = _economicConfigService.GetEconomicConfigs(new GetEconomicConfigsRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _economicConfigService.GetEconomicConfigs(new GetEconomicConfigsRequest
                {
                    Skip = e.StartDataRowIndex,
                    Take = e.DataRowCount
                }).EconomicConfigs;
        }

        public ActionResult Create()
        {
            var viewModel = new EconomicConfigViewModel();
            viewModel.Scenarios = _economicConfigService.GetEconomicConfigSelectList().Scenarios
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.EconomicSummaries = _economicConfigService.GetEconomicConfigSelectList().EconomicSummaries
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            viewModel.IsActive = true;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(EconomicConfigViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveEconomicConfigRequest>();
            var response = _economicConfigService.SaveEconomicConfig(request);
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
            var viewModel = _economicConfigService.GetEconomicConfig(new GetEconomicConfigRequest { Id = id }).MapTo<EconomicConfigViewModel>();
            viewModel.Scenarios = _economicConfigService.GetEconomicConfigSelectList().Scenarios
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.EconomicSummaries = _economicConfigService.GetEconomicConfigSelectList().EconomicSummaries
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
            
        }

        [HttpPost]
        public ActionResult Edit(EconomicConfigViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveEconomicConfigRequest>();
            var response = _economicConfigService.SaveEconomicConfig(request);
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
            var response = _economicConfigService.DeleteEconomicConfig(new DeleteEconomicConfigRequest { Id = id });
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
            var economic = _economicConfigService.GetEconomicConfigs(new GetEconomicConfigsRequest
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
                iTotalRecords = economic.EconomicConfigs.Count,
                aaData = economic.EconomicConfigs
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
	}
}