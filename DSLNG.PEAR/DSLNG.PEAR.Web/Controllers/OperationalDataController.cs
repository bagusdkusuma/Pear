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

namespace DSLNG.PEAR.Web.Controllers
{
    public class OperationalDataController : Controller
    {
        private IOperationalDataService _operationalDataService;
        public OperationalDataController(IOperationalDataService operationalDataService)
        {
            _operationalDataService = operationalDataService;
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
            e.DataRowCount = _operationalDataService.GetOperationalDatas(new GetOperationalDatasRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _operationalDataService.GetOperationalDatas(new GetOperationalDatasRequest
                {
                    Skip = e.StartDataRowIndex,
                    Take = e.DataRowCount
                }).OperationalDatas;
        }


        public ActionResult Create()
        {
            var viewModel = new OperationalDataViewModel();
            viewModel.KeyOperations = _operationalDataService.GetOperationalSelectList().Operations
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.KPIS = _operationalDataService.GetOperationalSelectList().KPIS
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);

        }

        [HttpPost]
        public ActionResult Create(OperationalDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationalDataRequest>();
            var response = _operationalDataService.SaveOperationalData(request);
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
            var viewModel = _operationalDataService.GetOperationalData(new GetOperationalDataRequest { Id = id }).MapTo<OperationalDataViewModel>();
            viewModel.KeyOperations = _operationalDataService.GetOperationalSelectList().Operations
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            viewModel.KPIS = _operationalDataService.GetOperationalSelectList().KPIS
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(OperationalDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOperationalDataRequest>();
            var response = _operationalDataService.SaveOperationalData(request);
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
            var response = _operationalDataService.DeleteOperationalData(new DeleteOperationalDataRequest { Id = id });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}