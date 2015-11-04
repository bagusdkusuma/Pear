using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Select;
using DSLNG.PEAR.Web.ViewModels.Select;
using DevExpress.Web.Mvc;
using System;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Web.Controllers
{
    public class SelectController : BaseController
    {
        private readonly ISelectService _selectService;

        public SelectController(ISelectService selectService)
        {
            _selectService = selectService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridSelectIndex");
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

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Options");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridSelectIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _selectService.GetSelects(new GetSelectsRequest()).Selects.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _selectService.GetSelects(new GetSelectsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Selects;
        }

        public ActionResult Create()
        {
            var viewModel = new CreateSelectViewModel();
            foreach (var name in Enum.GetNames(typeof(SelectType)))
            {
                    viewModel.Types.Add(new SelectListItem { Text = name, Value = name });
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateSelectViewModel viewModel)
        {
            var request = viewModel.MapTo<CreateSelectRequest>();
            var response = _selectService.Create(request);
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        public ActionResult Update(int id)
        {
            var response = _selectService.GetSelect(new GetSelectRequest { Id = id });
            var viewModel = response.MapTo<UpdateSelectViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateSelectViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateSelectRequest>();
            var response = _selectService.Update(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Update", viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var response = _selectService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }
	}
}