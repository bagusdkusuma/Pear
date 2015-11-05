using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.AssumptionCategory;
using DSLNG.PEAR.Services.Requests.AssumptionConfig;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Web.ViewModels.AssumptionConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Web.Controllers
{
    public class AssumptionConfigController : BaseController
    {
        private IAssumptionConfigService _assumptionConfigService;
        private readonly IMeasurementService _measurementService;
        public AssumptionConfigController(IAssumptionConfigService assumptionConfigService, IMeasurementService measurementService)
        {
            _assumptionConfigService = assumptionConfigService;
            _measurementService = measurementService;
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridAssumptionConfig");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return BindingCore(viewModel);
        }

        PartialViewResult BindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(GetDataRowCount, GetData);
            return PartialView("_IndexGridPartial", gridViewModel);
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridAssumptionConfigIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        private static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Category");
            viewModel.Columns.Add("Measurement");
            viewModel.Columns.Add("Order");
            viewModel.Columns.Add("Remark");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = _assumptionConfigService.GetAssumptionConfigs(new GetAssumptionConfigsRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _assumptionConfigService.GetAssumptionConfigs(new GetAssumptionConfigsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).AssumptionConfigs;
        }

        public ActionResult Create()
        {
            var viewModel = new AssumptionConfigViewModel();
            viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest()).Measurements
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.Categories = _assumptionConfigService.GetAssumptionConfigCategories().AssumptionConfigCategoriesResponse
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(AssumptionConfigViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveAssumptionConfigRequest>();
            var response = _assumptionConfigService.SaveAssumptionConfig(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Create", viewModel);
        }

        public ActionResult Edit (int id)
        {
            var viewModel = _assumptionConfigService.GetAssumptionConfig(new GetAssumptionConfigRequest { Id = id }).MapTo<AssumptionConfigViewModel>();
            
            viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest()).Measurements
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.Categories = _assumptionConfigService.GetAssumptionConfigCategories().AssumptionConfigCategoriesResponse
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Edit(AssumptionConfigViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveAssumptionConfigRequest>();
            var response = _assumptionConfigService.SaveAssumptionConfig(request);
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
            var request = _assumptionConfigService.DeleteAssumptionConfig(new DeleteAssumptionConfigRequest { Id = id });
            TempData["IsSuccess"] = request.IsSuccess;
            TempData["Message"] = request.Message;
            if (request.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}