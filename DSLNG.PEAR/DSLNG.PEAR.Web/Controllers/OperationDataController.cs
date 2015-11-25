using AutoMapper;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Web.ViewModels.OperationData;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OperationalData;
using DSLNG.PEAR.Web.ViewModels.OperationalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class OperationDataController : Controller
    {
        private readonly IOperationDataService _operationDataService;
        private readonly IDropdownService _dropdownService;
        public OperationDataController(IOperationDataService operationDataService, IDropdownService dropdownService)
        {
            _operationDataService = operationDataService;
            _dropdownService = dropdownService;
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridOperationalData");
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
            var viewModel = GridViewExtension.GetViewModel("gridAssumptiondDataIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        private static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("KeyOperation");
            viewModel.Columns.Add("KPI");
            viewModel.Columns.Add("ActualValue");
            viewModel.Columns.Add("ForecastValue");
            viewModel.Columns.Add("Remark");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = _operationDataService.GetOperationalDatas(new GetOperationalDatasRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _operationDataService.GetOperationalDatas(new GetOperationalDatasRequest
                {
                    Skip = e.StartDataRowIndex,
                    Take = e.DataRowCount
                }).OperationalDatas;
        }


        public ActionResult Create()
        {
            var viewModel = new OperationalDataViewModel();
            viewModel.KeyOperations = _operationDataService.GetOperationalSelectList().Operations
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.KPIS = _operationDataService.GetOperationalSelectList().KPIS
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);

        }

        [HttpPost]
        public ActionResult Create(OperationalDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationalDataRequest>();
            var response = _operationDataService.SaveOperationalData(request);
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
            var viewModel = _operationDataService.GetOperationalData(new GetOperationalDataRequest { Id = id }).MapTo<OperationalDataViewModel>();
            viewModel.KeyOperations = _operationDataService.GetOperationalSelectList().Operations
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.KPIS = _operationDataService.GetOperationalSelectList().KPIS
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(OperationalDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationalDataRequest>();
            var response = _operationDataService.SaveOperationalData(request);
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
            var response = _operationDataService.DeleteOperationalData(new DeleteOperationalDataRequest { Id = id });
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
            var operational = _operationDataService.GetOperationalDatas(new GetOperationalDatasRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = operational.TotalRecords,
                iTotalRecords = operational.OperationalDatas.Count,
                aaData = operational.OperationalDatas
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            var response =
                _operationDataService.GetOperationalDataDetail(new GetOperationalDataDetailRequest() {Id = id});
            var viewModel = response.MapTo<OperationDataDetailViewModel>();
            viewModel.ScenarioId = id;
            return View(viewModel);
        }

        public ActionResult ConfigurationPartial(OperationDataParamConfigurationViewModel paramViewModel)
        {
            PeriodeType pType = string.IsNullOrEmpty(paramViewModel.PeriodeType)
                                    ? PeriodeType.Yearly
                                    : (PeriodeType)Enum.Parse(typeof(PeriodeType), paramViewModel.PeriodeType);

            var request = paramViewModel.MapTo<GetOperationDataConfigurationRequest>();
            var response = _operationDataService.GetOperationDataConfiguration(request);

            var viewModel = response.MapTo<OperationDataConfigurationViewModel>();
            viewModel.Years = _dropdownService.GetYears().MapTo<SelectListItem>();
            viewModel.PeriodeType = pType.ToString();
            viewModel.Year = request.Year;
            return PartialView("Configuration/_" + viewModel.PeriodeType, viewModel);
            
        }

        public ActionResult Configuration(OperationDataParamConfigurationViewModel paramViewModel)
        {
            PeriodeType pType = string.IsNullOrEmpty(paramViewModel.PeriodeType)
                                    ? PeriodeType.Yearly
                                    : (PeriodeType)Enum.Parse(typeof(PeriodeType), paramViewModel.PeriodeType);
            var request = paramViewModel.MapTo<GetOperationDataConfigurationRequest>();
            var response = _operationDataService.GetOperationDataConfiguration(request);

            var viewModel = response.MapTo<OperationDataConfigurationViewModel>();
            viewModel.Years = _dropdownService.GetYears().MapTo<SelectListItem>();
            viewModel.PeriodeType = pType.ToString();
            viewModel.Year = request.Year;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateOperationDataViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateOperationDataRequest>();
            var response = _operationDataService.Update(request);
            return Json(new { Message = response.Message, isSuccess = response.IsSuccess });
        }
    }
}