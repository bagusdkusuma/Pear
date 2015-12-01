using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OperationGroup;
using DSLNG.PEAR.Web.ViewModels.OperationGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;


namespace DSLNG.PEAR.Web.Controllers
{
    public class OperationGroupController : BaseController
    {
        private IOperationGroupService _operationGroupService;
        public OperationGroupController(IOperationGroupService OperationGroupService)
        {
            _operationGroupService = OperationGroupService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridOperationGroupIndex");
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
            viewModel.Columns.Add("Order");
            viewModel.Columns.Add("Remark");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;

        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridOperationDataCategoryIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = _operationGroupService.GetOperationGroups(new GetOperationGroupsRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _operationGroupService.GetOperationGroups(new GetOperationGroupsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).OperationGroups;
        }

        public ActionResult Create()
        {
            var viewModel = new OperationGroupViewModel();
            viewModel.IsActive = true;
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Create(OperationGroupViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationGroupRequest>();
            var response = _operationGroupService.SaveOperationGroup(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ViewBag.IsActive = true;
            return View("Create", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var viewModel = _operationGroupService.GetOperationGroup(new GetOperationGroupRequest { Id = id }).MapTo<OperationGroupViewModel>();
            return View(viewModel);

        }

        [HttpPost]
        public ActionResult Edit(OperationGroupViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationGroupRequest>();
            var response = _operationGroupService.SaveOperationGroup(request);
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
            var request = _operationGroupService.DeleteOperationGroup(new DeleteOperationGroupRequest { Id = id });
            TempData["IsSuccess"] = request.IsSuccess;
            TempData["Message"] = request.Message;
            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var operationGroup = _operationGroupService.GetOperationGroups(new GetOperationGroupsRequest
                {
                    Search = gridParams.Search,
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    SortingDictionary = gridParams.SortingDictionary
                });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = operationGroup.TotalRecords,
                iTotalRecords = operationGroup.OperationGroups.Count,
                aaData = operationGroup.OperationGroups
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
    }
}