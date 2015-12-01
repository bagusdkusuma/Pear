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
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Services.Responses.AssumptionConfig;

namespace DSLNG.PEAR.Web.Controllers
{
    public class AssumptionConfigController : BaseController
    {
        private IAssumptionConfigService _assumptionConfigService;
        public AssumptionConfigController(IAssumptionConfigService assumptionConfigService)
        {
            _assumptionConfigService = assumptionConfigService;
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
            var Selectlist = _assumptionConfigService.GetAssumptionConfigCategories();

            viewModel.Measurements = Selectlist.MeasurementsSelectList.Select
                (x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            viewModel.Measurements.Insert(0, new SelectListItem { Value = "0", Text = "No Measurement" });
            viewModel.Categories = Selectlist.AssumptionConfigCategoriesResponse.Select
                (x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            viewModel.IsActive = true;


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
            var Selectlist = _assumptionConfigService.GetAssumptionConfigCategories();

            viewModel.Measurements = Selectlist.MeasurementsSelectList.Select
                (x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            viewModel.Measurements.Insert(0, new SelectListItem { Value = "0", Text = "No Measurement" });
            viewModel.Categories = Selectlist.AssumptionConfigCategoriesResponse.Select
                (x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

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
                return RedirectToAction("Index");

        }


        public ActionResult Grid(GridParams gridParams)
        {
            var assumptionConfig = _assumptionConfigService.GetAssumptionConfigs(new GetAssumptionConfigsRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            IList<GetAssumptionConfigsResponse.AssumptionConfig> assumptionConfigResponse = assumptionConfig.AssumptionConfigs;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = assumptionConfig.TotalRecords,
                iTotalRecords = assumptionConfig.AssumptionConfigs.Count,
                aaData = assumptionConfigResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}