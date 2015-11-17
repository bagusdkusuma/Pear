using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Conversion;
using DSLNG.PEAR.Services.Responses.Conversion;
using DSLNG.PEAR.Web.ViewModels.Conversion;
using DevExpress.Web.Mvc;
using System.Data.SqlClient;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class ConversionController : BaseController
    {
        private readonly IConversionService _conversionService;
        private readonly IMeasurementService _measurementService;
        public ConversionController(IConversionService conversionService, IMeasurementService measurementService)
        {
            _conversionService = conversionService;
            _measurementService = measurementService;
        }
        // GET: Conversion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridConversionIndex");
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
            return PartialView("_GridViewPartial", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("FromName");
            viewModel.Columns.Add("ToName");
            viewModel.Columns.Add("Value");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("IsReverse");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridConversionIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _conversionService.GetConversions(new GetConversionsRequest()).Conversions.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _conversionService.GetConversions(new GetConversionsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Conversions;
        }

        public CreateConversionViewModel CreateViewModel(CreateConversionViewModel viewModel)
        {
            viewModel.MeasurementList = _measurementService.GetMeasurements(
                new Services.Requests.Measurement.GetMeasurementsRequest {
                    Take = -1,
                    SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                }).Measurements.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

            return viewModel;
        }

        public ActionResult Create()
        {
            var viewModel = new CreateConversionViewModel();
            viewModel = CreateViewModel(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateConversionViewModel viewModel)
        {
            var request = viewModel.MapTo<CreateConversionRequest>();
            var response = _conversionService.Create(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Create", viewModel);
        }

        public UpdateConversionViewModel UpdateViewModel(UpdateConversionViewModel viewModel)
        {
            viewModel.MeasurementList = _measurementService.GetMeasurements(
                new Services.Requests.Measurement.GetMeasurementsRequest {
                    Take = -1,
                    SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                }).Measurements.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

            return viewModel;
        }

        public ActionResult Update(int id)
        {
            var response = _conversionService.GetConversion(new GetConversionRequest { Id = id });
            var viewModel = response.MapTo<UpdateConversionViewModel>();

            viewModel = UpdateViewModel(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateConversionViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateConversionRequest>();
            var response = _conversionService.Update(request);
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
            var response = _conversionService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var conversion = _conversionService.GetConversions(new GetConversionsRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = conversion.TotalRecords,
                iTotalRecords = conversion.Conversions.Count,
                aaData = conversion.Conversions
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}