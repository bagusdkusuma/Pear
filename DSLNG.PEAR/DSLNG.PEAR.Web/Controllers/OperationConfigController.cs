using System.Data.SqlClient;
using DSLNG.PEAR.Services.Requests.Kpi;
using DSLNG.PEAR.Services.Requests.OperationGroup;
using DSLNG.PEAR.Web.ViewModels.OperationConfig;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Operation;
using DSLNG.PEAR.Web.ViewModels.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Services.Responses.Operation;

namespace DSLNG.PEAR.Web.Controllers
{
    public class OperationConfigController : BaseController
    {
        private readonly IOperationConfigService _operationConfigService;
        private readonly IKpiService _kpiService;
        private readonly IOperationGroupService _operationGroupService;

        public OperationConfigController(IKpiService kpiService, IOperationGroupService operationGroupService,
                                         IOperationConfigService operationConfigService)
        {
            _kpiService = kpiService;
            _operationGroupService = operationGroupService;
            _operationConfigService = operationConfigService;
        }

        public ActionResult Index()
        {
            var viewModel = new OperationConfigIndexViewModel();
            viewModel.OperationGroups = _operationGroupService.GetOperationGroups(new GetOperationGroupsRequest
                {
                    Take = -1,
                    SortingDictionary = new Dictionary<string, SortOrder> {{"Order", SortOrder.Ascending}}
                }).OperationGroups.Select(x => new SelectListItem {Value = x.Id.ToString(), Text = x.Name}).ToList();
            viewModel.OperationGroups.Insert(0, new SelectListItem {Value = "0", Text = "Choose Group"});
            return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new OperationViewModel();
            viewModel.KeyOperationGroups = _operationConfigService.GetOperationGroups().OperationGroups
                                                                  .Select(
                                                                      x =>
                                                                      new SelectListItem
                                                                          {
                                                                              Value = x.Id.ToString(),
                                                                              Text = x.Name
                                                                          }).ToList();
            viewModel.Kpis.Insert(0, new OperationViewModel.Kpi());
            viewModel.IsActive = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(OperationViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationRequest>();
            var response = _operationConfigService.SaveOperation(request);
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
            var viewModel =
                _operationConfigService.GetOperation(new GetOperationRequest {Id = id}).MapTo<OperationViewModel>();
            viewModel.KeyOperationGroups = _operationConfigService.GetOperationGroups().OperationGroups
                                                                  .Select(
                                                                      x =>
                                                                      new SelectListItem
                                                                          {
                                                                              Value = x.Id.ToString(),
                                                                              Text = x.Name
                                                                          }).ToList();
            viewModel.Kpis = new List<OperationViewModel.Kpi>();
            var kpi = _kpiService.GetKpi(new GetKpiRequest {Id = viewModel.KpiId});
            viewModel.Kpis.Insert(0, new OperationViewModel.Kpi {Id = kpi.Id, Name = kpi.Name});

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(OperationViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationRequest>();
            var response = _operationConfigService.SaveOperation(request);
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
            var response = _operationConfigService.DeleteOperation(new DeleteOperationRequest {Id = id});
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
            var operation = _operationConfigService.GetOperations(new GetOperationsRequest
                {
                    Search = gridParams.Search,
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    SortingDictionary = gridParams.SortingDictionary
                });
            IList<GetOperationsResponse.Operation> operationsResponse = operation.Operations;
            var data = new
                {
                    sEcho = gridParams.Echo + 1,
                    iTotalDisplayRecords = operation.TotalRecords,
                    iTotalRecords = operation.Operations.Count,
                    aaData = operationsResponse
                };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(OperationConfigUpdateViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateOperationRequest>();
            return Json(_operationConfigService.UpdateOperation(request));
        }
    }
}