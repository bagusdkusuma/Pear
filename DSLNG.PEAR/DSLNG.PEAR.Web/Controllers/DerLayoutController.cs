using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Der;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Services.Responses.Der;
using DSLNG.PEAR.Web.ViewModels.Artifact;
using DSLNG.PEAR.Web.ViewModels.DerLayout;
using DSLNG.PEAR.Web.ViewModels.DerLayout.LayoutType;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerLayoutController : BaseController
    {
        private readonly IDropdownService _dropdownService;
        private readonly IDerService _derService;
        private readonly IMeasurementService _measurementService;

        public DerLayoutController(IDropdownService dropdownService, IDerService derService, IMeasurementService measurementService)
        {
            _dropdownService = dropdownService;
            _derService = derService;
            _measurementService = measurementService;
        }

        public ActionResult Index()
        {
            var viewModel = new DerLayoutIndexViewModel();
            var response = _derService.GetDerLayouts();
            viewModel.DerLayouts = response.DerLayouts.Select(x => new DerLayoutViewModel() { Id = x.Id, IsActive = x.IsActive, Title = x.Title })
                    .ToList();
            // viewModel.DerLayouts.Add(new DerLayoutViewModel{Id = 1, Title = "First Layout"});
            return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new CreateDerLayoutViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateDerLayoutViewModel viewModel)
        {
            var request = new CreateOrUpdateDerLayoutRequest();
            request.IsActive = viewModel.IsActive;
            request.Title = viewModel.Title;
            var response = _derService.CreateOrUpdateDerLayout(request);
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        public ActionResult Config(int id) //this is derlayout id
        {
            var reponse = _derService.GetDerLayoutItems(id);
            var viewModel = reponse.MapTo<DerLayoutConfigViewModel>();
            viewModel.DerLayoutId = id;
            return View(viewModel);
        }

        public ActionResult LayoutItem(DerLayoutItemViewModel viewModel)
        {
            if (viewModel.Id > 0)
            {
                var response = _derService.GetDerLayoutItem(viewModel.Id);

                switch (response.Type.ToLowerInvariant())
                {
                    case "line":
                        //viewModel.LineChart = new LineChartViewModel();
                        break;
                }
                return View("LayoutItem", viewModel);
            }
            else
            {
                //var viewModel = new DerCreateLayoutItemViewModel();
                viewModel.Types = _dropdownService.GetDerItemTypes().MapTo<SelectListItem>();
                return View("LayoutItem", viewModel);
            }

            /*var viewModel = new DerCreateLayoutItemViewModel();
            viewModel.Types = _dropdownService.GetDerItemTypes().MapTo<SelectListItem>();
            viewModel.Row = vModel.Row;
            viewModel.Column = vModel.Column;
            viewModel.DerLayoutId = vModel.DerLayoutId;
            return PartialView("_ModalLayoutItem", viewModel);*/
        }

        public ActionResult LayoutSetting(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "line":
                    {
                        /*var viewModel = new DerLayoutLineViewModel();
                        viewModel.LineChart = new LineChartViewModel();
                        var series = new LineChartViewModel.SeriesViewModel();
                        viewModel.LineChart.Series.Add(series);
                        viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                        {
                            Take = -1,
                            SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                        }).Measurements
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                        return PartialView("LayoutType/_Line", viewModel);*/

                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.Artifact = new DerLayoutItemViewModel.DerLayoutItemArtifactViewModel();
                        viewModel.Artifact.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                        {
                            Take = -1,
                            SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                        }).Measurements
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                        viewModel.LineChart = new LineChartViewModel();
                        var series = new LineChartViewModel.SeriesViewModel();
                        viewModel.LineChart.Series.Add(series);
                        return PartialView("LayoutType/_Line", viewModel);
                    }

                case "multiaxis":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.Artifact = new DerLayoutItemViewModel.DerLayoutItemArtifactViewModel();
                        viewModel.MultiaxisChart = new MultiaxisChartViewModel();
                        var chart = new MultiaxisChartViewModel.ChartViewModel();
                        viewModel.MultiaxisChart.Charts.Add(chart);
                        viewModel.MultiaxisChart.GraphicTypes.Add(new SelectListItem { Value = "line", Text = "Line" });
                        viewModel.MultiaxisChart.ValueAxes.Add(new SelectListItem { Value = ValueAxis.KpiActual.ToString(), Text = "Kpi Actual" });
                        viewModel.MultiaxisChart.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                        {
                            Take = -1,
                            SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                        }).Measurements
             .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                        return PartialView("LayoutType/_MultiAxis", viewModel);
                    }
            }

            return Content("Error");
        }

        [HttpPost]
        public ActionResult SaveLayoutItem(DerLayoutItemViewModel layoutItemViewModel/*,
            DerLayoutLineViewModel lineViewModel, DerLayoutMultiAxisViewModel multiAxisViewModel*/)
        {
            var request = new SaveLayoutItemRequest();
            var response = new SaveLayoutItemResponse();
            switch (layoutItemViewModel.Type.ToLowerInvariant())
            {
                case "line":
                    {
                        /*request.DerLayoutId = layoutItemViewModel.DerLayoutId;
                        request.Column = layoutItemViewModel.Column;
                        request.Row = layoutItemViewModel.Row;
                        request.Type = layoutItemViewModel.Type;
                        request.Artifact = new SaveLayoutItemRequest.LayoutItemArtifact();
                        request.Artifact.HeaderTitle = layoutItemViewModel.Artifact.HeaderTitle;
                        request.Artifact.MeasurementId = layoutItemViewModel.Artifact.MeasurementId;
                        request.Artifact.LineChart = new SaveLayoutItemRequest.LayoutItemArtifactLine();
                        foreach (var serie in layoutItemViewModel.LineChart.Series)
                        {
                            request.Artifact.LineChart.Series.Add(new SaveLayoutItemRequest.LayoutItemArtifactSerie()
                            {
                                Color = serie.Color,
                                KpiId = serie.KpiId,
                                Label = serie.Label
                            });

                        }*/

                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.Artifact = layoutItemViewModel.Artifact.MapTo<SaveLayoutItemRequest.LayoutItemArtifact>();
                        request.Artifact.LineChart = layoutItemViewModel.LineChart.MapTo<SaveLayoutItemRequest.LayoutItemArtifactLine>();
                        response = _derService.SaveLayoutItem(request);

                        break;
                    }
                    
                case "multiaxis":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.Artifact = layoutItemViewModel.Artifact.MapTo<SaveLayoutItemRequest.LayoutItemArtifact>();
                        request.Artifact.MultiAxis = layoutItemViewModel.MultiaxisChart.MapTo<SaveLayoutItemRequest.LayoutItemArtifactMultiAxis>();
                        response = _derService.SaveLayoutItem(request);
                        break;    
                    }
                    

            }

            return Content(response.Message);
        }
    }
}