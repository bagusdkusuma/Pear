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
using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Services.Requests.Select;
using DSLNG.PEAR.Services.Responses.Der;
using DSLNG.PEAR.Web.ViewModels.Artifact;
using DSLNG.PEAR.Web.ViewModels.DerLayout;
using DSLNG.PEAR.Web.ViewModels.DerLayout.LayoutType;
using DSLNG.PEAR.Web.Grid;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerLayoutController : BaseController
    {
        private readonly IDropdownService _dropdownService;
        private readonly IDerService _derService;
        private readonly IMeasurementService _measurementService;
        private readonly IHighlightService _highlightService;
        private readonly ISelectService _selectService;

        public DerLayoutController(IDropdownService dropdownService, IDerService derService, IMeasurementService measurementService, IHighlightService highlightService, ISelectService selectService)
        {
            _dropdownService = dropdownService;
            _derService = derService;
            _measurementService = measurementService;
            _highlightService = highlightService;
            _selectService = selectService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var viewModel = new DerLayoutIndexViewModel();
            var response = _derService.GetDerLayouts();
            viewModel.DerLayouts = response.DerLayouts.Select(x => new DerLayoutViewModel() { Id = x.Id, IsActive = x.IsActive, Title = x.Title })
                    .ToList();
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = viewModel.DerLayouts.Count,
                iTotalRecords = viewModel.DerLayouts.Count,
                aaData = viewModel.DerLayouts
            };
            return Json(data, JsonRequestBehavior.AllowGet);
            //// viewModel.DerLayouts.Add(new DerLayoutViewModel{Id = 1, Title = "First Layout"});
            //return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new CreateDerLayoutViewModel();

            return View(viewModel);
        }

        public ActionResult Edit(int id)
        {
            var viewModel = new CreateDerLayoutViewModel();
            var response = _derService.GetDerLayout(id);
            viewModel.Id = response.Id;
            viewModel.Title = response.Title;
            viewModel.IsActive = response.IsActive;

            return View(viewModel);
        }

        public ActionResult DeleteLayout(int id)
        {
            var response = _derService.DeleteLayout(id);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Create(CreateDerLayoutViewModel viewModel)
        {
            var request = new CreateOrUpdateDerLayoutRequest();
            request.Id = viewModel.Id;
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
                var editViewModel = response.MapTo<DerLayoutItemViewModel>();
                editViewModel.Types = _dropdownService.GetDerItemTypes().MapTo<SelectListItem>();
                editViewModel.Type = response.Type;
                switch (response.Type.ToLowerInvariant())
                {
                    case "line":
                        {
                            var lineChart = new LineChartViewModel();
                            editViewModel.LineChart = response.Artifact.MapPropertiesToInstance<LineChartViewModel>(lineChart);
                            var series = new LineChartViewModel.SeriesViewModel();
                            editViewModel.LineChart.Series.Insert(0, series);
                            editViewModel.Artifact.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                            {
                                Take = -1,
                                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                            }).Measurements
                            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                            break;
                        }

                    case "pie":
                        {
                            var pie = new PieViewModel();
                            editViewModel.Pie = response.Artifact.MapPropertiesToInstance<PieViewModel>(pie);
                            var series = new PieViewModel.SeriesViewModel();
                            editViewModel.Pie.Series.Insert(0, series);
                            editViewModel.Artifact.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                            {
                                Take = -1,
                                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                            }).Measurements
                            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                            break;
                        }

                    case "tank":
                        {
                            var tank = new TankViewModel();
                            editViewModel.Tank = response.Artifact.Tank.MapPropertiesToInstance<TankViewModel>(tank);
                            break;
                        }

                    case "multiaxis":
                        {
                            var multiaxisChart = new MultiaxisChartViewModel();
                            editViewModel.MultiaxisChart = response.Artifact.MapPropertiesToInstance<MultiaxisChartViewModel>(multiaxisChart);
                            multiaxisChart.GraphicTypes.Add(new SelectListItem { Value = "line", Text = "Line" });
                            multiaxisChart.ValueAxes.Add(new SelectListItem { Value = ValueAxis.KpiActual.ToString(), Text = "Kpi Actual" });
                            multiaxisChart.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                            {
                                Take = -1,
                                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                            }).Measurements
                  .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                            foreach (var chartRes in response.Artifact.Charts)
                            {
                                var chartViewModel = chartRes.MapTo<MultiaxisChartViewModel.ChartViewModel>();
                                switch (chartViewModel.GraphicType)
                                {
                                    case "line":
                                        {
                                            chartViewModel.LineChart = chartRes.MapTo<LineChartViewModel>();
                                            chartViewModel.LineChart.ValueAxes.Add(new SelectListItem { Value = ValueAxis.KpiActual.ToString(), Text = "Kpi Actual" });
                                            var series = new LineChartViewModel.SeriesViewModel();
                                            chartViewModel.LineChart.Series.Insert(0, series);
                                        }
                                        break;
                                }
                                multiaxisChart.Charts.Add(chartViewModel);
                            }
                            var chart = new MultiaxisChartViewModel.ChartViewModel();
                            editViewModel.MultiaxisChart.Charts.Insert(0, chart);

                            break;
                        }
                    case "avg-ytd-key-statistic":
                        {
                            break;
                        }
                    case "highlight":
                        {
                            var result = _selectService.GetHighlightTypesDropdown();
                            editViewModel.Highlights = result.Select(item => new SelectListItem() { Text = item.Text, Value = item.Value }).ToList();
                            editViewModel.HighlightId = response.Highlight.SelectOptionId;
                            break;
                        }
                }
                return View("EditLayoutItem", editViewModel);
            }
            else
            {
                viewModel.Types = _dropdownService.GetDerItemTypes().MapTo<SelectListItem>();
                return View("LayoutItem", viewModel);
            }
        }

        public ActionResult LayoutSetting(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "line":
                    {
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
                case "pie":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.Artifact = new DerLayoutItemViewModel.DerLayoutItemArtifactViewModel();
                        viewModel.Artifact.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                        {
                            Take = -1,
                            SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                        }).Measurements.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                        viewModel.Pie = new PieViewModel();
                        var series = new PieViewModel.SeriesViewModel();
                        viewModel.Pie.Series.Add(series);
                        return PartialView("LayoutType/_Pie", viewModel);
                    }
                case "tank":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.Artifact = new DerLayoutItemViewModel.DerLayoutItemArtifactViewModel();
                        viewModel.Tank = new TankViewModel();
                        return PartialView("LayoutType/_Tank", viewModel);
                    }

                case "highlight":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        var result = _selectService.GetHighlightTypesDropdown();
                        viewModel.Highlights =
                            result.Select(item => new SelectListItem() { Text = item.Text, Value = item.Value }).ToList();
                        //foreach(var item in result)
                        //{
                        //    var sl = new SelectListItem();
                        //    sl.Text = item.Text;
                        //    sl.Value = item.Value;
                        //    viewModel.Highlights.Add(sl);
                        //}
                        return PartialView("LayoutType/_Highlight", viewModel);
                    }
                case "alert":
                case "weather":
                case "wave":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.Type = type;
                        return PartialView("LayoutType/_StaticHighlight", viewModel);
                    }

                case "avg-ytd-key-statistic":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(6);
                        return PartialView("LayoutType/_AvgYtdKeyStatistic", viewModel);
                    }
                case "safety":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(9);
                        return PartialView("LayoutType/_SafetyTable", viewModel);
                    }
                case "security":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(6);
                        return PartialView("LayoutType/_Security", viewModel);
                    }
                case "lng-and-cds":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(12);
                        return PartialView("LayoutType/_LngAndCds", viewModel);
                    }
                case "dafwc":
                    {
                        //var viewModel = new DerLayoutItemViewModel();
                        return Content("You have chosen DAFWC and LOPC type");
                    }
                case "job-pmts":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(6);
                        return PartialView("LayoutType/_JobPmts", viewModel);
                    }
                case "total-feed-gas":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(4);
                        return PartialView("LayoutType/_TotalFeedGas", viewModel);
                    }
                case "table-tank":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(11);
                        return PartialView("LayoutType/_TableTank", viewModel);
                    }
                case "mgdp":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(6);
                        return PartialView("LayoutType/_MGDP", viewModel);
                    }
                case "hhv":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(4);
                        return PartialView("LayoutType/_HHV", viewModel);
                    }
                case "lng-and-cds-production":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(9);
                        return PartialView("LayoutType/_LngAndCdsProduction", viewModel);
                    }
            }

            return Content("Error");
        }

        public ActionResult Delete(int id, string type)
        {
            //var request = id;
            var response = _derService.DeleteLayoutItem(id, type);
            //{
            //    case "highlight":
            //        {
            //            request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
            //            response = _derService.DeleteLayoutItem(request);
            //            break;
            //        }
            //}
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Config", new { id = response.DerLayoutId });
        }

        [HttpPost]
        public ActionResult SaveLayoutItem(DerLayoutItemViewModel layoutItemViewModel)
        {
            var req = Request;
            var request = new SaveLayoutItemRequest();
            var response = new SaveLayoutItemResponse();
            switch (layoutItemViewModel.Type.ToLowerInvariant())
            {
                case "line":
                    {
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
                case "pie":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.Artifact = layoutItemViewModel.Artifact.MapTo<SaveLayoutItemRequest.LayoutItemArtifact>();
                        request.Artifact.Pie = layoutItemViewModel.Pie.MapTo<SaveLayoutItemRequest.LayoutItemArtifactPie>();
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }
                case "tank":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.Artifact = layoutItemViewModel.Artifact.MapTo<SaveLayoutItemRequest.LayoutItemArtifact>();
                        request.Artifact.Tank = layoutItemViewModel.Tank.MapTo<SaveLayoutItemRequest.LayoutItemArtifactTank>();
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }
                case "highlight":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.Highlight = new SaveLayoutItemRequest.LayoutItemHighlight();
                        request.Highlight.SelectOptionId = layoutItemViewModel.HighlightId;
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }
                case "alert":
                case "weather":
                case "wave":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.Type = layoutItemViewModel.Type;
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }
                /*case "avg-ytd-key-statistic":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.KpiInformations =
                            layoutItemViewModel.KpiInformations.MapTo<SaveLayoutItemRequest.DerKpiInformationRequest>();
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }*/
                case "safety":
                case "security":
                case "job-pmts":
                case "avg-ytd-key-statistic":
                case "lng-and-cds":
                case "total-feed-gas":
                case "table-tank":
                case "mgdp":
                case "hhv":
                case "lng-and-cds-production":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.KpiInformations = layoutItemViewModel.KpiInformations.MapTo<SaveLayoutItemRequest.DerKpiInformationRequest>();
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }
                /*case "lng-and-cds":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.KpiInformations =
                            layoutItemViewModel.KpiInformations.MapTo<SaveLayoutItemRequest.DerKpiInformationRequest>();
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }*/
                case "dafwc":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }
            }

            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;

            return RedirectToAction("Config", new { id = layoutItemViewModel.DerLayoutId });
        }

        private IList<DerLayoutItemViewModel.DerKpiInformationViewModel> GetKpiInformations(int numberOfKpi)
        {
            var list = new List<DerLayoutItemViewModel.DerKpiInformationViewModel>();
            for (int i = 0; i < numberOfKpi; i++)
            {
                list.Add(new DerLayoutItemViewModel.DerKpiInformationViewModel { Position = i });
            }

            return list;
        }

    }
}