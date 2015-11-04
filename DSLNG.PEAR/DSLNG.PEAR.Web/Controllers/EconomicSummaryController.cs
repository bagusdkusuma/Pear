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

namespace DSLNG.PEAR.Web.Controllers
{
    public class EconomicSummaryController : Controller
    {
        private IEconomicSummaryService _economicSummaryService;
        public EconomicSummaryController(IEconomicSummaryService economicSummaryService)
        {
            _economicSummaryService = economicSummaryService;
        }
        
        public ActionResult Config()
        {
            return View();
        }

        public ActionResult ConfigPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridEconomicSummary");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return BindingCore(viewModel);
        }

        PartialViewResult BindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(GetDataRowCount, GetData);
            return PartialView("_ConfigGridPartial", gridViewModel);
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridEconomicSummaryConfig");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
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

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = _economicSummaryService.GetEconomicSummaries(new GetEconomicSummariesRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _economicSummaryService.GetEconomicSummaries(new GetEconomicSummariesRequest
                {
                    Skip = e.StartDataRowIndex,
                    Take = e.DataRowCount
                }).EconomicSummaries;
       }


        public ActionResult Create()
        {
            var viewModel = new EconomicSummaryViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(EconomicSummaryViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveEconomicSummaryRequest>();
            var response = _economicSummaryService.SaveEconomicSummary(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Config");
            }
            return View("Create", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var viewModel = _economicSummaryService.GetEconomicSummary(new GetEconomicSummaryRequest { Id = id }).MapTo<EconomicSummaryViewModel>();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EconomicSummaryViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveEconomicSummaryRequest>();
            var response = _economicSummaryService.SaveEconomicSummary(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Config");
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
	}
}