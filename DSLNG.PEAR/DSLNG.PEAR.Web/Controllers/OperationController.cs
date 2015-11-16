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
    public class OperationController : BaseController
    {
        private IOperationService _operationService;
        public OperationController(IOperationService operationService)
        {
            _operationService = operationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridOperation");
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
            var viewModel = GridViewExtension.GetViewModel("gridOperationIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        private static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("KeyOperationGroup");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Desc");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = _operationService.GetOperations(new GetOperationsRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _operationService.GetOperations(new GetOperationsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Operations;
        }

        public ActionResult Create()
        {
            var viewModel = new OperationViewModel();
            viewModel.KeyOperationGroups = _operationService.GetOperationGroups().OperationGroups
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(OperationViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationRequest>();
            var response = _operationService.SaveOperation(request);
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
            var viewModel = _operationService.GetOperation(new GetOperationRequest { Id = id }).MapTo<OperationViewModel>();
            viewModel.KeyOperationGroups = _operationService.GetOperationGroups().OperationGroups
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(OperationViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationRequest>();
            var response = _operationService.SaveOperation(request);
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
            var response = _operationService.DeleteOperation(new DeleteOperationRequest { Id = id });
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
            var operation = _operationService.GetOperations(new GetOperationsRequest
            {
                Search = gridParams.Search,
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary
            });
            IList<GetOperationsResponse.Operation> OperationsResponse = operation.Operations;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = operation.TotalRecords,
                iTotalRecords = operation.Operations.Count,
                aaData = OperationsResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
	}
}