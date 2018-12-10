using System.Globalization;
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
using DSLNG.PEAR.Common.Contants;

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
        private const int _prevIndex = -10000;
        private const int _totalIndex = -20000;
        private const int _exceedIndex = -30000;
        private const int _remainIndex = -40000;
        private const int _percentageIndex = -5000;

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
            var previewViewModel = GetPreview(viewModel);
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
                case "combo":
                    foreach (var chart in artifact.Charts)
                    {
                        foreach (var kpi in GetKpisAsSelectListItem(chart))
                        {
                            kpis.Add(new SelectListItem { Value = kpi.Value, Text = kpi.Text });
                        }
                    }
                    break;
                case "tank":
                case "baraccumulative":
                case "pie":
                default:
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
            viewModel.AsNetBackChart = artifact.AsNetbackChart;
            viewModel.Name = artifact.HeaderTitle;
            viewModel.ArtifactId = artifact.Id;

            viewModel.StartInDisplay = ParseDateToString(artifact.PeriodeType, artifact.Start);
            viewModel.EndInDisplay = ParseDateToString(artifact.PeriodeType, artifact.End);

            GetStartAndEnd(artifact.PeriodeType, artifact.RangeFilter, viewModel);

            return PartialView("_ExportSetting", viewModel);
        }

        [HttpPost]
        public ActionResult ExportSettingPreview(ArtifactDesignerViewModel designerViewModel)
        {
            var previewViewModel = GetPreview(designerViewModel);
            var kpis = new List<SelectListItem>();
            switch (previewViewModel.GraphicType.ToLowerInvariant())
            {
                case "tank":
                    {
                        var daysToTankTop = _kpiService.GetKpi(new GetKpiRequest { Id = designerViewModel.Tank.DaysToTankTopId });
                        var volumeInventory = _kpiService.GetKpi(new GetKpiRequest { Id = designerViewModel.Tank.VolumeInventoryId });
                        kpis.Add(new SelectListItem
                        {
                            Value =
                                string.Format(@"{0}|{1}|{2}|{3}", designerViewModel.Tank.DaysToTankTopId.ToString(), ValueAxis.KpiActual, designerViewModel.Tank.DaysToTankTopTitle, "days"),
                            Text = string.IsNullOrEmpty(designerViewModel.Tank.DaysToTankTopTitle) ? designerViewModel.Tank.DaysToTankTopTitle : daysToTankTop.Name
                        });

                        kpis.Add(new SelectListItem
                        {
                            Value =
                                string.Format(@"{0}|{1}|{2}|{3}", designerViewModel.Tank.VolumeInventoryId.ToString(), ValueAxis.KpiActual, volumeInventory.Name, "volume"),
                            Text = volumeInventory.Name
                        });
                        break;

                    }

                case "pie":
                    {
                        foreach (var serie in designerViewModel.Pie.Series)
                        {

                            kpis.Add(new SelectListItem
                            {
                                Value =
                                string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), designerViewModel.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? designerViewModel.ValueAxis : designerViewModel.ValueAxis, serie.Label, designerViewModel.GraphicType),
                                Text = serie.Label
                            });

                        }
                        break;
                    }
                case "line":
                    foreach (var serie in designerViewModel.LineChart.Series)
                    {
                        kpis.Add(new SelectListItem
                        {
                            Value =
                                string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), designerViewModel.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : designerViewModel.ValueAxis, serie.Label, designerViewModel.GraphicType),
                            Text = serie.Label
                        });
                    }
                    break;
                case "multiaxis":
                    foreach (var chart in designerViewModel.MultiaxisChart.Charts)
                    {
                        foreach (var kpi in GetKpisAsSelectListItem(chart))
                        {
                            kpis.Add(new SelectListItem { Value = kpi.Value, Text = kpi.Text });
                        }
                    }
                    break;
                case "combo":
                    foreach (var chart in designerViewModel.ComboChart.Charts)
                    {
                        foreach (var kpi in GetKpisAsSelectListItem(chart))
                        {
                            kpis.Add(new SelectListItem { Value = kpi.Value, Text = kpi.Text });
                        }
                    }
                    break;
                case "area":
                    foreach (var serie in designerViewModel.AreaChart.Series)
                    {
                        if (serie.Stacks.Count > 0)
                        {
                            foreach (var stack in serie.Stacks)
                            {
                                kpis.Add(new SelectListItem
                                {
                                    Value = string.Format(@"{0}|{1}|{2}|{3}", stack.KpiId.ToString(), designerViewModel.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : designerViewModel.ValueAxis, stack.Label, designerViewModel.GraphicType),
                                    Text = stack.Label
                                });
                            }
                        }
                        else
                        {
                            kpis.Add(new SelectListItem
                            {
                                Value =
                                string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), designerViewModel.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : designerViewModel.ValueAxis, serie.Label, designerViewModel.GraphicType),
                                Text = serie.Label
                            });
                        }
                    }
                    break;
                case "bar":
                case "baraccumulative":
                case "barachievement":
                case "barhorizontal":
                    foreach (var serie in designerViewModel.BarChart.Series)
                    {
                        if (serie.Stacks.Count > 0)
                        {
                            foreach (var stack in serie.Stacks)
                            {
                                kpis.Add(new SelectListItem
                                {
                                    Value = string.Format(@"{0}|{1}|{2}|{3}", stack.KpiId.ToString(), designerViewModel.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : designerViewModel.ValueAxis, stack.Label, designerViewModel.GraphicType),
                                    Text = stack.Label
                                });
                            }
                        }
                        else
                        {
                            kpis.Add(new SelectListItem
                            {
                                Value =
                                string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), designerViewModel.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : designerViewModel.ValueAxis, serie.Label, designerViewModel.GraphicType),
                                Text = serie.Label
                            });
                        }
                    }
                    break;
            }
            var viewModel = new ExportSettingViewModel();
            viewModel.Kpis = kpis;
            viewModel.PeriodeType = designerViewModel.PeriodeType.ToString();
            viewModel.RangeFilter = designerViewModel.RangeFilter.ToString();
            viewModel.GraphicType = designerViewModel.GraphicType;
            viewModel.AsNetBackChart = designerViewModel.AsNetbackChart;
            viewModel.Name = string.IsNullOrEmpty(designerViewModel.HeaderTitle) ? "Simulated Data Extraction" : designerViewModel.HeaderTitle;
            viewModel.ArtifactId = designerViewModel.Id;


            var rangeFilter = (RangeFilter)Enum.Parse(typeof(RangeFilter), designerViewModel.RangeFilter);
            var periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), designerViewModel.PeriodeType);

            viewModel.StartInDisplay = designerViewModel.StartInDisplay;
            viewModel.EndInDisplay = designerViewModel.EndInDisplay;

            GetStartAndEnd(periodeType, rangeFilter, viewModel);

            return PartialView("_ExportSettingPreview", viewModel);
        }

        [HttpPost]
        public ActionResult ExportSetting(ExportSettingViewModel viewModel)
        {
            try
            {
                viewModel.KpiIds = viewModel.KpiIds.Select(x => x).Where(x => x != "*").ToArray();

                var labelDictionaries = new Dictionary<string, List<string>>();
                if (viewModel.KpiIds != null)
                {
                    foreach (var item in viewModel.KpiIds)
                    {
                        var split = item.Split('|');
                        if (split.Length > 2)
                        {
                            var kpiIndex = split[0] + "|" + split[1];
                            if (!labelDictionaries.Keys.Contains(kpiIndex))
                            {
                                labelDictionaries.Add(kpiIndex, new List<string> { split[0], split[1], split[2], split[3] });
                            }
                            else
                            {
                                labelDictionaries[kpiIndex] = new List<string> { split[0], split[1], split[2], split[3] };
                            }
                        }
                    }
                }

                var periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType);
                var rangeFilter = (RangeFilter)Enum.Parse(typeof(RangeFilter), viewModel.RangeFilter);

                IList<DateTime> dateTimePeriodes = new List<DateTime>();
                string timeInformation;

                IList<ExportSettingData> exportData = new List<ExportSettingData>();
                switch (viewModel.GraphicType.ToLowerInvariant())
                {
                    case "tabular":
                        var artifact = _artifactServie.GetArtifact(new GetArtifactRequest { Id = viewModel.ArtifactId });
                        var request = new GetTabularDataRequest();
                        request.Actual = artifact.Actual;
                        request.Target = artifact.Target;
                        request.Rows = artifact.Rows.Select(x => new GetTabularDataRequest.RowRequest { KpiId = x.KpiId, End = x.End, KpiName = x.KpiName, PeriodeType = x.PeriodeType, RangeFilter = x.RangeFilter, Start = x.Start }).ToList();
                        var data = _artifactServie.GetTabularData(request);
                        return ExportTabular(data, viewModel.Name, viewModel.FileName);
                    case "pie":
                        exportData = _artifactServie.GetExportExcelPieData(labelDictionaries, rangeFilter, viewModel.StartAfterParsed, viewModel.EndAfterParsed, periodeType, ArtifactValueInformation.AsOf, out dateTimePeriodes, out timeInformation);
                        break;
                    case "tank":
                        exportData = _artifactServie.GetExportExcelTankData(labelDictionaries, rangeFilter, viewModel.StartAfterParsed, viewModel.EndAfterParsed, periodeType, ArtifactValueInformation.AsOf, out dateTimePeriodes, out timeInformation);
                        break;
                    default:
                        _artifactServie.GetPeriodes(periodeType, rangeFilter, viewModel.StartAfterParsed, viewModel.EndAfterParsed, out dateTimePeriodes, out timeInformation);
                        exportData = _artifactServie.GetExportExcelData(labelDictionaries, viewModel.StartAfterParsed, viewModel.EndAfterParsed, viewModel.PeriodeType);
                        break;
                }

                var exportKpis = ModifyKpis(viewModel.KpiIds);
                IDictionary<DateTime, IDictionary<string, ExportSettingData>> dataDictionary = new Dictionary<DateTime, IDictionary<string, ExportSettingData>>();
                IList<DateTime> existedPeriodes = new List<DateTime>();
                dateTimePeriodes = FilterPeriodes(dateTimePeriodes, viewModel.StartAfterParsed, viewModel.EndAfterParsed);
                foreach (var periode in dateTimePeriodes)
                {
                    var data = new Dictionary<string, ExportSettingData>();
                    foreach (var exportKpi in exportKpis)
                    {
                        if (exportKpi.KpiName == "Previous Accumulation")
                        {
                            var previousIndex = string.Format(@"{0}|{1}", exportKpi.KpiId.ToString(), exportKpi.ValueAxes);
                            var item = new ExportSettingData { KpiId = exportKpi.KpiId, KpiName = exportKpi.KpiName, KpiReferenceId = exportKpi.KpiReferenceId, MeasurementName = exportKpi.MeasurementName, Periode = periode, ValueAxes = exportKpi.ValueAxes };
                            item.Value = existedPeriodes.Count == 0 ? 0 : exportData.Where(x => existedPeriodes.Contains(x.Periode) && x.KpiId == exportKpi.KpiReferenceId && x.ValueAxes == exportKpi.ValueAxes).Sum(x => x.Value.Value);
                            data.Add(previousIndex, item);
                        }
                        else if (exportKpi.KpiName == "Total")
                        {
                            var currentIndex = string.Format(@"{0}|{1}", exportKpi.KpiReferenceId.ToString(), exportKpi.ValueAxes);
                            var previousIndex = string.Format(@"{0}|{1}", (_prevIndex - exportKpi.KpiReferenceId).ToString(), exportKpi.ValueAxes);
                            var totalIndex = string.Format(@"{0}|{1}", exportKpi.KpiId.ToString(), exportKpi.ValueAxes);
                            var item = new ExportSettingData { KpiId = exportKpi.KpiId, KpiName = exportKpi.KpiName, KpiReferenceId = exportKpi.KpiReferenceId, MeasurementName = exportKpi.MeasurementName, Periode = periode, ValueAxes = exportKpi.ValueAxes };
                            if (data[previousIndex].Value.HasValue && data[currentIndex].Value.HasValue)
                            {
                                item.Value = (data[previousIndex].Value) + (data[currentIndex].Value);
                            }

                            data.Add(totalIndex, item);
                        }
                        else if (exportKpi.KpiName == "Remain")
                        {
                            var target = _kpiTargetService.GetKpiTarget(exportKpi.KpiReferenceId, periode, periodeType);
                            var remainIndex = string.Format(@"{0}|{1}", exportKpi.KpiId.ToString(), exportKpi.ValueAxes);
                            var currentIndex = string.Format(@"{0}|{1}", exportKpi.KpiReferenceId.ToString(), exportKpi.ValueAxes);
                            var item = new ExportSettingData { KpiId = exportKpi.KpiId, KpiName = exportKpi.KpiName, KpiReferenceId = exportKpi.KpiReferenceId, MeasurementName = exportKpi.MeasurementName, Periode = periode, ValueAxes = exportKpi.ValueAxes };
                            item.Value = 0;
                            if (target.Value.HasValue && data[currentIndex].Value.HasValue)
                            {
                                if ((target.Value.Value) - (data[currentIndex].Value) > 0)
                                {
                                    item.Value = (target.Value.Value) - (data[currentIndex].Value);
                                }
                            }
                            data.Add(remainIndex, item);
                        }
                        else if (exportKpi.KpiName == "Exceed")
                        {
                            var target = _kpiTargetService.GetKpiTarget(exportKpi.KpiReferenceId, periode, periodeType);
                            var exceedIndex = string.Format(@"{0}|{1}", exportKpi.KpiId.ToString(), exportKpi.ValueAxes);
                            var currentIndex = string.Format(@"{0}|{1}", exportKpi.KpiReferenceId.ToString(), exportKpi.ValueAxes);
                            var item = new ExportSettingData { KpiId = exportKpi.KpiId, KpiName = exportKpi.KpiName, KpiReferenceId = exportKpi.KpiReferenceId, MeasurementName = exportKpi.MeasurementName, Periode = periode, ValueAxes = exportKpi.ValueAxes };
                            item.Value = 0;
                            if (target.Value.HasValue && data[currentIndex].Value.HasValue)
                            {
                                if ((target.Value.Value) - (data[currentIndex].Value) < 0)
                                {
                                    item.Value = -((target.Value.Value) - (data[currentIndex].Value));
                                }
                            }
                            data.Add(exceedIndex, item);
                        }
                        else if (exportKpi.KpiName == "Percentage")
                        {
                            var currentIndex = string.Format(@"{0}|{1}", exportKpi.KpiReferenceId.ToString(), exportKpi.ValueAxes);
                            var item = new ExportSettingData { KpiId = exportKpi.KpiId, KpiName = exportKpi.KpiName, KpiReferenceId = exportKpi.KpiReferenceId, MeasurementName = exportKpi.MeasurementName, Periode = periode, ValueAxes = exportKpi.ValueAxes };
                            item.Value = 0;
                            if (data[currentIndex].Value.HasValue)
                            {
                                item.Value = (data[currentIndex].Value / exportData.Select(x => x.Value).Sum()) * 100;
                            }
                        }
                        else
                        {
                            if (!viewModel.AsNetBackChart)
                            {
                                var currentIndex = string.Format(@"{0}|{1}", exportKpi.KpiId.ToString(), exportKpi.ValueAxes);
                                var val = exportData.Where(x => x.Periode == periode && x.KpiId == exportKpi.KpiId && x.ValueAxes == exportKpi.ValueAxes).FirstOrDefault();
                                if (val == null)
                                {
                                    var item = new ExportSettingData { KpiId = exportKpi.KpiId, KpiName = exportKpi.KpiName, KpiReferenceId = exportKpi.KpiId, MeasurementName = exportKpi.MeasurementName, Periode = periode, ValueAxes = exportKpi.ValueAxes };
                                    data.Add(currentIndex, item);
                                }
                                else
                                {
                                    data.Add(currentIndex, val);
                                }
                            }
                            else
                            {
                                var currentIndex = string.Format(@"{0}|{1}", exportKpi.KpiId.ToString(), exportKpi.ValueAxes);
                                var item = new ExportSettingData { KpiId = exportKpi.KpiId, KpiName = exportKpi.KpiName, KpiReferenceId = exportKpi.KpiId, MeasurementName = exportKpi.MeasurementName, Periode = periode, ValueAxes = exportKpi.ValueAxes };
                                item.Value = exportData.Where(x => x.KpiId == exportKpi.KpiId && x.ValueAxes == exportKpi.ValueAxes).Average(x => x.Value);
                                data.Add(currentIndex, item);
                            }
                        }
                    }
                    dataDictionary.Add(periode, data);
                    existedPeriodes.Add(periode);
                }
                string newDateTimeInformation = string.Empty;
                if (viewModel.AsNetBackChart)
                {
                    var start = rangeFilter == RangeFilter.AllExistingYears ? DateTime.MinValue : dateTimePeriodes[0];
                    var end = rangeFilter == RangeFilter.AllExistingYears ? DateTime.MaxValue : dateTimePeriodes[dateTimePeriodes.Count - 1];
                    switch (periodeType)
                    {
                        case PeriodeType.Hourly:
                            newDateTimeInformation = start.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture) + " - " + end.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Daily:
                            newDateTimeInformation = start.ToString("dd MMM yy", CultureInfo.InvariantCulture) + " - " + end.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Monthly:
                            newDateTimeInformation = start.ToString("MMM yy", CultureInfo.InvariantCulture) + " - " + end.ToString("MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Yearly:
                            newDateTimeInformation = start.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture) + " - " + end.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                            break;
                    }
                }

                if (viewModel.GraphicType.ToLowerInvariant() == "pie")
                {
                    newDateTimeInformation = string.IsNullOrEmpty(timeInformation) ? timeInformation : string.Empty;
                }

                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

                ws.Cells["A1"].Value = "Extracted Date";
                ws.Cells["B1"].Value = ": " + string.Format("{0:dd MMMM yyyy hh:mm tt}", DateTimeOffset.Now);
                ws.Cells["C1"].Value = "By: " + UserProfile().Name;

                ws.Cells["A2"].Value = "Artifact Name";
                ws.Cells["B2"].Value = ": " + viewModel.Name;


                ws.Cells["A4:A5"].Value = "Periode";
                ws.Cells["A4:A5"].Merge = true;
                ws.Cells["A4:A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A4:A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A4:A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells["A4:A5"].Style.Font.Bold = true;

                int kpiRowStart = 6;
                int kpiColStart = 2;
                IDictionary<int, string> kpiDictionaries = new Dictionary<int, string>();

                foreach (var dictionary in dataDictionary)
                {
                    foreach (var kpi in dictionary.Value)
                    {
                        ws.Cells[4, kpiColStart].Value = kpi.Value.KpiId > 0 ?
                            string.Format("KPI ID {0}, {1} ({2})", kpi.Value.KpiId, kpi.Value.KpiName, kpi.Value.MeasurementName) :
                            string.Format("{0} ({1})", kpi.Value.KpiName, kpi.Value.MeasurementName);
                        ws.Cells[4, kpiColStart].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[4, kpiColStart].Style.Font.Bold = true;
                        ws.Cells[4, kpiColStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[5, kpiColStart].Value = kpi.Value.KpiId > 0 ?
                            labelDictionaries.Single(x => x.Key == kpi.Value.KpiId + "|" + kpi.Value.ValueAxes).Value[2] : string.Empty;
                        ws.Cells[5, kpiColStart].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[5, kpiColStart].Style.Font.Bold = true;
                        ws.Cells[5, kpiColStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        kpiColStart++;

                    }
                    break;

                }

                kpiColStart = 1;
                foreach (var dictionary in dataDictionary)
                {
                    ws.Cells[kpiRowStart, kpiColStart].Value = string.IsNullOrEmpty(newDateTimeInformation) ? FormatDate(dictionary.Key, viewModel.PeriodeType) : newDateTimeInformation;
                    ws.Cells[kpiRowStart, kpiColStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[kpiRowStart, kpiColStart].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    foreach (var val in dictionary.Value)
                    {
                        kpiColStart++;
                        ws.Cells[kpiRowStart, kpiColStart].Style.Numberformat.Format = "#,##0.00";
                        ws.Cells[kpiRowStart, kpiColStart].Value = val.Value.Value;
                        ws.Cells[kpiRowStart, kpiColStart].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[kpiRowStart, kpiColStart].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    kpiRowStart++;
                    kpiColStart = 1;
                    if (viewModel.AsNetBackChart) break;

                }

                ws.Cells["A:AZ"].AutoFitColumns();

                string handle = Guid.NewGuid().ToString();
                TempData[handle] = pck.GetAsByteArray();
                
                return Json(new { FileGuid = handle, FileName = viewModel.FileName + ".xlsx", IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, IsSuccess = false });
            }
        }

        [HttpGet]
        public virtual ActionResult DownloadExportToExcel(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }

        private IList<DateTime> FilterPeriodes(IList<DateTime> dateTimePeriodes, DateTime? startAfterParsed, DateTime? endAfterParsed)
        {
            if(startAfterParsed.HasValue && endAfterParsed.HasValue)
            {
                return dateTimePeriodes.Where(x => x <= endAfterParsed && x >= startAfterParsed).ToList();
            }

            return dateTimePeriodes;

        }

        private ActionResult ExportTabular(GetTabularDataResponse data, string graphicName, string fileName)
        {
            try
            {
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

                ws.Cells["A1"].Value = "Extracted Date";
                ws.Cells["B1"].Value = ": " + DateTime.Now.ToString("dd/MMMM/yyyy");
                ws.Cells["C1"].Value = "By: " + UserProfile().Name;

                ws.Cells["A2"].Value = "Artifact Name";
                ws.Cells["B2"].Value = ": " + graphicName;

                ws.Cells["A1"].Value = "Date";
                ws.Cells["B1"].Value = string.Format("{0:dd MMMM yyyy hh:mm tt}", DateTimeOffset.Now);

                int kpiRowStart = 6;
                int kpiColStart = 2;
                IDictionary<int, string> kpiDictionaries = new Dictionary<int, string>();
                
                var labels = new List<string>();
                labels.Add("KPI Name");
                labels.Add("Periode");
                if (data.Actual)
                {
                    labels.Add("Actual");
                }

                if (data.Target)
                {
                    labels.Add("Target");
                }

                if (data.Remark)
                {
                    labels.Add("Remark");
                }
                var headerIndex = 1;
                foreach (var label in labels)
                {
                    ws.Cells[4, headerIndex].Value = label;
                    ws.Cells[4, headerIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    ws.Cells[4, headerIndex].Style.Font.Bold = true;
                    ws.Cells[4, headerIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    headerIndex++;
                }

                int rowStart = 5;
                foreach (var row in data.Rows)
                {
                    int idx = 1;
                    ws.Cells[rowStart, idx].Value = string.Format("KPI ID {0}, {1} ({2})", row.KpiId, row.KpiName, row.Measurement);
                    ws.Cells[rowStart, idx].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    ws.Cells[rowStart, idx].Style.Font.Bold = false;
                    ws.Cells[rowStart, idx].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    idx++;

                    ws.Cells[rowStart, idx].Value = string.Format("{0}", row.Periode.ToString());
                    ws.Cells[rowStart, idx].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    ws.Cells[rowStart, idx].Style.Font.Bold = false;
                    ws.Cells[rowStart, idx].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    if (data.Actual)
                    {
                        idx++;
                        ws.Cells[rowStart, idx].Value = row.Actual;
                        ws.Cells[rowStart, idx].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[rowStart, idx].Style.Font.Bold = false;
                        ws.Cells[rowStart, idx].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[rowStart, idx].Style.Numberformat.Format = "#,##0.00";
                    }

                    if (data.Target)
                    {
                        idx++;
                        ws.Cells[rowStart, idx].Value = row.Target;
                        ws.Cells[rowStart, idx].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[rowStart, idx].Style.Font.Bold = false;
                        ws.Cells[rowStart, idx].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[rowStart, idx].Style.Numberformat.Format = "#,##0.00";
                    }

                    if (data.Remark)
                    {
                        idx++;
                        ws.Cells[rowStart, idx].Value = row.Remark;
                        ws.Cells[rowStart, idx].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[rowStart, idx].Style.Font.Bold = false;
                        ws.Cells[rowStart, idx].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[rowStart, idx].Style.Numberformat.Format = "#,##0.00";
                    }
                    rowStart++;

                }

                ws.Cells["A:AZ"].AutoFitColumns();

                string handle = Guid.NewGuid().ToString();
                TempData[handle] = pck.GetAsByteArray();

                return Json(new { FileGuid = handle, FileName = fileName + ".xlsx", IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, IsSuccess = false });
            }
        }

        private List<ExportSettingData> GetExistedKpis(IList<ExportSettingData> existedKpis)
        {
            var data = new List<ExportSettingData>();
            IDictionary<string, ExportSettingData> dictionary = new Dictionary<string, ExportSettingData>();
            foreach (var kpi in existedKpis)
            {
                var key = string.Format(@"{0}|{1}|{2}", kpi.KpiId, kpi.ValueAxes, kpi.KpiGraphicType);
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, kpi);
                    data.Add(kpi);
                }
            }

            return data;
        }

        private List<KpiExport> ModifyKpis(string[] kpiIds)
        {
            var listKpiId = kpiIds.Select(x => Int32.Parse(x.Split('|')[0])).ToList();

            var kpis = _kpiService.GetKpis(listKpiId);
            var kpiExports = new List<KpiExport>();
            foreach (var kpi in kpiIds)
            {
                var key = kpi.Split('|');
                var kpiExport = new KpiExport
                {
                    KpiId = Int32.Parse(key[0]),
                    ValueAxes = key[1],
                    KpiName = key[2],
                    KpiGraphicType = key[3],
                    MeasurementName = kpis.Kpis.First(x => x.Id == Int32.Parse(key[0])).Measurement.Name
                };

                if (kpiExport.KpiGraphicType == "baraccumulative")
                {
                    kpiExports.Add(new KpiExport { KpiId = _prevIndex - kpiExport.KpiId, KpiReferenceId = kpiExport.KpiId, KpiName = "Previous Accumulation", ValueAxes = kpiExport.ValueAxes, MeasurementName = kpiExport.MeasurementName });
                    kpiExports.Add(kpiExport);
                    kpiExports.Add(new KpiExport { KpiId = _totalIndex - kpiExport.KpiId, KpiReferenceId = kpiExport.KpiId, KpiName = "Total", ValueAxes = kpiExport.ValueAxes, MeasurementName = kpiExport.MeasurementName });
                }
                else if (kpiExport.KpiGraphicType == "barachievement")
                {
                    kpiExports.Add(kpiExport);
                    kpiExports.Add(new KpiExport { KpiId = _remainIndex - kpiExport.KpiId, KpiReferenceId = kpiExport.KpiId, KpiName = "Remain", ValueAxes = kpiExport.ValueAxes, MeasurementName = kpiExport.MeasurementName });
                    kpiExports.Add(new KpiExport { KpiId = _exceedIndex - kpiExport.KpiId, KpiReferenceId = kpiExport.KpiId, KpiName = "Exceed", ValueAxes = kpiExport.ValueAxes, MeasurementName = kpiExport.MeasurementName });
                }
                else if (kpiExport.KpiGraphicType == "pie")
                {
                    kpiExports.Add(kpiExport);
                    kpiExports.Add(new KpiExport { KpiId = _percentageIndex - kpiExport.KpiId, KpiReferenceId = kpiExport.KpiId, KpiName = "Percentage", ValueAxes = kpiExport.ValueAxes, MeasurementName = kpiExport.MeasurementName });
                }
                else
                {
                    kpiExports.Add(kpiExport);
                }
            }

            return kpiExports;
        }

        private List<ExportSettingData> ModifyKpis(List<ExportSettingData> existedKpis, string graphicType)
        {

            var modifiedKpis = new List<ExportSettingData>();
            foreach (var kpi in existedKpis)
            {
                if (kpi.KpiGraphicType == "baraccumulative")
                {
                    modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId, KpiReferenceId = kpi.KpiId, KpiName = "Previous Accumulation", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });
                    modifiedKpis.Add(kpi);
                    modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId + 1, KpiReferenceId = kpi.KpiId, KpiName = "Total", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });
                }
                else if (kpi.KpiGraphicType == "barachievement")
                {
                    modifiedKpis.Add(kpi);
                    modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId, KpiReferenceId = kpi.KpiId, KpiName = "Remain", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });
                    modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId + 1, KpiReferenceId = kpi.KpiId, KpiName = "Exceed", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });
                }
                else
                {
                    modifiedKpis.Add(kpi);
                }
            }
            //switch (graphicType.ToLowerInvariant())
            //{
            //    case "baraccumulative":
            //        foreach (var kpi in existedKpis)
            //        {
            //            modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId, KpiReferenceId = kpi.KpiId, KpiName = "Previous Accumulation", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });
            //            modifiedKpis.Add(kpi);
            //            modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId + 1, KpiReferenceId = kpi.KpiId, KpiName = "Total", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });
            //        }
            //        break;
            //    case "barachievement":
            //        foreach (var kpi in existedKpis)
            //        {
            //            modifiedKpis.Add(kpi);
            //            modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId, KpiReferenceId = kpi.KpiId, KpiName = "Remain", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });
            //            modifiedKpis.Add(new ExportSettingData { KpiId = 0 - kpi.KpiId + 1, KpiReferenceId = kpi.KpiId, KpiName = "Exceed", MeasurementName = kpi.MeasurementName, Periode = kpi.Periode, Value = null, ValueAxes = kpi.ValueAxes });

            //        }
            //        break;
            //    default:
            //        modifiedKpis = existedKpis;
            //        break;
            //}

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
                case "barachievement":
                    {
                        foreach (var serie in chart.Series)
                        {
                            if (serie.Stacks.Count > 0)
                            {
                                foreach (var stack in serie.Stacks)
                                {
                                    kpis.Add(new SelectListItem
                                    {
                                        Value = string.Format(@"{0}|{1}|{2}|{3}", stack.KpiId.ToString(), chart.ValueAxis == ValueAxis.Custom ? serie.ValueAxis : chart.ValueAxis, stack.Label, chart.GraphicType),
                                        Text = stack.Label
                                    });
                                }
                            }
                            else
                            {
                                kpis.Add(new SelectListItem
                                {
                                    Value =
                                    string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), chart.ValueAxis == ValueAxis.Custom ? serie.ValueAxis : chart.ValueAxis, serie.Label, chart.GraphicType),
                                    Text = serie.Label
                                });
                            }
                        }

                        return kpis;
                    }
                case "line":
                case "pie":
                    {
                        foreach (var serie in chart.Series)
                        {

                            kpis.Add(new SelectListItem
                            {
                                Value =
                                string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), chart.ValueAxis == ValueAxis.Custom ? serie.ValueAxis : chart.ValueAxis, serie.Label, chart.GraphicType),
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
            if (artifact.GraphicType.ToLowerInvariant() == "tank")
            {
                var daysToTankTop = _kpiService.GetKpi(new GetKpiRequest { Id = artifact.Tank.DaysToTankTopId });
                var volumeInventory = _kpiService.GetKpi(new GetKpiRequest { Id = artifact.Tank.VolumeInventoryId });
                kpis.Add(new SelectListItem
                {
                    Value =
                        string.Format(@"{0}|{1}|{2}|{3}", artifact.Tank.DaysToTankTopId.ToString(), ValueAxis.KpiActual, artifact.Tank.DaysToTankTopTitle, "days"),
                    Text = string.IsNullOrEmpty(artifact.Tank.DaysToTankTopTitle) ? artifact.Tank.DaysToTankTopTitle : daysToTankTop.Name
                });

                kpis.Add(new SelectListItem
                {
                    Value =
                        string.Format(@"{0}|{1}|{2}|{3}", artifact.Tank.VolumeInventoryId.ToString(), ValueAxis.KpiActual, volumeInventory.Name, "volume"),
                    Text = volumeInventory.Name
                });
            }
            else
            {
                foreach (var serie in artifact.Series)
                {
                    if (serie.Stacks.Count > 0)
                    {
                        foreach (var stack in serie.Stacks)
                        {
                            kpis.Add(new SelectListItem
                            {
                                Value =
                        string.Format(@"{0}|{1}|{2}|{3}", stack.KpiId.ToString(), artifact.ValueAxis == ValueAxis.Custom ?
                        serie.ValueAxis : artifact.ValueAxis, stack.Label, artifact.GraphicType),
                                Text = stack.Label
                            });
                        }
                    }
                    else
                    {
                        kpis.Add(new SelectListItem
                        {
                            Value =
                       string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), artifact.ValueAxis == ValueAxis.Custom ?
                       serie.ValueAxis : artifact.ValueAxis, serie.Label, artifact.GraphicType),
                            Text = string.IsNullOrEmpty(serie.Label) ? serie.KpiName : serie.Label
                        });
                    }

                }
            }

            return kpis;
        }

        private IList<SelectListItem> GetKpisAsSelectListItem(MultiaxisChartViewModel.ChartViewModel chart)
        {
            var kpis = new List<SelectListItem>();
            switch (chart.GraphicType.ToLowerInvariant())
            {
                case "bar":
                case "barachievement":
                    {
                        foreach (var serie in chart.BarChart.Series)
                        {
                            if (serie.Stacks.Count > 0)
                            {
                                foreach (var stack in serie.Stacks)
                                {
                                    kpis.Add(new SelectListItem
                                    {
                                        Value = string.Format(@"{0}|{1}|{2}|{3}", stack.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, stack.Label, chart.GraphicType),
                                        Text = stack.Label
                                    });
                                }
                            }
                            else
                            {
                                kpis.Add(new SelectListItem
                                {
                                    Value =
                                    string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, serie.Label, chart.GraphicType),
                                    Text = serie.Label
                                });
                            }
                        }

                        return kpis;
                    }
                case "line":
                    {
                        foreach (var serie in chart.LineChart.Series)
                        {

                            kpis.Add(new SelectListItem
                            {
                                Value =
                                string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, serie.Label, chart.GraphicType),
                                Text = serie.Label
                            });

                        }

                        return kpis;
                    }
                case "area":
                    {
                        foreach (var serie in chart.AreaChart.Series)
                        {
                            if (serie.Stacks.Count > 0)
                            {
                                foreach (var stack in serie.Stacks)
                                {
                                    kpis.Add(new SelectListItem
                                    {
                                        Value = string.Format(@"{0}|{1}|{2}|{3}", stack.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, stack.Label, chart.GraphicType),
                                        Text = stack.Label
                                    });
                                }
                            }
                            else
                            {
                                kpis.Add(new SelectListItem
                                {
                                    Value =
                                    string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, serie.Label, chart.GraphicType),
                                    Text = serie.Label
                                });
                            }
                        }

                        return kpis;
                    }
            }

            return new List<SelectListItem>();
        }

        private IList<SelectListItem> GetKpisAsSelectListItem(ComboChartViewModel.ChartViewModel chart)
        {
            var kpis = new List<SelectListItem>();
            switch (chart.GraphicType.ToLowerInvariant())
            {
                case "bar":
                case "barachievement":
                    {
                        foreach (var serie in chart.BarChart.Series)
                        {
                            if (serie.Stacks.Count > 0)
                            {
                                foreach (var stack in serie.Stacks)
                                {
                                    kpis.Add(new SelectListItem
                                    {
                                        Value = string.Format(@"{0}|{1}|{2}|{3}", stack.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, stack.Label, chart.GraphicType),
                                        Text = stack.Label
                                    });
                                }
                            }
                            else
                            {
                                kpis.Add(new SelectListItem
                                {
                                    Value =
                                    string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, serie.Label, chart.GraphicType),
                                    Text = serie.Label
                                });
                            }
                        }

                        return kpis;
                    }
                case "line":
                    {
                        foreach (var serie in chart.LineChart.Series)
                        {

                            kpis.Add(new SelectListItem
                            {
                                Value =
                                string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, serie.Label, chart.GraphicType),
                                Text = serie.Label
                            });

                        }

                        return kpis;
                    }
                case "area":
                    {
                        foreach (var serie in chart.AreaChart.Series)
                        {
                            if (serie.Stacks.Count > 0)
                            {
                                foreach (var stack in serie.Stacks)
                                {
                                    kpis.Add(new SelectListItem
                                    {
                                        Value = string.Format(@"{0}|{1}|{2}|{3}", stack.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, stack.Label, chart.GraphicType),
                                        Text = stack.Label
                                    });
                                }
                            }
                            else
                            {
                                kpis.Add(new SelectListItem
                                {
                                    Value =
                                    string.Format(@"{0}|{1}|{2}|{3}", serie.KpiId.ToString(), chart.ValueAxis.ToLowerInvariant() == ValueAxis.Custom.ToString().ToLowerInvariant() ? serie.ValueAxis : chart.ValueAxis, serie.Label, chart.GraphicType),
                                    Text = serie.Label
                                });
                            }
                        }

                        return kpis;
                    }
            }

            return new List<SelectListItem>();
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

        private ArtifactPreviewViewModel GetPreview(ArtifactDesignerViewModel viewModel)
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

            return previewViewModel;
        }

        private DateTime StartOfWeek()
        {
            DateTime dt = DateTime.Now;
            int diff = dt.DayOfWeek - DayOfWeek.Sunday;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        private void GetStartAndEnd(PeriodeType periodeType, RangeFilter rangeFilter, ExportSettingViewModel viewModel)
        {
            var currentYear = DateTime.Now.Date.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDay = DateTime.Now.Day;
            switch (periodeType)
            {
                case PeriodeType.Daily:
                    {
                        switch (rangeFilter)
                        {
                            case RangeFilter.CurrentDay:
                                {
                                    var currentDays = DateTime.Now.Date;
                                    viewModel.StartInDisplay = ParseDateToString(periodeType, currentDays);
                                    viewModel.EndInDisplay = ParseDateToString(periodeType, currentDays);
                                    break;
                                }
                            case RangeFilter.CurrentWeek:
                                {
                                    var startDay = StartOfWeek();
                                    var endDay = startDay.AddDays(6);
                                    viewModel.StartInDisplay = ParseDateToString(periodeType, startDay);
                                    viewModel.EndInDisplay = ParseDateToString(periodeType, endDay);
                                    break;
                                }
                            case RangeFilter.CurrentMonth:
                                {
                                    viewModel.StartInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                                    viewModel.EndInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
                                    break;
                                }
                            case RangeFilter.MTD:
                                {
                                    viewModel.StartInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, currentMonth, 1));
                                    viewModel.EndInDisplay = ParseDateToString(periodeType, DateTime.Now);
                                    break;
                                }
                            case RangeFilter.YTD:
                                {
                                    viewModel.StartInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, 1, 1));
                                    viewModel.EndInDisplay = ParseDateToString(periodeType, DateTime.Now);
                                    break;
                                }
                        }
                        break;
                    }
                case PeriodeType.Monthly:
                    {
                        switch (rangeFilter)
                        {
                            case RangeFilter.CurrentMonth:
                                {
                                    viewModel.StartInDisplay = ParseDateToString(periodeType, DateTime.Now.Date);
                                    viewModel.EndInDisplay = ParseDateToString(periodeType, DateTime.Now.Date);
                                }
                                break;
                            case RangeFilter.CurrentYear:
                                {
                                    viewModel.StartInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, 1, 1));
                                    viewModel.EndInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, 12, 1));
                                }
                                break;
                            case RangeFilter.YTD:
                                {
                                    viewModel.StartInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, 1, 1));
                                    viewModel.EndInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                                }
                                break;
                        }
                        break;
                    }

                case PeriodeType.Yearly:
                    {
                        switch (rangeFilter)
                        {
                            case RangeFilter.CurrentYear:
                                {
                                    viewModel.StartInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, 1, 1));
                                    viewModel.EndInDisplay = ParseDateToString(periodeType, new DateTime(DateTime.Now.Year, 12, 1));
                                    break;
                                }
                        }
                        break;
                    }

            }
        }
    }
}
