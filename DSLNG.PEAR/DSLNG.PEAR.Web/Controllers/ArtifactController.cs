﻿using System.Globalization;
using System.Threading;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.Artifact;
using System;
using System.Linq;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.Artifact;
using System.Collections.Generic;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Requests.Kpi;
using PeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;
using DSLNG.PEAR.Services.Requests.Highlight;
using System.Data.SqlClient;
using NReco.ImageGenerator;
using DSLNG.PEAR.Services.Responses.Artifact;
using DSLNG.PEAR.Web.Attributes;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using DSLNG.PEAR.Services.Responses;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Reflection;
using OfficeOpenXml;
using System.Drawing;
using DSLNG.PEAR.Services.Responses.KpiAchievement;
using OfficeOpenXml.Style;

namespace DSLNG.PEAR.Web.Controllers
{
    [Authorize]
    public class ArtifactController : BaseController
    {
        private readonly IMeasurementService _measurementService;
        private readonly IKpiService _kpiService;
        private readonly IArtifactService _artifactServie;
        private readonly IHighlightService _highlightService;
        private readonly IKpiAchievementService _kpiAchievementService;
        private readonly IKpiTargetService _kpiTargetService;

        public ArtifactController(IMeasurementService measurementService,
            IKpiService kpiService,
            IArtifactService artifactServcie,
            IHighlightService highlightService, IKpiAchievementService kpiAchievementService,
            IKpiTargetService kpiTargetService)
        {
            _measurementService = measurementService;
            _kpiService = kpiService;
            _artifactServie = artifactServcie;
            _highlightService = highlightService;
            _kpiAchievementService = kpiAchievementService;
            _kpiTargetService = kpiTargetService;
        }

        [AuthorizeUser(AccessLevel = "AllowView")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridArtifactIndex");
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
            viewModel.Columns.Add("GraphicName");
            viewModel.Columns.Add("GraphicType");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridArtifactIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _artifactServie.GetArtifacts(new GetArtifactsRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _artifactServie.GetArtifacts(new GetArtifactsRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Artifacts;
        }

        public ActionResult KpiList(SearchKpiViewModel viewModel)
        {
            var kpis = _kpiService.GetKpiToSeries(viewModel.MapTo<GetKpiToSeriesRequest>()).KpiList;
            return Json(new { results = kpis }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Designer()
        {
            var viewModel = new ArtifactDesignerViewModel();
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "bar", Text = "Bar" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "barhorizontal", Text = "Bar Horizontal" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "baraccumulative", Text = "Bar Accumulative" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "barachievement", Text = "Bar Achievement" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "line", Text = "Line" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "area", Text = "Area" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "multiaxis", Text = "Multi Axis" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "combo", Text = "Combination" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "speedometer", Text = "Speedometer" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "trafficlight", Text = "Traffic Light" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "tabular", Text = "Tabular" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "tank", Text = "Tank" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "pie", Text = "Pie" });
            viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            }).Measurements
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            this.SetPeriodeTypes(viewModel.PeriodeTypes);
            this.SetRangeFilters(viewModel.RangeFilters);
            this.SetValueAxes(viewModel.ValueAxes);
            //this.SetKpiList(viewModel.KpiList);
            return View(viewModel);
        }

        [AuthorizeUser(AccessLevel = "AllowUpdate")]
        public ActionResult Edit(int id)
        {
            var artifact = _artifactServie.GetArtifact(new GetArtifactRequest { Id = id });

            var viewModel = new ArtifactDesignerViewModel();
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "bar", Text = "Bar" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "barhorizontal", Text = "Bar" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "baraccumulative", Text = "Bar Accumulative" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "barachievement", Text = "Bar Achievement" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "line", Text = "Line" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "area", Text = "Area" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "multiaxis", Text = "Multi Axis" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "combo", Text = "Combination" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "speedometer", Text = "Speedometer" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "tabular", Text = "Tabular" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "tank", Text = "Tank" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "trafficlight", Text = "Traffic Light" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "pie", Text = "Pie" });


            viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            }).Measurements
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            this.SetPeriodeTypes(viewModel.PeriodeTypes);
            this.SetRangeFilters(viewModel.RangeFilters);
            this.SetValueAxes(viewModel.ValueAxes);
            artifact.MapPropertiesToInstance<ArtifactDesignerViewModel>(viewModel);
            switch (viewModel.GraphicType)
            {
                case "speedometer":
                    {
                        var speedometerChart = new SpeedometerChartViewModel();
                        viewModel.SpeedometerChart = artifact.MapPropertiesToInstance<SpeedometerChartViewModel>(speedometerChart);
                        var plot = new SpeedometerChartViewModel.PlotBand();
                        viewModel.SpeedometerChart.PlotBands.Insert(0, plot);
                    }
                    break;
                case "line":
                    {
                        var lineChart = new LineChartViewModel();
                        viewModel.LineChart = artifact.MapPropertiesToInstance<LineChartViewModel>(lineChart);
                        this.SetValueAxes(viewModel.LineChart.ValueAxes);
                        var series = new LineChartViewModel.SeriesViewModel();
                        viewModel.LineChart.Series.Insert(0, series);
                    }
                    break;
                case "area":
                    {


                        var areaChart = new AreaChartViewModel();
                        areaChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        areaChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                        viewModel.AreaChart = artifact.MapPropertiesToInstance<AreaChartViewModel>(areaChart);
                        this.SetValueAxes(viewModel.AreaChart.ValueAxes);
                        var series = new AreaChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new AreaChartViewModel.StackViewModel());
                        viewModel.AreaChart.Series.Insert(0, series);
                    }
                    break;
                case "multiaxis":
                    {
                        var multiaxisChart = new MultiaxisChartViewModel();
                        viewModel.MultiaxisChart = artifact.MapPropertiesToInstance<MultiaxisChartViewModel>(multiaxisChart);
                        this.SetValueAxes(viewModel.MultiaxisChart.ValueAxes);
                        multiaxisChart.GraphicTypes.Add(new SelectListItem { Value = "bar", Text = "Bar" });
                        multiaxisChart.GraphicTypes.Add(new SelectListItem { Value = "baraccumulative", Text = "Bar Accumulative" });
                        multiaxisChart.GraphicTypes.Add(new SelectListItem { Value = "barachievement", Text = "Bar Achievement" });
                        multiaxisChart.GraphicTypes.Add(new SelectListItem { Value = "line", Text = "Line" });
                        multiaxisChart.GraphicTypes.Add(new SelectListItem { Value = "area", Text = "Area" });
                        multiaxisChart.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                        {
                            Take = -1,
                            SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                        }).Measurements
              .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                        foreach (var chartRes in artifact.Charts)
                        {
                            var chartViewModel = chartRes.MapTo<MultiaxisChartViewModel.ChartViewModel>();
                            switch (chartViewModel.GraphicType)
                            {
                                case "line":
                                    {
                                        chartViewModel.LineChart = chartRes.MapTo<LineChartViewModel>();
                                        this.SetValueAxes(chartViewModel.LineChart.ValueAxes);
                                        var series = new LineChartViewModel.SeriesViewModel();
                                        chartViewModel.LineChart.Series.Insert(0, series);
                                    }
                                    break;
                                case "area":
                                    {
                                        chartViewModel.AreaChart = chartRes.MapTo<AreaChartViewModel>();
                                        chartViewModel.AreaChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                                        chartViewModel.AreaChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });

                                        this.SetValueAxes(chartViewModel.AreaChart.ValueAxes);
                                        var series = new AreaChartViewModel.SeriesViewModel();
                                        series.Stacks.Add(new AreaChartViewModel.StackViewModel());
                                        chartViewModel.AreaChart.Series.Insert(0, series);
                                    }
                                    break;
                                default:
                                    {
                                        chartViewModel.BarChart = chartRes.MapTo<BarChartViewModel>();
                                        chartViewModel.BarChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                                        chartViewModel.BarChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                                        this.SetValueAxes(chartViewModel.BarChart.ValueAxes, false);

                                        var series = new BarChartViewModel.SeriesViewModel();
                                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                                        chartViewModel.BarChart.Series.Insert(0, series);
                                    }
                                    break;
                            }
                            multiaxisChart.Charts.Add(chartViewModel);
                        }
                        var chart = new MultiaxisChartViewModel.ChartViewModel();
                        viewModel.MultiaxisChart.Charts.Insert(0, chart);
                    }
                    break;
                case "combo":
                    {
                        var comboChart = new ComboChartViewModel();
                        viewModel.ComboChart = artifact.MapPropertiesToInstance<ComboChartViewModel>(comboChart);
                        this.SetValueAxes(viewModel.ComboChart.ValueAxes);
                        comboChart.GraphicTypes.Add(new SelectListItem { Value = "bar", Text = "Bar" });
                        comboChart.GraphicTypes.Add(new SelectListItem { Value = "baraccumulative", Text = "Bar Accumulative" });
                        comboChart.GraphicTypes.Add(new SelectListItem { Value = "barachievement", Text = "Bar Achievement" });
                        comboChart.GraphicTypes.Add(new SelectListItem { Value = "line", Text = "Line" });
                        comboChart.GraphicTypes.Add(new SelectListItem { Value = "area", Text = "Area" });
                        foreach (var chartRes in artifact.Charts)
                        {
                            var chartViewModel = chartRes.MapTo<ComboChartViewModel.ChartViewModel>();
                            switch (chartViewModel.GraphicType)
                            {
                                case "line":
                                    {
                                        chartViewModel.LineChart = chartRes.MapTo<LineChartViewModel>();
                                        this.SetValueAxes(chartViewModel.LineChart.ValueAxes);
                                        var series = new LineChartViewModel.SeriesViewModel();
                                        chartViewModel.LineChart.Series.Insert(0, series);
                                    }
                                    break;
                                case "area":
                                    {
                                        chartViewModel.AreaChart = chartRes.MapTo<AreaChartViewModel>();
                                        chartViewModel.AreaChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                                        chartViewModel.AreaChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });

                                        this.SetValueAxes(chartViewModel.AreaChart.ValueAxes);
                                        var series = new AreaChartViewModel.SeriesViewModel();
                                        series.Stacks.Add(new AreaChartViewModel.StackViewModel());
                                        chartViewModel.AreaChart.Series.Insert(0, series);
                                    }
                                    break;
                                default:
                                    {
                                        chartViewModel.BarChart = chartRes.MapTo<BarChartViewModel>();
                                        chartViewModel.BarChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                                        chartViewModel.BarChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                                        this.SetValueAxes(chartViewModel.BarChart.ValueAxes, false);

                                        var series = new BarChartViewModel.SeriesViewModel();
                                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                                        chartViewModel.BarChart.Series.Insert(0, series);
                                    }
                                    break;
                            }
                            comboChart.Charts.Add(chartViewModel);
                        }
                        var chart = new ComboChartViewModel.ChartViewModel();
                        viewModel.ComboChart.Charts.Insert(0, chart);
                    }
                    break;
                case "barachievement":
                    {
                        var barChart = new BarChartViewModel();
                        barChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        this.SetValueAxes(barChart.ValueAxes, false);
                        viewModel.BarChart = artifact.MapPropertiesToInstance<BarChartViewModel>(barChart);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.BarChart.Series.Insert(0, series);
                    }
                    break;
                case "baraccumulative":
                    {
                        var barChart = new BarChartViewModel();
                        barChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        this.SetValueAxes(barChart.ValueAxes, false);
                        viewModel.BarChart = artifact.MapPropertiesToInstance<BarChartViewModel>(barChart);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.BarChart.Series.Insert(0, series);
                    }
                    break;
                case "trafficlight":
                    {
                        var trafficLightChart = new TrafficLightChartViewModel();
                        viewModel.TrafficLightChart = artifact.MapPropertiesToInstance<TrafficLightChartViewModel>(trafficLightChart);
                        var plot = new TrafficLightChartViewModel.PlotBand();
                        viewModel.TrafficLightChart.PlotBands.Insert(0, plot);
                    }
                    break;
                case "tank":
                    {
                        viewModel.Tank = artifact.Tank.MapTo<TankViewModel>();
                    }
                    break;
                case "tabular":
                    {
                        viewModel.Tabular = artifact.MapPropertiesToInstance(new TabularViewModel());
                        viewModel.Tabular.Rows = new List<TabularViewModel.RowViewModel>();
                        viewModel.Tabular.Rows.Insert(0, new TabularViewModel.RowViewModel());
                        this.SetPeriodeTypes(viewModel.Tabular.PeriodeTypes);
                        this.SetRangeFilters(viewModel.Tabular.RangeFilters);

                        foreach (var row in artifact.Rows)
                        {
                            viewModel.Tabular.Rows.Add(new TabularViewModel.RowViewModel
                            {
                                KpiId = row.KpiId,
                                KpiName = row.KpiName,
                                PeriodeType = row.PeriodeType.ToString(),
                                EndInDisplay = ParseDateToString(row.PeriodeType, row.End),
                                StartInDisplay = ParseDateToString(row.PeriodeType, row.Start),
                                RangeFilter = row.RangeFilter.ToString()
                            });
                        }
                        /*foreach (var item in viewModel.Tabular.Rows)
                        {
                            if (item.Start.HasValue && item.End.HasValue)
                            {
                                item.StartInDisplay = ParseDateToString((PeriodeType)Enum.Parse(typeof(PeriodeType), item.PeriodeType), item.Start);
                                item.EndInDisplay = ParseDateToString((PeriodeType)Enum.Parse(typeof(PeriodeType), item.PeriodeType), item.End);    
                            }
                        }*/
                    }
                    break;
                case "pie":
                    {
                        viewModel.Pie = artifact.MapPropertiesToInstance(new PieViewModel());
                        this.SetValueAxes(viewModel.Pie.ValueAxes);
                        var series = new PieViewModel.SeriesViewModel();
                        /*viewModel.Is3D = artifact.Is3D;
                        viewModel.ShowLegend = artifact.ShowLegend;*/
                        viewModel.Pie.Series.Insert(0, series);
                    }
                    break;
                default:
                    {
                        var barChart = new BarChartViewModel();
                        barChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        barChart.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                        this.SetValueAxes(barChart.ValueAxes, false);

                        viewModel.BarChart = artifact.MapPropertiesToInstance<BarChartViewModel>(barChart);
                        //viewModel.BarChart.SeriesType = artifact.SeriesType;
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.BarChart.Series.Insert(0, series);
                    }
                    break;
            }

            viewModel.StartInDisplay = ParseDateToString(artifact.PeriodeType, artifact.Start);
            viewModel.EndInDisplay = ParseDateToString(artifact.PeriodeType, artifact.End);
            return View(viewModel);
        }

        public ActionResult GraphSettings()
        {
            switch (Request.QueryString["type"])
            {
                case "bar":
                case "barhorizontal":
                    {
                        var viewModel = new BarChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.BarChart = viewModel;
                        return PartialView("~/Views/BarChart/_Create.cshtml", artifactViewModel);
                    }
                case "baraccumulative":
                    {
                        var viewModel = new BarChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.BarChart = viewModel;
                        return PartialView("~/Views/BarChart/_Create.cshtml", artifactViewModel);
                    }
                case "barachievement":
                    {
                        var viewModel = new BarChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.BarChart = viewModel;
                        return PartialView("~/Views/BarChart/_Create.cshtml", artifactViewModel);
                    }
                case "line":
                    {
                        var viewModel = new LineChartViewModel();
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new LineChartViewModel.SeriesViewModel();
                        viewModel.Series.Add(series);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.LineChart = viewModel;
                        return PartialView("~/Views/LineChart/_Create.cshtml", artifactViewModel);
                    }
                case "area":
                    {
                        var viewModel = new AreaChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new AreaChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new AreaChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.AreaChart = viewModel;
                        return PartialView("~/Views/AreaChart/_Create.cshtml", artifactViewModel);
                    }
                case "multiaxis":
                    {
                        var viewModel = new MultiaxisChartViewModel();
                        var chart = new MultiaxisChartViewModel.ChartViewModel();
                        this.SetValueAxes(viewModel.ValueAxes);
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "bar", Text = "Bar" });
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "baraccumulative", Text = "Bar Accumulative" });
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "barachievement", Text = "Bar Achievement" });
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "line", Text = "Line" });
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "area", Text = "Area" });
                        viewModel.Charts.Add(chart);
                        viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                        {
                            Take = -1,
                            SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                        }).Measurements
              .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.MultiaxisChart = viewModel;
                        return PartialView("~/Views/MultiaxisChart/_Create.cshtml", artifactViewModel);
                    }
                case "combo":
                    {
                        var viewModel = new ComboChartViewModel();
                        var chart = new ComboChartViewModel.ChartViewModel();
                        this.SetValueAxes(viewModel.ValueAxes);
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "bar", Text = "Bar" });
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "baraccumulative", Text = "Bar Accumulative" });
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "barachievement", Text = "Bar Achievement" });
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "line", Text = "Line" });
                        viewModel.GraphicTypes.Add(new SelectListItem { Value = "area", Text = "Area" });
                        viewModel.Charts.Add(chart);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.ComboChart = viewModel;
                        return PartialView("~/Views/ComboChart/_Create.cshtml", artifactViewModel);
                    }
                case "speedometer":
                    {
                        var viewModel = new SpeedometerChartViewModel();
                        var plot = new SpeedometerChartViewModel.PlotBand();
                        viewModel.PlotBands.Add(plot);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.SpeedometerChart = viewModel;
                        return PartialView("~/Views/SpeedometerChart/_Create.cshtml", artifactViewModel);
                    }
                case "trafficlight":
                    {
                        var viewModel = new TrafficLightChartViewModel();
                        var plot = new TrafficLightChartViewModel.PlotBand();
                        viewModel.PlotBands.Add(plot);
                        var trafficLightViewModel = new ArtifactDesignerViewModel();
                        trafficLightViewModel.TrafficLightChart = viewModel;
                        return PartialView("~/Views/TrafficLightChart/_Create.cshtml", trafficLightViewModel);
                    }

                case "tabular":
                    {
                        var viewModel = new TabularViewModel();
                        var row = new TabularViewModel.RowViewModel();
                        this.SetPeriodeTypes(viewModel.PeriodeTypes);
                        this.SetRangeFilters(viewModel.RangeFilters);
                        viewModel.Rows.Add(row);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.Tabular = viewModel;
                        return PartialView("~/Views/Tabular/_Create.cshtml", artifactViewModel);
                    }
                case "tank":
                    {
                        var viewModel = new TankViewModel();
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.Tank = viewModel;
                        return PartialView("~/Views/Tank/_Create.cshtml", artifactViewModel);
                    }
                case "pie":
                    {
                        var viewModel = new PieViewModel();
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new PieViewModel.SeriesViewModel();
                        viewModel.Series.Add(series);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.Pie = viewModel;
                        return PartialView("~/Views/Pie/_Create.cshtml", artifactViewModel);
                    }
                default:
                    return PartialView("NotImplementedChart.cshtml");
            }
        }

        public ActionResult ComboSettings()
        {
            var artifactViewModel = new ArtifactDesignerViewModel();
            artifactViewModel.ComboChart = new ComboChartViewModel();
            var chart = new ComboChartViewModel.ChartViewModel();
            artifactViewModel.ComboChart.Charts.Add(chart);
            switch (Request.QueryString["type"])
            {
                case "bar":
                    {
                        var viewModel = new BarChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        artifactViewModel.ComboChart.Charts[0].BarChart = viewModel;
                        return PartialView("~/Views/ComboChart/_BarChartCreate.cshtml", artifactViewModel);
                    }
                case "baraccumulative":
                    {
                        var viewModel = new BarChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        artifactViewModel.ComboChart.Charts[0].BarChart = viewModel;
                        return PartialView("~/Views/ComboChart/_BarChartCreate.cshtml", artifactViewModel);
                    }
                case "barachievement":
                    {
                        var viewModel = new BarChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        artifactViewModel.ComboChart.Charts[0].BarChart = viewModel;
                        return PartialView("~/Views/ComboChart/_BarChartCreate.cshtml", artifactViewModel);
                    }
                case "line":
                    {
                        var viewModel = new LineChartViewModel();
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new LineChartViewModel.SeriesViewModel();
                        viewModel.Series.Add(series);
                        artifactViewModel.ComboChart.Charts[0].LineChart = viewModel;
                        return PartialView("~/Views/ComboChart/_LineChartCreate.cshtml", artifactViewModel);
                    }
                case "area":
                    {
                        var viewModel = new AreaChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new AreaChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new AreaChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        artifactViewModel.ComboChart.Charts[0].AreaChart = viewModel;
                        return PartialView("~/Views/ComboChart/_AreaChartCreate.cshtml", artifactViewModel);
                    }
                default:
                    return PartialView("NotImplementedChart.cshtml");
            }
        }

        public ActionResult MultiaxisSettings()
        {
            var artifactViewModel = new ArtifactDesignerViewModel();
            artifactViewModel.MultiaxisChart = new MultiaxisChartViewModel();
            var chart = new MultiaxisChartViewModel.ChartViewModel();
            artifactViewModel.MultiaxisChart.Charts.Add(chart);
            switch (Request.QueryString["type"])
            {
                case "bar":
                    {
                        var viewModel = new BarChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        artifactViewModel.MultiaxisChart.Charts[0].BarChart = viewModel;
                        //arti.BarChart = viewModel;
                        return PartialView("~/Views/MultiaxisChart/_BarChartCreate.cshtml", artifactViewModel);
                    }
                case "baraccumulative":
                    {
                        var viewModel = new BarChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        artifactViewModel.MultiaxisChart.Charts[0].BarChart = viewModel;
                        //arti.BarChart = viewModel;
                        return PartialView("~/Views/MultiaxisChart/_BarChartCreate.cshtml", artifactViewModel);
                    }
                case "barachievement":
                    {
                        var viewModel = new BarChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new BarChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new BarChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        artifactViewModel.MultiaxisChart.Charts[0].BarChart = viewModel;
                        //arti.BarChart = viewModel;
                        return PartialView("~/Views/MultiaxisChart/_BarChartCreate.cshtml", artifactViewModel);
                    }
                case "line":
                    {
                        var viewModel = new LineChartViewModel();
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new LineChartViewModel.SeriesViewModel();
                        viewModel.Series.Add(series);
                        artifactViewModel.MultiaxisChart.Charts[0].LineChart = viewModel;
                        //arti.BarChart = viewModel;
                        return PartialView("~/Views/MultiaxisChart/_LineChartCreate.cshtml", artifactViewModel);
                    }
                case "area":
                    {
                        var viewModel = new AreaChartViewModel();
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                        this.SetValueAxes(viewModel.ValueAxes, false);
                        var series = new AreaChartViewModel.SeriesViewModel();
                        series.Stacks.Add(new AreaChartViewModel.StackViewModel());
                        viewModel.Series.Add(series);
                        artifactViewModel.MultiaxisChart.Charts[0].AreaChart = viewModel;
                        //arti.BarChart = viewModel;
                        return PartialView("~/Views/MultiaxisChart/_AreaChartCreate.cshtml", artifactViewModel);
                    }
                default:
                    return PartialView("NotImplementedChart.cshtml");
            }
        }

        public void SetValueAxes(IList<SelectListItem> valueAxes, bool isCustomIncluded = true)
        {
            valueAxes.Add(new SelectListItem { Value = ValueAxis.KpiTarget.ToString(), Text = "Kpi Target" });
            valueAxes.Add(new SelectListItem { Value = ValueAxis.KpiActual.ToString(), Text = "Kpi Actual" });
            valueAxes.Add(new SelectListItem { Value = ValueAxis.KpiEconomic.ToString(), Text = "Kpi Economic" });
            if (isCustomIncluded)
            {
                valueAxes.Add(new SelectListItem { Value = ValueAxis.Custom.ToString(), Text = "Uniqe Each Series" });
            }
        }

        public void SetPeriodeTypes(IList<SelectListItem> periodeTypes)
        {
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    periodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
        }

        public void SetRangeFilters(IList<SelectListItem> rangeFilters)
        {
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentHour.ToString(), Text = "CURRENT HOUR" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentDay.ToString(), Text = "CURRENT DAY" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentWeek.ToString(), Text = "CURRENT WEEK" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentMonth.ToString(), Text = "CURRENT MONTH" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentYear.ToString(), Text = "CURRENT YEAR" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.DTD.ToString(), Text = "DAY TO DATE" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.MTD.ToString(), Text = "MONTH TO DATE" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.YTD.ToString(), Text = "YEAR TO DATE" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.Interval.ToString(), Text = "INTERVAL" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.SpecificDay.ToString(), Text = "SPECIFIC DAY" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.SpecificMonth.ToString(), Text = "SPECIFIC MONTH" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.SpecificYear.ToString(), Text = "SPECIFIC YEAR" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.AllExistingYears.ToString(), Text = "All Existing Years" });
        }

        [AuthorizeUser(AccessLevel = "AllowView")]
        public ActionResult View(int id)
        {
            var artifactResp = _artifactServie.GetArtifact(new GetArtifactRequest { Id = id });
            var previewViewModel = new ArtifactPreviewViewModel();
            previewViewModel.Id = id;
            previewViewModel.FractionScale = artifactResp.FractionScale;
            previewViewModel.MaxFractionScale = artifactResp.MaxFractionScale;
            previewViewModel.AsNetbackChart = artifactResp.AsNetbackChart;
            return GetView(previewViewModel, artifactResp);
        }

        [HttpPost]
        public ActionResult Preview(ArtifactDesignerViewModel viewModel)
        {
            var previewViewModel = new ArtifactPreviewViewModel();
            previewViewModel.FractionScale = viewModel.FractionScale;
            previewViewModel.MaxFractionScale = viewModel.MaxFractionScale;
            switch (viewModel.GraphicType)
            {
                case "line":
                    {
                        var cartesianRequest = viewModel.MapTo<GetCartesianChartDataRequest>();
                        viewModel.LineChart.MapPropertiesToInstance<GetCartesianChartDataRequest>(cartesianRequest);
                        var chartData = _artifactServie.GetChartData(cartesianRequest);
                        var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                        {
                            TimePeriodes = chartData.TimePeriodes,
                            Type = "Overall",
                            PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType)
                        });
                        previewViewModel.PeriodeType = viewModel.PeriodeType;
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.LineChart = new LineChartDataViewModel();
                        previewViewModel.LineChart.Title = viewModel.HeaderTitle;
                        previewViewModel.LineChart.Subtitle = chartData.Subtitle;
                        previewViewModel.LineChart.ValueAxisTitle = _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
                        previewViewModel.LineChart.Series = chartData.Series.MapTo<LineChartDataViewModel.SeriesViewModel>();
                        previewViewModel.LineChart.Periodes = chartData.Periodes;
                    }
                    break;
                case "area":
                    {
                        var cartesianRequest = viewModel.MapTo<GetCartesianChartDataRequest>();
                        viewModel.AreaChart.MapPropertiesToInstance<GetCartesianChartDataRequest>(cartesianRequest);
                        var chartData = _artifactServie.GetChartData(cartesianRequest);
                        var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                        {
                            TimePeriodes = chartData.TimePeriodes,
                            Type = "Overall",
                            PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType)
                        });
                        previewViewModel.PeriodeType = viewModel.PeriodeType;
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.AreaChart = new AreaChartDataViewModel();
                        previewViewModel.AreaChart.Title = viewModel.HeaderTitle;
                        previewViewModel.AreaChart.Subtitle = chartData.Subtitle;
                        previewViewModel.AreaChart.ValueAxisTitle = _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
                        previewViewModel.AreaChart.Series = chartData.Series.MapTo<AreaChartDataViewModel.SeriesViewModel>();
                        previewViewModel.AreaChart.Periodes = chartData.Periodes;
                        previewViewModel.AreaChart.SeriesType = chartData.SeriesType;
                    }
                    break;
                case "speedometer":
                    {
                        var request = viewModel.MapTo<GetSpeedometerChartDataRequest>();
                        viewModel.SpeedometerChart.MapPropertiesToInstance<GetSpeedometerChartDataRequest>(request);
                        var chartData = _artifactServie.GetSpeedometerChartData(request);
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.SpeedometerChart = new SpeedometerChartDataViewModel();
                        previewViewModel.SpeedometerChart.Title = viewModel.HeaderTitle;
                        previewViewModel.SpeedometerChart.Subtitle = chartData.Subtitle;
                        previewViewModel.SpeedometerChart.ValueAxisTitle = _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
                        previewViewModel.SpeedometerChart.Series = chartData.Series.MapTo<SpeedometerChartDataViewModel.SeriesViewModel>();
                        previewViewModel.SpeedometerChart.PlotBands = chartData.PlotBands.MapTo<SpeedometerChartDataViewModel.PlotBandViewModel>();
                    }
                    break;
                case "trafficlight":
                    {
                        var request = viewModel.MapTo<GetTrafficLightChartDataRequest>();
                        viewModel.TrafficLightChart.MapPropertiesToInstance<GetTrafficLightChartDataRequest>(request);
                        var chartData = _artifactServie.GetTrafficLightChartData(request);
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.TrafficLightChart = new TrafficLightChartDataViewModel();
                        previewViewModel.TrafficLightChart.Title = viewModel.HeaderTitle;
                        previewViewModel.TrafficLightChart.Subtitle = chartData.Subtitle;
                        previewViewModel.TrafficLightChart.ValueAxisTitle =
                            _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId })
                                               .Name;
                        previewViewModel.TrafficLightChart.Series =
                            chartData.Series.MapTo<TrafficLightChartDataViewModel.SeriesViewModel>();
                        previewViewModel.TrafficLightChart.PlotBands =
                            chartData.PlotBands.MapTo<TrafficLightChartDataViewModel.PlotBandViewModel>();
                    }
                    break;
                case "tabular":
                    {
                        var request = viewModel.MapTo<GetTabularDataRequest>();
                        /*request.Rows = new List<GetTabularDataRequest.RowRequest>();
                        foreach (var rowViewModel in viewModel.Tabular.Rows)
                        {
                            request.Rows.Add(new GetTabularDataRequest.RowRequest
                                {
                                    End = rowViewModel.EndAfterParsed,
                                    KpiId = rowViewModel.KpiId,
                                    PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), rowViewModel.PeriodeType),
                                    KpiName = rowViewModel.KpiName,
                                    RangeFilter = (RangeFilter)Enum.Parse(typeof(RangeFilter), rowViewModel.RangeFilter),
                                    Start = rowViewModel.StartAfterParsed
                                });
                        }*/

                        viewModel.Tabular.MapPropertiesToInstance<GetTabularDataRequest>(request);

                        var chartData = _artifactServie.GetTabularData(request);
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.Tabular = new TabularDataViewModel();
                        chartData.MapPropertiesToInstance<TabularDataViewModel>(previewViewModel.Tabular);
                        previewViewModel.Tabular.Title = viewModel.HeaderTitle;
                    }
                    break;
                case "tank":
                    {
                        var request = viewModel.MapTo<GetTankDataRequest>();
                        //viewModel.Tank.MapPropertiesToInstance<GetTankDataRequest>(request);
                        var chartData = _artifactServie.GetTankData(request);
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.Tank = new TankDataViewModel();
                        chartData.MapPropertiesToInstance<TankDataViewModel>(previewViewModel.Tank);
                        previewViewModel.Tank.Title = viewModel.HeaderTitle;
                        previewViewModel.Tank.Subtitle = chartData.Subtitle;
                    }
                    break;
                case "multiaxis":
                    {
                        var request = viewModel.MapTo<GetMultiaxisChartDataRequest>();
                        viewModel.MultiaxisChart.MapPropertiesToInstance<GetMultiaxisChartDataRequest>(request);
                        var chartData = _artifactServie.GetMultiaxisChartData(request);
                        var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                        {
                            TimePeriodes = chartData.TimePeriodes,
                            Type = "Overall",
                            PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType)
                        });
                        previewViewModel.PeriodeType = viewModel.PeriodeType;
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.MultiaxisChart = new MultiaxisChartDataViewModel();
                        chartData.MapPropertiesToInstance<MultiaxisChartDataViewModel>(previewViewModel.MultiaxisChart);
                        previewViewModel.MultiaxisChart.Title = viewModel.HeaderTitle;

                    }
                    break;
                case "combo":
                    {
                        var request = viewModel.MapTo<GetComboChartDataRequest>();
                        viewModel.ComboChart.MapPropertiesToInstance<GetComboChartDataRequest>(request);
                        var chartData = _artifactServie.GetComboChartData(request);
                        var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                        {
                            TimePeriodes = chartData.TimePeriodes,
                            Type = "Overall",
                            PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType)
                        });
                        previewViewModel.PeriodeType = viewModel.PeriodeType;
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.ComboChart = new ComboChartDataViewModel();
                        chartData.MapPropertiesToInstance<ComboChartDataViewModel>(previewViewModel.ComboChart);
                        previewViewModel.ComboChart.Title = viewModel.HeaderTitle;

                    }
                    break;
                case "pie":
                    {
                        var request = viewModel.MapTo<GetPieDataRequest>();
                        viewModel.Pie.MapPropertiesToInstance<GetPieDataRequest>(request);
                        var pieData = _artifactServie.GetPieData(request);
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.Pie = pieData.MapTo<PieDataViewModel>();
                        previewViewModel.Pie.Is3D = request.Is3D;
                        previewViewModel.Pie.ShowLegend = request.ShowLegend;
                    }
                    break;

                default:
                    {
                        var cartesianRequest = viewModel.MapTo<GetCartesianChartDataRequest>();
                        viewModel.BarChart.MapPropertiesToInstance<GetCartesianChartDataRequest>(cartesianRequest);
                        var chartData = _artifactServie.GetChartData(cartesianRequest);
                        if (!viewModel.AsNetbackChart)
                        {
                            var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                            {
                                TimePeriodes = chartData.TimePeriodes,
                                Type = "Overall",
                                PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType)
                            });
                            previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        }
                        previewViewModel.AsNetbackChart = viewModel.AsNetbackChart;
                        previewViewModel.PeriodeType = viewModel.PeriodeType;
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.BarChart = new BarChartDataViewModel();
                        previewViewModel.BarChart.Title = viewModel.HeaderTitle;
                        previewViewModel.BarChart.Subtitle = chartData.Subtitle;
                        previewViewModel.BarChart.ValueAxisTitle = _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
                        previewViewModel.BarChart.Series = chartData.Series.MapTo<BarChartDataViewModel.SeriesViewModel>();
                        previewViewModel.BarChart.Periodes = chartData.Periodes;
                        previewViewModel.BarChart.SeriesType = chartData.SeriesType;
                    }
                    break;
            }
            return Json(previewViewModel);
        }

        [HttpPost]
        public ActionResult Create(ArtifactDesignerViewModel viewModel)
        {
            switch (viewModel.GraphicType)
            {
                case "line":
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Create";
                        viewModel.LineChart.MapPropertiesToInstance<CreateArtifactRequest>(request);
                        _artifactServie.Create(request);
                    }
                    break;

                case "area":
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Create";
                        viewModel.AreaChart.MapPropertiesToInstance<CreateArtifactRequest>(request);
                        _artifactServie.Create(request);
                    }
                    break;
                case "multiaxis":
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Create";
                        viewModel.MultiaxisChart.MapPropertiesToInstance<CreateArtifactRequest>(request);
                        _artifactServie.Create(request);
                    }
                    break;
                case "combo":
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Create";
                        viewModel.ComboChart.MapPropertiesToInstance<CreateArtifactRequest>(request);
                        _artifactServie.Create(request);
                    }
                    break;
                case "speedometer":
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Create";
                        viewModel.SpeedometerChart.MapPropertiesToInstance<CreateArtifactRequest>(request);
                        _artifactServie.Create(request);
                    }
                    break;

                case "trafficlight":
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Create";
                        viewModel.TrafficLightChart.MapPropertiesToInstance<CreateArtifactRequest>(request);
                        _artifactServie.Create(request);
                    }
                    break;
                case "tabular":
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Create";
                        viewModel.Tabular.MapPropertiesToInstance<CreateArtifactRequest>(request);
                        _artifactServie.Create(request);
                    }
                    break;
                case "tank":
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        //viewModel.Tank.MapPropertiesToInstance<CreateArtifactRequest>(request);
                        _artifactServie.Create(request);
                    }
                    break;
                case "pie":
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Create";
                        request.Series = viewModel.Pie.Series.MapTo<CreateArtifactRequest.SeriesRequest>();
                        _artifactServie.Create(request);
                    }
                    break;
                default:
                    {
                        var request = viewModel.MapTo<CreateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Create";
                        viewModel.BarChart.MapPropertiesToInstance<CreateArtifactRequest>(request);
                        _artifactServie.Create(request);
                    }
                    break;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(ArtifactDesignerViewModel viewModel)
        {
            switch (viewModel.GraphicType)
            {
                case "line":
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.LineChart.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        _artifactServie.Update(request);
                    }
                    break;

                case "area":
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.AreaChart.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        _artifactServie.Update(request);
                    }
                    break;
                case "multiaxis":
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.MultiaxisChart.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        _artifactServie.Update(request);
                    }
                    break;
                case "combo":
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.ComboChart.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        _artifactServie.Update(request);
                    }
                    break;
                case "speedometer":
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.SpeedometerChart.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        _artifactServie.Update(request);
                    }
                    break;
                case "trafficlight":
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.TrafficLightChart.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        _artifactServie.Update(request);
                    }
                    break;
                case "tank":
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.Tank.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        request.Id = viewModel.Id;
                        _artifactServie.Update(request);
                    }
                    break;
                case "tabular":
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.Tabular.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        _artifactServie.Update(request);
                    }
                    break;
                case "pie":
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.Pie.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        _artifactServie.Update(request);
                    }
                    break;
                default:
                    {
                        var request = viewModel.MapTo<UpdateArtifactRequest>();
                        request.UserId = this.UserProfile().UserId;
                        request.ControllerName = "Artifact";
                        request.ActionName = "Edit";
                        viewModel.BarChart.MapPropertiesToInstance<UpdateArtifactRequest>(request);
                        _artifactServie.Update(request);
                    }
                    break;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var templates = _artifactServie.GetArtifacts(new GetArtifactsRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search
            });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalRecords = templates.Artifacts.Count,
                iTotalDisplayRecords = templates.TotalRecords,
                aaData = templates.Artifacts
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Delete(int id)
        {
            var request = new DeleteArtifactRequest();
            request.Id = id;
            request.UserId = this.UserProfile().UserId;
            request.ControllerName = "Artifact";
            request.ActionName = "Delete";
            var resp = _artifactServie.Delete(request);
            TempData["IsSuccess"] = resp.IsSuccess;
            TempData["Message"] = resp.Message;
            return RedirectToAction("Index");
        }

        public ActionResult Print(int id)
        {
            var secretNumber = Guid.NewGuid().ToString();
            ArtifactCloneController.SecretNumber = secretNumber;
            var displayUrl = Url.Action("Display", "ArtifactClone", new { id = id, secretNumber = secretNumber }, this.Request.Url.Scheme);
            var htmlToImageConverter = new HtmlToImageConverter();
            htmlToImageConverter.Height = 350;
            htmlToImageConverter.Width = 500;
            return File(htmlToImageConverter.GenerateImageFromFile(displayUrl, ImageFormat.Png), "image/png", "TheGraph.png");
        }

        public ActionResult GraphicSetting(int id)
        {
            var artifact = _artifactServie.GetArtifact(new GetArtifactRequest { Id = id });
            var viewModel = new ArtifactDesignerViewModel();
            SetPeriodeTypes(viewModel.PeriodeTypes);
            SetRangeFilters(viewModel.RangeFilters);
            SetValueAxes(viewModel.ValueAxes);
            artifact.MapPropertiesToInstance<ArtifactDesignerViewModel>(viewModel);

            viewModel.StartInDisplay = ParseDateToString(artifact.PeriodeType, artifact.Start);
            viewModel.EndInDisplay = ParseDateToString(artifact.PeriodeType, artifact.End);
            return PartialView("_GraphicSetting", viewModel);
        }

        public ActionResult HighlightSetting(int id)
        {
            var highlight = _highlightService.GetHighlight(new GetHighlightRequest { Id = id });
            var viewModel = new ArtifactDesignerViewModel();
            SetPeriodeTypes(viewModel.PeriodeTypes);
            SetRangeFilters(viewModel.RangeFilters);
            SetValueAxes(viewModel.ValueAxes);
            highlight.MapPropertiesToInstance<ArtifactDesignerViewModel>(viewModel);
            viewModel.GraphicType = "highlight";
            viewModel.StartInDisplay = ParseDateToString(highlight.PeriodeType, highlight.Date);
            viewModel.EndInDisplay = ParseDateToString(highlight.PeriodeType, highlight.Date);
            viewModel.HighlightTypeId = highlight.TypeId;
            viewModel.HeaderTitle = highlight.Type;
            return PartialView("_GraphicSetting", viewModel);
        }

        [HttpPost]
        public ActionResult GraphicSetting(ArtifactDesignerViewModel viewModel)
        {
            var artifactResp = _artifactServie.GetArtifact(new GetArtifactRequest { Id = viewModel.Id });
            var previewViewModel = new ArtifactPreviewViewModel();
            previewViewModel.FractionScale = artifactResp.FractionScale;
            previewViewModel.MaxFractionScale = artifactResp.MaxFractionScale;
            previewViewModel.AsNetbackChart = artifactResp.AsNetbackChart;

            artifactResp.PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType);
            artifactResp.RangeFilter = (RangeFilter)Enum.Parse(typeof(RangeFilter), viewModel.RangeFilter);
            artifactResp.Start = viewModel.StartAfterParsed;
            artifactResp.End = viewModel.EndAfterParsed;

            return GetView(previewViewModel, artifactResp);
        }

        [HttpPost]
        public ActionResult HighlightSetting(ArtifactDesignerViewModel viewModel)
        {
            var highlight = _highlightService.GetHighlightByPeriode(new GetHighlightRequest
            {
                Id = 0,
                PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType),
                Date = viewModel.StartAfterParsed,
                HighlightTypeId = viewModel.HighlightTypeId
            });

            highlight.PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType);
            highlight.Date = viewModel.StartAfterParsed.Value;
            return Json(highlight, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportSetting(int id)
        {
            var artifact = _artifactServie.GetArtifact(new GetArtifactRequest { Id = id });
            var kpis = new List<SelectListItem>();
            switch (artifact.GraphicType.ToLowerInvariant())
            {
                case "multiaxis":
                    foreach (var chart in artifact.Charts)
                    {
                        foreach (var kpi in GetKpisAsSelectListItem(chart))
                        {
                            kpis.Add(new SelectListItem { Value = kpi.Value, Text = kpi.Text });
                        }
                    }
                    break;
                case "baraccumulative":
                    foreach (var kpi in GetKpisAsSelectListItem(artifact))
                    {
                        kpis.Add(new SelectListItem { Value = kpi.Value, Text = kpi.Text });
                    }
                    break;
            }
            var viewModel = new ExportSettingViewModel();
            viewModel.Kpis = kpis;
            viewModel.PeriodeType = artifact.PeriodeType.ToString();
            viewModel.RangeFilter = artifact.RangeFilter.ToString();
            viewModel.GraphicType = artifact.GraphicType;

            var currentYear = DateTime.Now.Date.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDay = DateTime.Now.Day;
            switch (artifact.RangeFilter)
            {

                case RangeFilter.MTD:
                    viewModel.StartInDisplay = ParseDateToString(artifact.PeriodeType, new DateTime(currentYear, currentMonth, 1));
                    viewModel.StartInDisplay = ParseDateToString(artifact.PeriodeType, new DateTime(currentYear, currentMonth, currentDay));
                    break;
                case RangeFilter.YTD:
                    viewModel.StartInDisplay = ParseDateToString(artifact.PeriodeType, new DateTime(currentYear, 1, 1));
                    viewModel.EndInDisplay = ParseDateToString(artifact.PeriodeType, new DateTime(currentYear, currentMonth, 1));
                    break;
                default:
                    viewModel.StartInDisplay = ParseDateToString(artifact.PeriodeType, artifact.Start);
                    viewModel.EndInDisplay = ParseDateToString(artifact.PeriodeType, artifact.End);
                    break;
            }
            //SetPeriodeTypes(viewModel.PeriodeTypes);
            //SetRangeFilters(viewModel.RangeFilters);
            //SetValueAxes(viewModel.ValueAxes);
            //artifact.MapPropertiesToInstance<ArtifactDesignerViewModel>(viewModel);

            return PartialView("_ExportSetting", viewModel);
        }

        [HttpPost]
        public ActionResult ExportSetting(ExportSettingViewModel viewModel)
        {
            var labelDictionaries = new Dictionary<string, List<string>>();
            foreach (var item in viewModel.KpiIds)
            {
                var split = item.Split('|');
                if (split.Length > 2)
                {
                    var kpiIndex = split[0] + "-" + split[1];
                    if (!labelDictionaries.Keys.Contains(kpiIndex))
                    {
                        labelDictionaries.Add(kpiIndex, new List<string> { split[0], split[1], split[2] });
                    }
                    else
                    {
                        labelDictionaries[kpiIndex] = new List<string> { split[0], split[1], split[2] };
                    }
                }
            }
            var kpiActuals = labelDictionaries.Where(x => x.Value.ElementAtOrDefault(1) == "KpiActual").Select(x => Int32.Parse(x.Value.ElementAt(0))).ToArray();
            var kpiTargets = labelDictionaries.Where(x => x.Value.ElementAtOrDefault(1) == "KpiTarget").Select(x => Int32.Parse(x.Value.ElementAt(0))).ToArray();
            var dataActuals = _kpiAchievementService.GetKpiAchievements(kpiActuals, viewModel.StartAfterParsed, viewModel.EndAfterParsed, viewModel.PeriodeType);
            var dataTargets = _kpiTargetService.GetKpiTargets(kpiTargets, viewModel.StartAfterParsed, viewModel.EndAfterParsed, viewModel.PeriodeType);

            //var existedTargetKpis = dataTargets.KpiTargets.GroupBy(x => x.KpiId).Select(g => g.First()).ToList();

            Dictionary<DateTime, IList<ExportSettingData>> dictionaries =
                new Dictionary<DateTime, IList<ExportSettingData>>();
            IList<ExportSettingData> exportData = new List<ExportSettingData>();

            foreach (var x in dataActuals.KpiAchievements)
            {
                exportData.Add(new ExportSettingData { Periode = x.Periode, KpiId = x.Kpi.Id, KpiName = x.Kpi.Name, MeasurementName = x.Kpi.KpiMeasurement, Value = x.Value, ValueAxes = ValueAxis.KpiActual.ToString() });
            }

            foreach (var x in dataTargets.KpiTargets)
            {
                exportData.Add(new ExportSettingData { Periode = x.Periode, KpiId = x.KpiId, KpiName = x.KpiName, MeasurementName = x.MeasurementName, Value = x.Value, ValueAxes = ValueAxis.KpiTarget.ToString() });
            }

            var periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType);
            var rangeFilter = (RangeFilter)Enum.Parse(typeof(RangeFilter), viewModel.RangeFilter);
            IList<DateTime> dateTimePeriodes = new List<DateTime>();
            string timeInformation;
            _artifactServie.GetPeriodes(periodeType, rangeFilter, viewModel.StartAfterParsed, viewModel.EndAfterParsed, out dateTimePeriodes, out timeInformation);

            var existedKpis = exportData.GroupBy(x => x.KpiId).Select(g => g.First()).ToList();
            existedKpis = ModifyKpis(existedKpis, viewModel.GraphicType);
            IDictionary<DateTime, IDictionary<string, ExportSettingData>> d = new Dictionary<DateTime, IDictionary<string, ExportSettingData>>();
            IList<DateTime> existedPeriodes = new List<DateTime>();
            foreach (var periode in dateTimePeriodes)
            {
                var data = new Dictionary<string, ExportSettingData>();
                foreach (var existedKpi in existedKpis)
                {
                    if (existedKpi.KpiName == "Previous Accumulation")
                    {
                        var item = new ExportSettingData { KpiId = existedKpi.KpiId, KpiName = existedKpi.KpiName, KpiReferenceId = existedKpi.KpiReferenceId, MeasurementName = existedKpi.MeasurementName, Periode = periode, ValueAxes = existedKpi.ValueAxes };
                        item.Value = existedPeriodes.Count == 0 ? 0 : exportData.Where(x => existedPeriodes.Contains(x.Periode) && x.KpiId == existedKpi.KpiReferenceId).Sum(x => x.Value.Value);
                        data.Add(existedKpi.KpiId.ToString(), item);
                    }
                    else if (existedKpi.KpiName == "Total")
                    {
                        var item = new ExportSettingData { KpiId = existedKpi.KpiId, KpiName = existedKpi.KpiName, KpiReferenceId = existedKpi.KpiReferenceId, MeasurementName = existedKpi.MeasurementName, Periode = periode, ValueAxes = existedKpi.ValueAxes };
                        //var previousValue = existedPeriodes.Count == 0 ? 0 : exportData.Where(x => existedPeriodes.Contains(x.Periode) && x.KpiId == existedKpi.KpiReferenceId).Sum(x => x.Value.Value);
                        if(data[(existedKpi.KpiId - 1).ToString()].Value.HasValue && data[existedKpi.KpiReferenceId.ToString()].Value.HasValue)
                        {
                            item.Value = (data[(existedKpi.KpiId - 1).ToString()].Value) +
                                 (data[existedKpi.KpiReferenceId.ToString()].Value);
                        }
                        
                        data.Add(existedKpi.KpiId.ToString(), item);
                    }
                    else
                    {
                        var val = exportData.Where(x => x.Periode == periode && x.KpiId == existedKpi.KpiId).FirstOrDefault();
                        if (val == null)
                        {
                            var item = new ExportSettingData { KpiId = existedKpi.KpiId, KpiName = existedKpi.KpiName, KpiReferenceId = existedKpi.KpiId, MeasurementName = existedKpi.MeasurementName, Periode = periode, ValueAxes = existedKpi.ValueAxes };
                            data.Add(existedKpi.KpiId.ToString(), item);
                        }
                        else
                        {
                            data.Add(existedKpi.KpiId.ToString(), val);
                        }
                    }
                }
                d.Add(periode, data);
                existedPeriodes.Add(periode);
            }

            //foreach(var periode in dateTimePeriodes)
            //{
            //    var tempData = new List<ExportSettingData>();
            //    tempData = exportData.Where(x => x.Periode == periode).OrderBy(x => x.KpiId).ToList();
            //    foreach (var existedKpi in existedKpis)
            //    {
            //        if (tempData.Where(x => x.KpiId == existedKpi.KpiId && x.KpiName != "Previous Accumulation").FirstOrDefault() == null)
            //        {
            //            tempData.Add(new ExportSettingData { Periode = periode, KpiId = existedKpi.KpiId, KpiName = existedKpi.KpiName, MeasurementName = existedKpi.MeasurementName, Value = null, ValueAxes = existedKpi.ValueAxes });
            //        } 
            //        else if (tempData.Where(x => x.KpiId == existedKpi.KpiId && x.KpiName == "Previous Accumulation").FirstOrDefault() == null)
            //        {
            //            tempData.Add(new ExportSettingData { Periode = periode, KpiId = existedKpi.KpiId, KpiName = existedKpi.KpiName, MeasurementName = existedKpi.MeasurementName, Value = null, ValueAxes = existedKpi.ValueAxes });
            //        }
            //    }

            //    if (!dictionaries.Keys.Contains(periode))
            //    {
            //        dictionaries.Add(periode, tempData);
            //    }
            //    else
            //    {
            //        dictionaries[periode] = tempData;
            //    }
            //}

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = ": " + DateTime.Now.ToString("dd/MMMM/yyyy");
            ws.Cells["C3"].Value = "By: " + UserProfile().Name;

            ws.Cells["A4"].Value = "Dashboard Name";
            ws.Cells["B4"].Value = ": " + viewModel.Name;

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy}", DateTimeOffset.Now);

            ws.Cells["A6:A7"].Value = "Periode";
            ws.Cells["A6:A7"].Merge = true;
            ws.Cells["A6:A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells["A6:A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells["A6:A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            ws.Cells["A6:A7"].Style.Font.Bold = true;

            int kpiRowStart = 8;
            int kpiColStart = 2;
            IDictionary<int, string> kpiDictionaries = new Dictionary<int, string>();

            foreach (var dictionary in d)
            {
                foreach (var kpi in dictionary.Value)
                {
                    ws.Cells[6, kpiColStart].Value = string.Format("{0}, {1} ({2})", kpi.Value.KpiId, kpi.Value.KpiName, kpi.Value.MeasurementName);
                    ws.Cells[7, kpiColStart].Value = string.Format("{0})", kpi.Value.KpiName);
                    kpiColStart++;
                }
                break;
            }

            kpiColStart = 1;
            foreach (var dictionary in d)
            {
                ws.Cells[kpiRowStart, kpiColStart].Value = FormatDate(dictionary.Key, viewModel.PeriodeType);
                foreach (var val in dictionary.Value)
                {
                    kpiColStart++;
                    ws.Cells[kpiRowStart, kpiColStart].Value = val.Value.Value.ToString();
                    ws.Cells[kpiRowStart, kpiColStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[kpiRowStart, kpiColStart].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
                kpiRowStart++;
                kpiColStart = 1;
            }

            //foreach (var dictionary in dictionaries)
            //{
            //    foreach (var val in dictionary.Value)
            //    {
            //        ws.Cells[6, kpiColStart].Value = string.Format("{0}, {1} ({2})", val.KpiId, val.KpiName, val.MeasurementName);
            //        ws.Cells[6, kpiColStart].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            //        ws.Cells[6, kpiColStart].Style.Font.Bold = true;
            //        ws.Cells[6, kpiColStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //        ws.Cells[7, kpiColStart].Value = labelDictionaries.Single(x => x.Key == val.KpiId + "-" + val.ValueAxes).Value[1];
            //        ws.Cells[7, kpiColStart].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            //        ws.Cells[7, kpiColStart].Style.Font.Bold = true;
            //        ws.Cells[7, kpiColStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        kpiColStart++;
            //    }
            //    break;
            //}
            //kpiColStart = 1;
            //foreach (var dictionary in dictionaries)
            //{
            //    ws.Cells[kpiRowStart, kpiColStart].Value = FormatDate(dictionary.Key, viewModel.PeriodeType);
            //    ws.Cells[kpiRowStart, kpiColStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //    ws.Cells[kpiRowStart, kpiColStart].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            //    foreach (var val in dictionary.Value)
            //    {
            //        kpiColStart++;
            //        ws.Cells[kpiRowStart, kpiColStart].Value = val.Value.ToString();
            //        ws.Cells[kpiRowStart, kpiColStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        ws.Cells[kpiRowStart, kpiColStart].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            //    }
            //    kpiRowStart++;
            //    kpiColStart = 1;
            //}



            ws.Cells["A:AZ"].AutoFitColumns();

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + viewModel.FileName + ".xls");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();


            var result = new BaseResponse { IsSuccess = true };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<ExportSettingData> ModifyKpis(List<ExportSettingData> existedKpis, string graphicType)
        {
            var modifiedKpis = new List<ExportSettingData>();
            switch (graphicType.ToLowerInvariant())
            {
                case "baraccumulative":
                    foreach (var kpi in existedKpis)
                    {
                        modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId, KpiReferenceId = kpi.KpiId, KpiName = "Previous Accumulation", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });
                        modifiedKpis.Add(kpi);
                        modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId + 1, KpiReferenceId = kpi.KpiId, KpiName = "Total", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });
                    }
                    break;
                default:
                    modifiedKpis = existedKpis;
                    break;
            }

            return modifiedKpis;
        }

        private string FormatDate(DateTime dateTime, string periodeType)
        {
            PeriodeType pType = (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);
            switch (pType)
            {
                case PeriodeType.Yearly:
                    return dateTime.ToString("yyyy");
                case PeriodeType.Monthly:
                    return dateTime.ToString("MMMM-yyyy");
                default:
                    return dateTime.ToString("dd-MMMM-yyyy");
            }
        }

        private IList<SelectListItem> GetKpisAsSelectListItem(GetArtifactResponse.ChartResponse chart)
        {
            var kpis = new List<SelectListItem>();
            switch (chart.GraphicType.ToLowerInvariant())
            {
                case "bar":
                    {
                        foreach (var serie in chart.Series)
                        {
                            if (serie.Stacks.Count > 0)
                            {
                                foreach (var stack in serie.Stacks)
                                {
                                    kpis.Add(new SelectListItem
                                    {
                                        Value = string.Format(@"{0}|{1}|{2}", stack.KpiId.ToString(), chart.ValueAxis, stack.Label)
                                        ,
                                        Text = stack.Label
                                    });
                                }
                            }
                            else
                            {
                                kpis.Add(new SelectListItem
                                {
                                    Value =
                                    string.Format(@"{0}|{1}|{2}", serie.KpiId.ToString(), chart.ValueAxis, serie.Label),
                                    Text = serie.Label
                                });
                            }
                        }

                        return kpis;
                    }
                case "line":
                    {
                        foreach (var serie in chart.Series)
                        {

                            kpis.Add(new SelectListItem
                            {
                                Value =
                                string.Format(@"{0}|{1}|{2}", serie.KpiId.ToString(), serie.ValueAxis, serie.Label),
                                Text = serie.Label
                            });

                        }

                        return kpis;
                    }
            }

            return new List<SelectListItem>();
        }

        private IList<SelectListItem> GetKpisAsSelectListItem(GetArtifactResponse artifact)
        {
            var kpis = new List<SelectListItem>();
            foreach (var serie in artifact.Series)
            {
                kpis.Add(new SelectListItem
                {
                    Value =
                    string.Format(@"{0}|{1}|{2}", serie.KpiId.ToString(), artifact.ValueAxis, serie.Label),
                    Text = serie.Label
                });
            }

            return kpis;
        }

        private string ParseDateToString(PeriodeType periodeType, DateTime? date)
        {
            switch (periodeType)
            {
                case PeriodeType.Yearly:
                    return date.HasValue ? date.Value.ToString("yyyy", CultureInfo.InvariantCulture) : string.Empty;
                case PeriodeType.Monthly:
                    return date.HasValue ? date.Value.ToString("MM/yyyy", CultureInfo.InvariantCulture) : string.Empty;
                case PeriodeType.Weekly:
                    return date.HasValue ? date.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : string.Empty;
                case PeriodeType.Daily:
                    return date.HasValue ? date.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : string.Empty;
                case PeriodeType.Hourly:
                    return date.HasValue ? date.Value.ToString("MM/dd/yyyy  h:mm", CultureInfo.InvariantCulture) : string.Empty;
                default:
                    return string.Empty;
            }
        }

        private ActionResult GetView(ArtifactPreviewViewModel previewViewModel, GetArtifactResponse artifactResp)
        {
            switch (artifactResp.GraphicType)
            {
                case "line":
                    {
                        var chartData = _artifactServie.GetChartData(artifactResp.MapTo<GetCartesianChartDataRequest>());
                        var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                        {
                            TimePeriodes = chartData.TimePeriodes,
                            Type = "Overall",
                            PeriodeType = artifactResp.PeriodeType
                        });
                        previewViewModel.PeriodeType = artifactResp.PeriodeType.ToString();
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        previewViewModel.LineChart = new LineChartDataViewModel();
                        previewViewModel.LineChart.Title = artifactResp.HeaderTitle;
                        previewViewModel.LineChart.Subtitle = chartData.Subtitle;
                        previewViewModel.LineChart.ValueAxisTitle = artifactResp.Measurement;
                        previewViewModel.LineChart.Series = chartData.Series.MapTo<LineChartDataViewModel.SeriesViewModel>();
                        previewViewModel.LineChart.Periodes = chartData.Periodes;
                    }
                    break;
                case "area":
                    {
                        var chartData = _artifactServie.GetChartData(artifactResp.MapTo<GetCartesianChartDataRequest>());
                        var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                        {
                            TimePeriodes = chartData.TimePeriodes,
                            Type = "Overall",
                            PeriodeType = artifactResp.PeriodeType
                        });
                        previewViewModel.PeriodeType = artifactResp.PeriodeType.ToString();
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.AreaChart = new AreaChartDataViewModel();
                        previewViewModel.AreaChart.Title = artifactResp.HeaderTitle;
                        previewViewModel.AreaChart.Subtitle = chartData.Subtitle;
                        previewViewModel.AreaChart.ValueAxisTitle = artifactResp.Measurement;
                        previewViewModel.AreaChart.Series = chartData.Series.MapTo<AreaChartDataViewModel.SeriesViewModel>();
                        previewViewModel.AreaChart.Periodes = chartData.Periodes;
                    }
                    break;
                case "multiaxis":
                    {
                        var chartData = _artifactServie.GetMultiaxisChartData(artifactResp.MapTo<GetMultiaxisChartDataRequest>());
                        var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                        {
                            TimePeriodes = chartData.TimePeriodes,
                            Type = "Overall",
                            PeriodeType = artifactResp.PeriodeType
                        });
                        previewViewModel.PeriodeType = artifactResp.PeriodeType.ToString();
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.MultiaxisChart = chartData.MapTo<MultiaxisChartDataViewModel>();
                        previewViewModel.MultiaxisChart.Title = artifactResp.HeaderTitle;
                    }
                    break;
                case "combo":
                    {
                        var chartData = _artifactServie.GetComboChartData(artifactResp.MapTo<GetComboChartDataRequest>());
                        var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                        {
                            TimePeriodes = chartData.TimePeriodes,
                            Type = "Overall",
                            PeriodeType = artifactResp.PeriodeType
                        });
                        previewViewModel.PeriodeType = artifactResp.PeriodeType.ToString();
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.ComboChart = chartData.MapTo<ComboChartDataViewModel>();
                        previewViewModel.ComboChart.Title = artifactResp.HeaderTitle;
                    }
                    break;
                case "speedometer":
                    {
                        var chartData = _artifactServie.GetSpeedometerChartData(artifactResp.MapTo<GetSpeedometerChartDataRequest>());
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.SpeedometerChart = new SpeedometerChartDataViewModel();
                        previewViewModel.SpeedometerChart.Title = artifactResp.HeaderTitle;
                        previewViewModel.SpeedometerChart.Subtitle = chartData.Subtitle;
                        previewViewModel.SpeedometerChart.ValueAxisTitle = artifactResp.Measurement;
                        previewViewModel.SpeedometerChart.Series = chartData.Series.MapTo<SpeedometerChartDataViewModel.SeriesViewModel>();
                        previewViewModel.SpeedometerChart.PlotBands = chartData.PlotBands.MapTo<SpeedometerChartDataViewModel.PlotBandViewModel>();
                    }
                    break;
                case "trafficlight":
                    {
                        var chartData = _artifactServie.GetTrafficLightChartData(artifactResp.MapTo<GetTrafficLightChartDataRequest>());
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.TrafficLightChart = new TrafficLightChartDataViewModel();
                        previewViewModel.TrafficLightChart.Title = artifactResp.HeaderTitle;
                        previewViewModel.TrafficLightChart.Subtitle = chartData.Subtitle;
                        previewViewModel.TrafficLightChart.ValueAxisTitle = artifactResp.Measurement;
                        previewViewModel.TrafficLightChart.Series = chartData.Series.MapTo<TrafficLightChartDataViewModel.SeriesViewModel>();
                        previewViewModel.TrafficLightChart.PlotBands = chartData.PlotBands.MapTo<TrafficLightChartDataViewModel.PlotBandViewModel>();
                    }
                    break;
                case "tabular":
                    {
                        var chartData = _artifactServie.GetTabularData(artifactResp.MapTo<GetTabularDataRequest>());
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.Tabular = new TabularDataViewModel();
                        chartData.MapPropertiesToInstance<TabularDataViewModel>(previewViewModel.Tabular);
                        previewViewModel.Tabular.Title = artifactResp.HeaderTitle;
                        //previewViewModel.SpeedometerChart.Series = chartData.Series.MapTo<SpeedometerChartDataViewModel.SeriesViewModel>();
                        //previewViewModel.SpeedometerChart.PlotBands = chartData.PlotBands.MapTo<SpeedometerChartDataViewModel.PlotBandViewModel>();
                    }
                    break;
                case "tank":
                    {
                        var chartData = _artifactServie.GetTankData(artifactResp.MapTo<GetTankDataRequest>());
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.Tank = new TankDataViewModel();
                        chartData.MapPropertiesToInstance<TankDataViewModel>(previewViewModel.Tank);
                        previewViewModel.Tank.Title = artifactResp.HeaderTitle;
                        previewViewModel.Tank.Subtitle = chartData.Subtitle;
                        previewViewModel.Tank.Id = artifactResp.Tank.Id;
                        //previewViewModel.SpeedometerChart.Series = chartData.Series.MapTo<SpeedometerChartDataViewModel.SeriesViewModel>();
                        //previewViewModel.SpeedometerChart.PlotBands = chartData.PlotBands.MapTo<SpeedometerChartDataViewModel.PlotBandViewModel>();
                    }
                    break;
                case "pie":
                    {
                        var chartData = _artifactServie.GetPieData(artifactResp.MapTo<GetPieDataRequest>());
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.Pie = chartData.MapTo<PieDataViewModel>();
                        previewViewModel.Pie.Title = artifactResp.HeaderTitle;
                        previewViewModel.Pie.Subtitle = chartData.Subtitle;
                        previewViewModel.Pie.Is3D = artifactResp.Is3D;
                        previewViewModel.Pie.ShowLegend = artifactResp.ShowLegend;
                    }
                    break;
                default:
                    {
                        var chartData = _artifactServie.GetChartData(artifactResp.MapTo<GetCartesianChartDataRequest>());
                        if (!artifactResp.AsNetbackChart)
                        {
                            var reportHighlights = _highlightService.GetReportHighlights(new GetReportHighlightsRequest
                            {
                                TimePeriodes = chartData.TimePeriodes,
                                Type = "Overall",
                                PeriodeType = artifactResp.PeriodeType
                            });
                            previewViewModel.Highlights = reportHighlights.Highlights.MapTo<ArtifactPreviewViewModel.HighlightViewModel>();
                        }
                        previewViewModel.PeriodeType = artifactResp.PeriodeType.ToString();
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.GraphicType = artifactResp.GraphicType;
                        previewViewModel.BarChart = new BarChartDataViewModel();
                        previewViewModel.BarChart.Title = artifactResp.HeaderTitle;
                        previewViewModel.BarChart.Subtitle = chartData.Subtitle;
                        previewViewModel.BarChart.ValueAxisTitle = artifactResp.Measurement; //.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
                        previewViewModel.BarChart.Series = chartData.Series.MapTo<BarChartDataViewModel.SeriesViewModel>();
                        previewViewModel.BarChart.Periodes = chartData.Periodes;
                        previewViewModel.BarChart.SeriesType = chartData.SeriesType;
                    }
                    break;
            }
            return Json(previewViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}
