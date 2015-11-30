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
using DSLNG.PEAR.Common.Contants;

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

        public ActionResult Create()
        {
            var viewModel = new OperationalDataViewModel();
            var selectList = _operationDataService.GetOperationalSelectList();
            viewModel.KeyOperations = selectList.Operations.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.KPIS = selectList.KPIS.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

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
            var selectList = _operationDataService.GetOperationalSelectList();
            viewModel.KeyOperations = selectList.Operations.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.KPIS = selectList.KPIS.Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

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
                aaData = operational.OperationalDatas.Select(x => new {
                    x.Id,
                    x.KeyOperation,
                    x.Kpi,
                    Periode = x.Periode.ToString(DateFormat.DateForGrid),
                    x.PeriodeType,
                    x.Remark,
                    x.Scenario,
                    x.Value               
                })
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

        //actually it can also be processed by check if it is ajax request but you know.. deadline happens
        public ActionResult ConfigurationPartial(OperationDataParamConfigurationViewModel paramViewModel)
        {
            var viewModel = ConfigurationViewModel(paramViewModel, null);
            return PartialView("Configuration/_" + viewModel.PeriodeType, viewModel);
        }

        public ActionResult Configuration(OperationDataParamConfigurationViewModel paramViewModel)
        {
            var viewModel = ConfigurationViewModel(paramViewModel, null);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateOperationDataViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateOperationDataRequest>();
            var response = _operationDataService.Update(request);
            return Json(new { Message = response.Message, isSuccess = response.IsSuccess });
        }

        public ActionResult DetailPartial(OperationDataParamConfigurationViewModel paramViewModel)
        {
            return View("_DetailPartial", ConfigurationViewModel(paramViewModel, true));
        }

        public ActionResult DetailPartialPeriodeType(OperationDataParamConfigurationViewModel paramViewModel)
        {
            var viewModel = ConfigurationViewModel(paramViewModel, true);
            return PartialView("DetailPartial/_" + viewModel.PeriodeType, viewModel);
        }

        private OperationDataConfigurationViewModel ConfigurationViewModel(OperationDataParamConfigurationViewModel paramViewModel,  bool? isIncludeGroup)
        {
            PeriodeType pType = string.IsNullOrEmpty(paramViewModel.PeriodeType)
                                    ? PeriodeType.Yearly
                                    : (PeriodeType)Enum.Parse(typeof(PeriodeType), paramViewModel.PeriodeType);

            var request = paramViewModel.MapTo<GetOperationDataConfigurationRequest>();
            request.PeriodeType = pType;
            request.IsPartial = isIncludeGroup.HasValue && isIncludeGroup.Value;
            var response = _operationDataService.GetOperationDataConfiguration(request);
            var viewModel = response.MapTo<OperationDataConfigurationViewModel>();
            viewModel.Years = _dropdownService.GetYears().MapTo<SelectListItem>();
            viewModel.PeriodeType = pType.ToString();
            viewModel.Year = request.Year;
            return viewModel;
        }
    }
}