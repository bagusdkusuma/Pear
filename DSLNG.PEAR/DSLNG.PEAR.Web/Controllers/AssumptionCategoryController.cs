using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.AssumptionCategory;
using DSLNG.PEAR.Web.ViewModels.AssumptionCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Responses.AssumptionCategory;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class AssumptionCategoryController : BaseController
    {
        private IAssumptionCategoryService _assumptionCategoryService;
        public AssumptionCategoryController(IAssumptionCategoryService assumptionCategoryService)
        {
            _assumptionCategoryService = assumptionCategoryService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridAssumptionIndex");
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
            viewModel.Columns.Add("Desc");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;

        }


        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridAssumptionCategoryIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }



        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = _assumptionCategoryService.GetAssumptionCategories(new GetAssumptionCategoriesRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _assumptionCategoryService.GetAssumptionCategories(new GetAssumptionCategoriesRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).AssumptionCategorys;
        }


        // GET: /AssumptionCategory/Create
        public ActionResult Create()
        {
            var viewModel = new AssumptionCategoryViewModel();
            viewModel.IsActive = true;
            return View(viewModel);
        }



        // POST: /AssumptionCategory/Create
        [HttpPost]
        public ActionResult Create(AssumptionCategoryViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveAssumptionCategoryRequest>();
            var response = _assumptionCategoryService.SaveAssumptionCategory(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View("Create", viewModel);
        }

        // GET: /AssumptionCategory/Edit
        public ActionResult Edit(int id)
        {
            var viewModel = _assumptionCategoryService.GetAssumptionCategory(new GetAssumptionCategoryRequest { Id = id }).MapTo<AssumptionCategoryViewModel>();
            return View(viewModel);
        }

        // POST: /AssumptionCategory/Create
        [HttpPost]
        public ActionResult Edit(AssumptionCategoryViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveAssumptionCategoryRequest>();
            var response = _assumptionCategoryService.SaveAssumptionCategory(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Edit", viewModel);
        }

        // POST: /AssumptionCategory/Delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var request = _assumptionCategoryService.DeleteAssumptionCategory(new DeleteAssumptionCategoryRequest { Id = id });
            TempData["IsSuccess"] = request.IsSuccess;
            TempData["Message"] = request.Message;
            return RedirectToAction("Index");

        }

        public ActionResult Grid(GridParams gridParams)
        {
            var assumptionCategory = _assumptionCategoryService.GetAssumptionCategories(new GetAssumptionCategoriesRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    SortingDictionary = gridParams.SortingDictionary,
                    Search = gridParams.Search
                });
            IList<GetAssumptionCategoriesResponse.AssumptionCategory> CategoriesResponse = assumptionCategory.AssumptionCategorys;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = assumptionCategory.TotalRecords,
                iTotalRecords = assumptionCategory.AssumptionCategorys.Count,
                aaData = CategoriesResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
    }
}