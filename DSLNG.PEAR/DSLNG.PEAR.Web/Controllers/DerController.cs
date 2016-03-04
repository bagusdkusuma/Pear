using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Artifact;
using DSLNG.PEAR.Services.Requests.Der;
using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Services.Responses.Der;
using DSLNG.PEAR.Web.ViewModels.Artifact;
using DSLNG.PEAR.Web.ViewModels.Der;
using DSLNG.PEAR.Web.ViewModels.Der.Display;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerController : BaseController
    {
        private readonly IDerService _derService;
        private readonly IDropdownService _dropdownService;
        private readonly IArtifactService _artifactService;
        private readonly IHighlightService _highlightService;
        private readonly IWeatherService _weatherService;
        private readonly IKpiAchievementService _kpiAchievementService;
        private readonly IKpiTargetService _kpiTargetService;

        public DerController(IDerService derService, IDropdownService dropdownService, IArtifactService artifactService, IHighlightService highlightService, IWeatherService weatherService, IKpiAchievementService kpiAchievementService, IKpiTargetService kpiTargetService)
        {
            _derService = derService;
            _dropdownService = dropdownService;
            _artifactService = artifactService;
            _highlightService = highlightService;
            _weatherService = weatherService;
            _kpiAchievementService = kpiAchievementService;
            _kpiTargetService = kpiTargetService;
        }

        public ActionResult Index()
        {
            var response = _derService.GetActiveDer();
            var viewModel = response.MapTo<DerIndexViewModel>();
            return View(viewModel);
        }

        public ActionResult Config()
        {
            var response = _derService.GetDers();
            var viewModel = new DerConfigViewModel();
            viewModel.Ders = response.Ders.MapTo<DerViewModel>();
            return View(viewModel);
        }

        public ActionResult Create()
        {
            var viewModel = new DerViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(DerViewModel viewModel)
        {
            var request = viewModel.MapTo<CreateOrUpdateDerRequest>();
            var response = _derService.CreateOrUpdate(request);
            if (response.IsSuccess)
            {
                return RedirectToAction("Config");
            }

            return base.ErrorPage(response.Message);
        }

        public ActionResult ManageItem(ManageDerItemViewModel viewModel)
        {
            var request = viewModel.MapTo<GetDerItemRequest>();
            var response = _derService.GetDerItem(request);
            var manageDerItemViewModel = response.MapTo<ManageDerItemViewModel>();
            manageDerItemViewModel.Types = _dropdownService.GetDerItemTypes().MapTo<SelectListItem>();
            if (response.IsSuccess)
            {
                return PartialView("_ManageItem", manageDerItemViewModel);
            }
            else
            {
                return base.ErrorPage(response.Message);
            }
        }

        public ActionResult Display(int id)
        {

            var response = _derService.GetDerLayout(id);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<DerDisplayViewModel>();
                return View(viewModel);
            }


            return base.ErrorPage(response.Message);
        }

        public ActionResult LayoutItem(int id, string currentDate)
        {
            DateTime date;
            bool isDate = DateTime.TryParse(currentDate, out date);
            var layout = _derService.GetDerLayoutItem(id);
            switch (layout.Type.ToLowerInvariant())
            {
                #region line
                case "line":
                    {

                        var request = new GetCartesianChartDataRequest();
                        request.Start = date.AddDays(-7);
                        request.End = date;
                        request.HeaderTitle = layout.Artifact.HeaderTitle;
                        request.MeasurementId = layout.Artifact.MeasurementId;
                        request.PeriodeType = PeriodeType.Daily;
                        request.RangeFilter = RangeFilter.Interval;
                        request.ValueAxis = ValueAxis.KpiActual;

                        var series = layout.Artifact.Series.Select(x => new GetCartesianChartDataRequest.SeriesRequest
                            {
                                Color = x.Color,
                                KpiId = x.KpiId,
                                Label = x.Label
                            }).ToList();
                        request.Series = series;
                        var chartData = _artifactService.GetChartData(request);

                        var previewViewModel = new ArtifactPreviewViewModel();
                        previewViewModel.PeriodeType = "Daily";
                        previewViewModel.Highlights = new List<ArtifactPreviewViewModel.HighlightViewModel>();
                        for (DateTime counter = request.Start.Value;
                             counter <= request.End.Value;
                             counter = counter.AddDays(1))
                        {
                            previewViewModel.Highlights.Add(null);
                        }
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.GraphicType = layout.Type;
                        previewViewModel.LineChart = new LineChartDataViewModel();
                        previewViewModel.LineChart.Title = layout.Artifact.HeaderTitle;
                        previewViewModel.LineChart.Subtitle = chartData.Subtitle;
                        previewViewModel.LineChart.ValueAxisTitle = layout.Artifact.MeasurementName;
                        previewViewModel.LineChart.Series =
                            chartData.Series.MapTo<LineChartDataViewModel.SeriesViewModel>();
                        previewViewModel.LineChart.Periodes = chartData.Periodes;
                        return Json(previewViewModel, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region multiaxis
                case "multiaxis":
                    {
                        var request = new GetMultiaxisChartDataRequest();
                        request.PeriodeType = PeriodeType.Daily;
                        request.RangeFilter = RangeFilter.Interval;
                        request.Start = date.AddDays(-7);
                        request.End = date;

                        var previewViewModel = new ArtifactPreviewViewModel();

                        request.Charts = layout.Artifact.Charts.MapTo<GetMultiaxisChartDataRequest.ChartRequest>();
                        var chartData = _artifactService.GetMultiaxisChartData(request);
                        previewViewModel.PeriodeType = "Daily";
                        previewViewModel.TimePeriodes = chartData.TimePeriodes;
                        previewViewModel.Highlights = new List<ArtifactPreviewViewModel.HighlightViewModel>();
                        for (DateTime counter = request.Start.Value;
                             counter <= request.End.Value;
                             counter = counter.AddDays(1))
                        {
                            previewViewModel.Highlights.Add(null);
                        }
                        previewViewModel.GraphicType = layout.Type;
                        previewViewModel.MultiaxisChart = new MultiaxisChartDataViewModel();
                        chartData.MapPropertiesToInstance<MultiaxisChartDataViewModel>(previewViewModel.MultiaxisChart);
                        previewViewModel.MultiaxisChart.Title = layout.Artifact.HeaderTitle;
                        return Json(previewViewModel, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region pie
                case "pie":
                    {
                        var request = new GetPieDataRequest();
                        request.PeriodeType = PeriodeType.Daily;
                        request.RangeFilter = RangeFilter.Interval;
                        request.Start = date.AddDays(-7);
                        request.End = date;
                        request.HeaderTitle = layout.Artifact.HeaderTitle;

                        request.ValueAxis = ValueAxis.KpiActual;

                        var series = layout.Artifact.Series.Select(x => new GetPieDataRequest.SeriesRequest
                        {
                            Color = x.Color,
                            KpiId = x.KpiId,
                            Label = x.Label
                        }).ToList();
                        request.Series = series;
                        var chartData = _artifactService.GetPieData(request);

                        var previewViewModel = new ArtifactPreviewViewModel();
                        previewViewModel.PeriodeType = "Daily";
                        previewViewModel.Highlights = new List<ArtifactPreviewViewModel.HighlightViewModel>();
                        for (DateTime counter = request.Start.Value;
                             counter <= request.End.Value;
                             counter = counter.AddDays(1))
                        {
                            previewViewModel.Highlights.Add(null);
                        }

                        previewViewModel.GraphicType = layout.Type;
                        previewViewModel.Pie = chartData.MapTo<PieDataViewModel>();
                        previewViewModel.Pie.Is3D = layout.Artifact.Is3D;
                        previewViewModel.Pie.ShowLegend = layout.Artifact.ShowLegend;
                        previewViewModel.Pie.Title = layout.Artifact.HeaderTitle;
                        previewViewModel.Pie.Subtitle = chartData.Subtitle;
                        previewViewModel.Pie.SeriesResponses =
                            chartData.SeriesResponses.MapTo<PieDataViewModel.SeriesResponse>();

                        return Json(previewViewModel, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region tank
                case "tank":
                    {
                        var request = new GetTankDataRequest();
                        request.PeriodeType = PeriodeType.Daily;
                        request.RangeFilter = RangeFilter.Interval;
                        request.Start = date.AddDays(-7);
                        request.End = date;
                        request.Tank = layout.Artifact.Tank.MapTo<GetTankDataRequest.TankRequest>();
                        var previewViewModel = new ArtifactPreviewViewModel();
                        var chartData = _artifactService.GetTankData(request);
                        previewViewModel.GraphicType = layout.Artifact.GraphicType;
                        previewViewModel.Tank = new TankDataViewModel();
                        chartData.MapPropertiesToInstance<TankDataViewModel>(previewViewModel.Tank);
                        previewViewModel.Tank.Title = layout.Artifact.HeaderTitle;
                        previewViewModel.Tank.Subtitle = chartData.Subtitle;
                        previewViewModel.Tank.Id = layout.Artifact.Tank.Id;
                        return Json(previewViewModel, JsonRequestBehavior.AllowGet);
                    }
                #endregion

                case "highlight":
                    {
                        var highlight =
                            _highlightService.GetHighlightByPeriode(new GetHighlightRequest
                                {
                                    Date = date,
                                    HighlightTypeId = layout.Highlight.SelectOptionId
                                });
                        var view = RenderPartialViewToString("Display/_Highlight", highlight);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                case "weather":
                    {
                        var weather = _weatherService.GetWeather(new GetWeatherRequest
                            {
                                Date = date,
                                ByDate = true
                            });

                        var view = RenderPartialViewToString("Display/_Weather", weather);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                case "alert":
                    {
                        var alert = _highlightService.GetHighlightByPeriode(new GetHighlightRequest
                            {
                                Type = "Alert",
                                Date = date
                            });
                        var view = RenderPartialViewToString("Display/_Alert", alert);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }

                case "avg-ytd-key-statistic":
                    {
                        var viewModel = new DisplayAvgYtdKeyStatisticViewModel();

                        for (int i = 1; i <= 6; i++)
                        {
                            var avgYtdKeyStatisticViewModel = new DisplayAvgYtdKeyStatisticViewModel.AvgYtdKeyStatisticViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                       new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };

                            avgYtdKeyStatisticViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var actual = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.YTD, YtdFormula.Average);
                                avgYtdKeyStatisticViewModel.KpiName = item.Kpi.Name;
                                avgYtdKeyStatisticViewModel.Value = actual.Value.HasValue ? actual.Value.ToString() : "n/a";
                            }

                            viewModel.AvgYtdKeyStatistics.Add(avgYtdKeyStatisticViewModel);
                        }

                        var view = RenderPartialViewToString("Display/_AvgYtdKeyStatistic", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }

                case "safety-table":
                    {
                        var viewModel = new DisplaySafetyTableViewModel();

                        for (int i = 1; i <= 9; i++)
                        {
                            var safetyTableViewModel = new DisplaySafetyTableViewModel.SafetyTableViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                       new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };

                            safetyTableViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var currentDay = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.CurrentDay);
                                var mtd = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.MTD);
                                var ytd = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.YTD);
                                var targetYearly = _kpiTargetService.GetKpiTarget(item.Kpi.Id, date, RangeFilter.CurrentYear);
                                var itd = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.AllExistingYears);
                                safetyTableViewModel.KpiName = item.Kpi.Name;
                                safetyTableViewModel.CurrentDay = GetDoubleToString(currentDay.Value);
                                safetyTableViewModel.Mtd = GetDoubleToString(mtd.Value);
                                safetyTableViewModel.Ytd = GetDoubleToString(ytd.Value);
                                safetyTableViewModel.AnnualTarget = GetDoubleToString(targetYearly.Value);
                                safetyTableViewModel.Itd = GetDoubleToString(itd.Value);
                            }

                            viewModel.SafetyTableViewModels.Add(safetyTableViewModel);
                        }

                        var view = RenderPartialViewToString("Display/_SafetyTable", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }

            }
            return Content("as");
        }

        private string GetDoubleToString(double? val)
        {
            return val.HasValue ? val.Value.ToString() : "n/a";
        }
    }
}