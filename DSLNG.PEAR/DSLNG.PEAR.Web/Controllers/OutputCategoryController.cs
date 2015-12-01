using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OutputCategory;
using DSLNG.PEAR.Services.Responses.OutputCategory;
using DSLNG.PEAR.Web.ViewModels.OutputCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class OutputCategoryController : BaseController
    {

        private IOutputCategoryService _outputCategoryService;
        public OutputCategoryController(IOutputCategoryService outputCategoryService)
        {
            _outputCategoryService = outputCategoryService;
        }

        //
        // GET: /OutputCategory/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridOutputIndex");
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


        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridOutputIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }


        private static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Order");
            viewModel.Columns.Add("Desc");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = _outputCategoryService.GetOutputCategories(new GetOutputCategoriesRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _outputCategoryService.GetOutputCategories(new GetOutputCategoriesRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).OutputCategories;
        }



        public ActionResult Create()
        {
            var viewModel = new OutputCategoryViewModel();
            viewModel.IsActive = true;
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Create(OutputCategoryViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOutputCategoryRequest>();
            var response = _outputCategoryService.SaveOutputCategory(request);
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
            var viewModel = _outputCategoryService.GetOutputCategory(new GetOutputCategoryRequest { Id = id }).MapTo<OutputCategoryViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(OutputCategoryViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOutputCategoryRequest>();
            var response = _outputCategoryService.SaveOutputCategory(request);
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
            var request = _outputCategoryService.DeleteOutputCategory(new DeleteOutputCategoryRequest { Id = id });
            TempData["IsSuccess"] = request.IsSuccess;
            TempData["Message"] = request.Message;

            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var outputCategory = _outputCategoryService.GetOutputCategories(new GetOutputCategoriesRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = outputCategory.TotalRecords,
                iTotalRecords = outputCategory.OutputCategories.Count,
                aaData = outputCategory.OutputCategories
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
    }
}