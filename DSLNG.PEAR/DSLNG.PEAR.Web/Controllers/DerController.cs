using System;
using System.Collections.Generic;
using System.Globalization;
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
using DSLNG.PEAR.Web.Extensions;

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
                #region highlight
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
                #endregion
                #region weather
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
                #endregion
                #region alert
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
                #endregion
                #region avg ytd key statistic
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
                                avgYtdKeyStatisticViewModel.Ytd = actual.Value.HasValue ? actual.Value.ToString() : "n/a";
                            }

                            viewModel.AvgYtdKeyStatistics.Add(avgYtdKeyStatisticViewModel);
                        }

                        var view = RenderPartialViewToString("Display/_AvgYtdKeyStatistic", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region safety
                case "safety":
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
                #endregion
                #region security
                case "security":
                    {
                        var viewModel = new DisplaySecurityViewModel();

                        for (int i = 1; i <= 6; i++)
                        {
                            var securityViewModel = new DisplaySecurityViewModel.SecurityViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                       new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };

                            securityViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var actual = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.CurrentDay);
                                securityViewModel.KpiName = item.Kpi.Name;
                                securityViewModel.Value = actual.Value.HasValue ? actual.Value.ToString() : "n/a";
                            }

                            viewModel.SecurityViewModels.Add(securityViewModel);
                        }

                        var view = RenderPartialViewToString("Display/_Security", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region lng and cds
                case "lng-and-cds":
                    {
                        var viewModel = new DisplayLngAndCdsViewModel();
                        bool isPlannedCargoes = false;
                        for (int i = 1; i <= 12; i++)
                        {
                            var lngAndCdsViewModel = new DisplayLngAndCdsViewModel.LngAndCdsViewModel();
                            if (i <= 4 || i >= 9)
                            {
                                var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                           new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };

                                lngAndCdsViewModel.Position = item.Position;
                                if (item.Kpi != null)
                                {
                                    lngAndCdsViewModel.KpiName = item.Kpi.Name;
                                    var actualMtd = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.MTD);
                                    lngAndCdsViewModel.ActualMtd = actualMtd.Value.ToStringFromNullableDouble("n/a");
                                    var targetMtd = _kpiTargetService.GetKpiTarget(item.Kpi.Id, date, RangeFilter.MTD);
                                    lngAndCdsViewModel.TargetYtd = targetMtd.Value.ToStringFromNullableDouble("n/a");
                                    var actualYtd = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.YTD);
                                    lngAndCdsViewModel.ActualYtd = actualYtd.Value.ToStringFromNullableDouble("n/a");
                                    var targetYtd = _kpiTargetService.GetKpiTarget(item.Kpi.Id, date, RangeFilter.MTD);
                                    lngAndCdsViewModel.TargetYtd = targetYtd.Value.ToStringFromNullableDouble("n/a");
                                }


                            }
                            else
                            {
                                if (!isPlannedCargoes)
                                {
                                    var mtdList = new List<string>();
                                    var ytdList = new List<string>();

                                    var list = layout.KpiInformations.Where(x => x.Position == 5).ToList();
                                    foreach (var item in list)
                                    {
                                        string mtd, ytd = string.Empty;
                                        if (item.Kpi != null)
                                        {
                                            var actualMtd = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.MTD);
                                            var targetMtd = _kpiTargetService.GetKpiTarget(item.Kpi.Id, date, RangeFilter.MTD);
                                            var actualYtd = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.MTD);
                                            var targetYtd = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, RangeFilter.MTD);
                                            mtd = string.Format(@"{0}/{1} {2}", actualMtd.Value.ToStringFromNullableDouble("-"), targetMtd.Value.ToStringFromNullableDouble("-"), item.Kpi.Name);
                                            ytd = string.Format(@"{0}/{1} {2}", actualYtd.Value.ToStringFromNullableDouble("-"), targetYtd.Value.ToStringFromNullableDouble("-"), item.Kpi.Name);
                                            mtdList.Add(mtd);
                                            ytdList.Add(ytd);
                                        }
                                    }

                                    lngAndCdsViewModel.KpiName = "Planned Cargoes";
                                    lngAndCdsViewModel.ActualMtd = string.Join(",", mtdList);
                                    lngAndCdsViewModel.ActualMtd = string.Join(",", ytdList);
                                    lngAndCdsViewModel.Position = 5;
                                    isPlannedCargoes = true;
                                }
                            }

                            viewModel.DisplayLngAndCds.Add(lngAndCdsViewModel);
                        }

                        var view = RenderPartialViewToString("Display/_LngAndCds", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region dafwc
                case "dafwc":
                    {
                        var viewModel = new DisplayDafwcViewModel();

                        var response = _derService.GetDafwcData(id, date);
                        viewModel.DaysWithoutDafwc = response.DaysWithoutDafwc;
                        viewModel.DaysWithoutLopc = response.DaysWithoutLopc;
                        viewModel.DaysWithoutDafwcSince = response.DaysWithoutDafwcSince;
                        viewModel.DaysWithoutLopcSince = response.DaysWithoutLopcSince;
                        var view = RenderPartialViewToString("Display/_Dafwc", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region job pmts
                case "job-pmts":
                    {
                        var viewModel = new DisplayJobPmtsViewModel();

                        for (int i = 0; i <= 5; i++)
                        {
                            var jobPmtsViewModel = new DisplayJobPmtsViewModel.JobPmtsViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                       new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };

                            jobPmtsViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.KpiId = item.Kpi.Id;
                                request.ConfigType = item.ConfigType;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentDay;
                                var daily = _derService.GetKpiValue(request);
                                jobPmtsViewModel.KpiName = item.Kpi.Name;
                                jobPmtsViewModel.Measurement = item.Kpi.MeasurementName;
                                jobPmtsViewModel.Daily = daily.Value.HasValue ? daily.Value.Value.ToString() : "n/a";

                                request.RangeFilter = i < 2 ? RangeFilter.MTD : RangeFilter.CurrentMonth;
                                var mtd = _derService.GetKpiValue(request);
                                jobPmtsViewModel.Mtd = mtd.Value.HasValue ? mtd.Value.Value.ToString() : "n/a";

                                request.RangeFilter = i < 2 ? RangeFilter.YTD : RangeFilter.CurrentYear;
                                var ytd = _derService.GetKpiValue(request);
                                jobPmtsViewModel.Ytd = ytd.Value.HasValue ? ytd.Value.Value.ToString() : "n/a";


                                /*double dailyValue = (daily.Value.HasValue) ? daily.Value.Value : 0;
                                double mtdValue = (mtd.Value.HasValue) ? mtd.Value.Value : 0;
                                double ytdValue = (ytd.Value.HasValue) ? ytd.Value.Value : 0;*/


                            }

                            viewModel.JobPmtsViewModels.Add(jobPmtsViewModel);
                        }

                        var view = RenderPartialViewToString("Display/_JobPmts", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region total feed gas
                case "total-feed-gas":
                    {
                        var viewModel = new DisplayTotalFeedGasViewModel();
                        for (int i = 0; i <= 3; i++)
                        {
                            var totalFeedGasViewModel = new DisplayTotalFeedGasViewModel.TotalFeedGasViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                      new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                            totalFeedGasViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.ConfigType = item.ConfigType;
                                request.KpiId = item.Kpi.Id;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentDay;
                                var daily = _derService.GetKpiValue(request);
                                totalFeedGasViewModel.Daily = daily.Value.HasValue ? daily.Value.Value.ToString() : "n/a";
                                request.RangeFilter = RangeFilter.MTD;
                                var mtd = _derService.GetKpiValue(request);
                                totalFeedGasViewModel.Mtd = mtd.Value.HasValue ? mtd.Value.Value.ToString() : "n/a";
                                request.RangeFilter = RangeFilter.YTD;
                                var ytd = _derService.GetKpiValue(request);
                                totalFeedGasViewModel.Ytd = ytd.Value.HasValue ? ytd.Value.Value.ToString() : "n/a";
                            }
                            viewModel.TotalFeedGasViewModels.Add(totalFeedGasViewModel);
                        }
                        var view = RenderPartialViewToString("Display/_TotalFeedGas", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region table tank
                case "table-tank":
                    {
                        var viewModel = new DisplayTableTankViewModel();
                        for (int i = 0; i <= 10; i++)
                        {
                            var totalTableTankViewModel = new DisplayTableTankViewModel.TableTankViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                      new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                            totalTableTankViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.ConfigType = item.ConfigType;
                                request.KpiId = item.Kpi.Id;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentDay;
                                var daily = _derService.GetKpiValue(request);
                                totalTableTankViewModel.Daily = daily.Value.HasValue ? daily.Value.Value.ToString() : "n/a";
                            }
                            viewModel.TableTankViewModels.Add(totalTableTankViewModel);
                        }
                        var view = RenderPartialViewToString("Display/_TableTank", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region MGDP
                case "mgdp":
                    {
                        var viewModel = new DisplayMGDPViewModel();

                        for (int i = 0; i <= 5; i++)
                        {
                            var MGDPViewModel = new DisplayMGDPViewModel.MGDPViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                       new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };

                            MGDPViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.KpiId = item.Kpi.Id;
                                request.ConfigType = item.ConfigType;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentDay;
                                var daily = _derService.GetKpiValue(request);
                                MGDPViewModel.KpiName = item.Kpi.Name;
                                MGDPViewModel.Measurement = item.Kpi.MeasurementName;
                                MGDPViewModel.Daily = daily.Value.HasValue ? daily.Value.Value.ToString() : "n/a";

                                request.RangeFilter = RangeFilter.MTD;
                                var mtd = _derService.GetKpiValue(request);
                                MGDPViewModel.Mtd = mtd.Value.HasValue ? mtd.Value.Value.ToString() : "n/a";

                                request.RangeFilter = RangeFilter.YTD;
                                var ytd = _derService.GetKpiValue(request);
                                MGDPViewModel.Ytd = ytd.Value.HasValue ? ytd.Value.Value.ToString() : "n/a";


                                /*double dailyValue = (daily.Value.HasValue) ? daily.Value.Value : 0;
                                double mtdValue = (mtd.Value.HasValue) ? mtd.Value.Value : 0;
                                double ytdValue = (ytd.Value.HasValue) ? ytd.Value.Value : 0;*/


                            }

                            viewModel.MGDPViewModels.Add(MGDPViewModel);
                        }

                        var view = RenderPartialViewToString("Display/_MGDP", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region HHV
                case "hhv":
                    {
                        var viewModel = new DisplayHHVViewModel();
                        for (int i = 0; i <= 3; i++)
                        {
                            var totalHHVViewModel = new DisplayHHVViewModel.HHVViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                      new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                            totalHHVViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.ConfigType = item.ConfigType;
                                request.KpiId = item.Kpi.Id;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentDay;
                                var daily = _derService.GetKpiValue(request);
                                totalHHVViewModel.Daily = daily.Value.HasValue ? daily.Value.Value.ToString() : "n/a";
                            }
                            viewModel.HHVViewModels.Add(totalHHVViewModel);
                        }
                        var view = RenderPartialViewToString("Display/_HHV", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region LNG and CDS Production
                case "lng-and-cds-production":
                    {
                        var viewModel = new DisplayLngAndCdsProductionViewModel();

                        for (int i = 0; i <= 8; i++)
                        {
                            var MGDPViewModel = new DisplayLngAndCdsProductionViewModel.LngAndCdsProductionViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                       new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };

                            MGDPViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.KpiId = item.Kpi.Id;
                                request.ConfigType = item.ConfigType;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentDay;
                                var daily = _derService.GetKpiValue(request);
                                MGDPViewModel.KpiName = item.Kpi.Name;
                                MGDPViewModel.Measurement = item.Kpi.MeasurementName;
                                MGDPViewModel.Daily = daily.Value.HasValue ? daily.Value.Value.ToString() : "n/a";

                                request.RangeFilter = RangeFilter.MTD;
                                var mtd = _derService.GetKpiValue(request);
                                MGDPViewModel.Mtd = mtd.Value.HasValue ? mtd.Value.Value.ToString() : "n/a";

                                request.RangeFilter = RangeFilter.YTD;
                                var ytd = _derService.GetKpiValue(request);
                                MGDPViewModel.Ytd = ytd.Value.HasValue ? ytd.Value.Value.ToString() : "n/a";


                                /*double dailyValue = (daily.Value.HasValue) ? daily.Value.Value : 0;
                                double mtdValue = (mtd.Value.HasValue) ? mtd.Value.Value : 0;
                                double ytdValue = (ytd.Value.HasValue) ? ytd.Value.Value : 0;*/


                            }

                            viewModel.LngAndCdsProductionViewModels.Add(MGDPViewModel);
                        }

                        var view = RenderPartialViewToString("Display/_LngAndCdsProduction", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                    #endregion
                #region weekly maintenance
                case "weekly-maintenance":
                    {
                        var viewModel = new DisplayWeeklyMaintenanceViewModel();
                        DateTime lastWednesday = date;
                        while (lastWednesday.DayOfWeek != DayOfWeek.Wednesday)
                            lastWednesday = lastWednesday.AddDays(-1);
                        viewModel.Date = lastWednesday.ToString("dd MMM yyyy");
                        for (int i = 0; i <= 3; i++)
                        {
                            var weeklyMaintenanceViewModel = new DisplayWeeklyMaintenanceViewModel.WeeklyMaintenanceViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ?? 
                                new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                            weeklyMaintenanceViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.ConfigType = item.ConfigType;
                                request.KpiId = item.Kpi.Id;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentWeek;
                                var weekly = _derService.GetKpiValue(request);
                                weeklyMaintenanceViewModel.Weekly = weekly.Value.HasValue && weekly.Value != null ? weekly.Value.Value.ToString() : "n/a"; ;
                            } 
                            else if (item.SelectOption != null)
                            {
                                var highlight = _highlightService.GetHighlightByPeriode(new GetHighlightRequest
                                    {
                                        Date = date,
                                        HighlightTypeId = item.SelectOption.Id
                                    });
                                weeklyMaintenanceViewModel.Remarks = !string.IsNullOrEmpty(highlight.Message)
                                                                         ? highlight.Message
                                                                         : "n/a";
                            }
                            viewModel.WeeklyMaintenanceViewModels.Add(weeklyMaintenanceViewModel);
                        }
                        var view = RenderPartialViewToString("Display/_WeeklyMaintenance", viewModel);
                        var json = new {type = layout.Type.ToLowerInvariant(), view};
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region critical PM
                case "critical-pm":
                    {
                        var viewModel = new DisplayCriticalPmViewModel();
                        for (int i = 0; i <= 4; i++)
                        {
                            var criticalViewModel = new DisplayCriticalPmViewModel.CriticalPmViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                            criticalViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.ConfigType = item.ConfigType;
                                request.KpiId = item.Kpi.Id;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentWeek;
                                var weekly = _derService.GetKpiValue(request);
                                criticalViewModel.Weekly = weekly.Value.HasValue && weekly.Value != null ? weekly.Value.Value.ToString() : "n/a"; ;
                            }
                            else if (item.SelectOption != null)
                            {
                                var highlight = _highlightService.GetHighlightByPeriode(new GetHighlightRequest
                                {
                                    Date = date,
                                    HighlightTypeId = item.SelectOption.Id
                                });
                                criticalViewModel.Remarks = !string.IsNullOrEmpty(highlight.Message)
                                                                         ? highlight.Message
                                                                         : "n/a";
                            }
                            viewModel.CriticalPmViewModels.Add(criticalViewModel);
                        }
                        var view = RenderPartialViewToString("Display/_CriticalPm", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region procurement
                case "procurement":
                    {
                        var viewModel = new DisplayProcurementViewModel();
                        for (int i = 0; i <= 3; i++)
                        {
                            var procureMentViewModel = new DisplayProcurementViewModel.ProcurementViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                            procureMentViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.ConfigType = item.ConfigType;
                                request.KpiId = item.Kpi.Id;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentDay;
                                var daily = _derService.GetKpiValue(request);
                                procureMentViewModel.Daily = daily.Value.HasValue && daily.Value != null ? daily.Value.Value.ToString() : "n/a"; ;
                            }
                            else if (item.SelectOption != null)
                            {
                                var highlight = _highlightService.GetHighlightByPeriode(new GetHighlightRequest
                                {
                                    Date = date,
                                    HighlightTypeId = item.SelectOption.Id
                                });
                                procureMentViewModel.Remarks = !string.IsNullOrEmpty(highlight.Message)
                                                                         ? highlight.Message
                                                                         : "n/a";
                            }
                            viewModel.ProcurementViewModels.Add(procureMentViewModel);
                        }
                        var view = RenderPartialViewToString("Display/_Procurement", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Indicative Commercial Price
                case "indicative-commercial-price":
                    {
                        var viewModel = new DisplayIndicativeCommercialPriceViewModel();
                        for (int i = 0; i <= 3; i++)
                        {
                            var indicativeCommercialPriceViewModel = new DisplayIndicativeCommercialPriceViewModel.IndicativeCommercialPriceViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                      new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                            indicativeCommercialPriceViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.ConfigType = item.ConfigType;
                                request.KpiId = item.Kpi.Id;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentDay;
                                var daily = _derService.GetKpiValue(request);
                                indicativeCommercialPriceViewModel.Daily = daily.Value.HasValue ? daily.Value.Value.ToString() : "n/a";
                            }
                            viewModel.IndicativeCommercialPriceViewModels.Add(indicativeCommercialPriceViewModel);
                        }
                        var view = RenderPartialViewToString("Display/_IndicativeCommercialPrice", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Plant Availability
                case "plant-availability":
                    {
                        var viewModel = new DisplayPlantAvailabilityViewModel();

                        for (int i = 0; i < 10; i++)
                        {
                            var MGDPViewModel = new DisplayPlantAvailabilityViewModel.PlantAvailabilityViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                       new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };

                            MGDPViewModel.Position = item.Position;
                            if (item.Kpi != null)
                            {
                                var request = new GetKpiValueRequest();
                                request.KpiId = item.Kpi.Id;
                                request.ConfigType = item.ConfigType;
                                request.Periode = date;
                                request.RangeFilter = RangeFilter.CurrentDay;
                                var daily = _derService.GetKpiValue(request);
                                MGDPViewModel.KpiName = item.Kpi.Name;
                                MGDPViewModel.Measurement = item.Kpi.MeasurementName;
                                MGDPViewModel.Daily = daily.Value.HasValue ? daily.Value.Value.ToString() : "n/a";

                                request.RangeFilter = RangeFilter.MTD;
                                var mtd = _derService.GetKpiValue(request);
                                MGDPViewModel.Mtd = mtd.Value.HasValue ? mtd.Value.Value.ToString() : "n/a";

                                request.RangeFilter = RangeFilter.YTD;
                                var ytd = _derService.GetKpiValue(request);
                                MGDPViewModel.Ytd = ytd.Value.HasValue ? ytd.Value.Value.ToString() : "n/a";


                                /*double dailyValue = (daily.Value.HasValue) ? daily.Value.Value : 0;
                                double mtdValue = (mtd.Value.HasValue) ? mtd.Value.Value : 0;
                                double ytdValue = (ytd.Value.HasValue) ? ytd.Value.Value : 0;*/


                            }

                            viewModel.PlantAvailabilityViewModels.Add(MGDPViewModel);
                        }

                        var view = RenderPartialViewToString("Display/_PlantAvalability", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                    #endregion
                    

            }
            return Content("Switch case does not matching");
        }

        public ActionResult OriginalData(int id, string currentDate)
        {
            DateTime date;
            bool isDate = DateTime.TryParse(currentDate, out date);
            if (!isDate)
            {
                return base.ErrorPage("Original Data should have date parameter");
            }
            var response = _derService.GetOriginalData(id, date);
            var viewModel = response.MapTo<DisplayOriginalDataViewModel>();
            viewModel.Id = id;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult OriginalData(DisplayOriginalDataViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveOriginalDataRequest>();
            var response = _derService.SaveOriginalData(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("OriginalData", new { id = viewModel.Id, currentDate = viewModel.CurrentDate });
        }

        private string GetDoubleToString(double? val)
        {
            return val.HasValue ? val.Value.ToString(CultureInfo.InvariantCulture) : "n/a";
        }
    }
}