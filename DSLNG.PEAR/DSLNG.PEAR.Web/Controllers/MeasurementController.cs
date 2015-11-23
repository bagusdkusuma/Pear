using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Measurement;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Measurement;
using DevExpress.Web.Mvc;
using System.Collections.Generic;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Services.Responses.Measurement;

namespace DSLNG.PEAR.Web.Controllers
{
    public class MeasurementController : BaseController
    {

        private readonly IMeasurementService _measurementService;

        public MeasurementController(IMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridMeasurementIndex");
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
            viewModel.Columns.Add("Remark");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridMeasurementIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _measurementService.GetMeasurements(new GetMeasurementsRequest()).Measurements.Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _measurementService.GetMeasurements(new GetMeasurementsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Measurements;
        }

        public ActionResult Create()
        {
            var viewModel = new CreateMeasurementViewModel();
            viewModel.IsActive = true;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateMeasurementViewModel viewModel)
        {
            var request = viewModel.MapTo<CreateMeasurementRequest>();
            var response = _measurementService.Create(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View("Create", viewModel);
        }

        public ActionResult Update(int id)
        {
            var response = _measurementService.GetMeasurement(new GetMeasurementRequest {Id = id});
            var viewModel = response.MapTo<UpdateMeasurementViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(UpdateMeasurementViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdateMeasurementRequest>();
            var response = _measurementService.Update(request);
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
            var response = _measurementService.Delete(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        
        public ActionResult Grid(GridParams gridParams)
        {
            var measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    SortingDictionary = gridParams.SortingDictionary,
                    Search = gridParams.Search
                });
            IList<GetMeasurementsResponse.Measurement> measurementsResponse = measurements.Measurements;
            var data = new 
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = measurements.TotalRecords,
                iTotalRecords = measurements.Measurements.Count,
                aaData = measurementsResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        
    }
}