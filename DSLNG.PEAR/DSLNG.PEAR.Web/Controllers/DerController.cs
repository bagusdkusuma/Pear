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
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Services.Requests.Wave;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Services.Responses.Der;
using DSLNG.PEAR.Web.ViewModels.Artifact;
using DSLNG.PEAR.Web.ViewModels.Der;
using DSLNG.PEAR.Web.ViewModels.Der.Display;
using DSLNG.PEAR.Web.Extensions;
using DSLNG.PEAR.Web.ViewModels.Highlight;
using NReco.ImageGenerator;
using NReco.PdfGenerator;
using System.IO;
using System.Web.Script.Serialization;
using DSLNG.PEAR.Common.Contants;
using DSLNG.PEAR.Services.Requests.KpiTarget;
using DSLNG.PEAR.Web.Grid;

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
        private readonly IVesselScheduleService _vesselScheduleService;
        private readonly IWaveService _waveService;

        public DerController(IDerService derService, IDropdownService dropdownService, IArtifactService artifactService, IHighlightService highlightService, IWeatherService weatherService, IKpiAchievementService kpiAchievementService, IKpiTargetService kpiTargetService, IVesselScheduleService vesselScheduleService, IWaveService waveService)
        {
            _derService = derService;
            _dropdownService = dropdownService;
            _artifactService = artifactService;
            _highlightService = highlightService;
            _weatherService = weatherService;
            _kpiAchievementService = kpiAchievementService;
            _kpiTargetService = kpiTargetService;
            _vesselScheduleService = vesselScheduleService;
            _waveService = waveService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var ders = _derService.GetDers(new GetDersRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                Search = gridParams.Search,
                SortingDictionary = gridParams.SortingDictionary
            });
            IList<GetDersResponse.Der> DatasResponse = ders.Ders;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = ders.TotalRecords,
                iTotalRecords = ders.Ders.Count,
                aaData = DatasResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Index()
        //{
        //    var response = _derService.GetActiveDer();
        //    var viewModel = response.MapTo<DerIndexViewModel>();
        //    return View(viewModel);
        //}

        //public ActionResult Config()
        //{
        //    var response = _derService.GetDers();
        //    var viewModel = new DerConfigViewModel();
        //    viewModel.Ders = response.Ders.MapTo<DerViewModel>();
        //    return View(viewModel);
        //}

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
                #region avg ytd key statistic
                case "avg-ytd-key-statistic":
                    {
                        var view = RenderPartialViewToString("~/Views/Der/Display/_AvgYtdKeyStatistic.cshtml", GetGeneralDerKpiInformations(5, layout, date, PeriodeType.Daily));
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region line
                case "line":
                    {

                        var request = new GetCartesianChartDataRequest();
                        request.Start = date.AddDays(-6);
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
                        request.Start = date.AddDays(-6);
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
                        request.Start = date;
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
                        request.Start = date;
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
                #region speedometer
                case "barmeter":
                case "speedometer":
                    {
                        var request = new GetSpeedometerChartDataRequest();
                        request.Start = date;
                        request.End = date;
                        request.PeriodeType = PeriodeType.Daily;
                        request.RangeFilter = RangeFilter.Interval;
                        request.ValueAxis = ValueAxis.KpiActual;
                        request.PlotBands = layout.Artifact.Plots.Select(x => new GetSpeedometerChartDataRequest.PlotBandRequest
                        {
                            From = x.From,
                            Color = x.Color,
                            To = x.To
                        }).ToList();
                        request.Series = new GetSpeedometerChartDataRequest.SeriesRequest
                        {
                            KpiId = layout.Artifact.CustomSerie.Id,
                            Label = layout.Artifact.CustomSerie.Name
                        };

                        var chartData = _artifactService.GetSpeedometerChartData(request);

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
                        previewViewModel.SpeedometerChart = new SpeedometerChartDataViewModel();
                        previewViewModel.SpeedometerChart.Title = layout.Artifact.HeaderTitle;
                        previewViewModel.SpeedometerChart.Subtitle = chartData.Subtitle;
                        previewViewModel.SpeedometerChart.ValueAxisTitle = layout.Artifact.MeasurementName;
                        previewViewModel.SpeedometerChart.Series = chartData.Series.MapTo<SpeedometerChartDataViewModel.SeriesViewModel>();
                        previewViewModel.SpeedometerChart.PlotBands = chartData.PlotBands.MapTo<SpeedometerChartDataViewModel.PlotBandViewModel>();
                        previewViewModel.SpeedometerChart.PlotBands = previewViewModel.SpeedometerChart.PlotBands.OrderBy(x => x.to).ToList();
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
                        var view = RenderPartialViewToString("~/Views/Der/Display/_Highlight.cshtml", highlight);
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

                        var view = RenderPartialViewToString("~/Views/Der/Display/_Weather.cshtml", weather);
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
                        var view = RenderPartialViewToString("~/Views/Der/Display/_Alert.cshtml", alert);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region wave
                case "wave":
                    {
                        var wave = _waveService.GetWave(new GetWaveRequest
                        {
                            Date = date,
                            ByDate = true
                        });
                        var view = RenderPartialViewToString("~/Views/Der/Display/_Wave.cshtml", wave);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Next Loading Schedule
                case "nls":
                    {
                        var vesselSchedule = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest
                        {
                            allActiveList = true,
                            Skip = 0,
                            Take = 3,
                        });
                        var schedules = vesselSchedule.VesselSchedules.OrderByDescending(x => x.ETA).Take(3).ToList().OrderBy(x => x.ETA).ToList();
                        var nls = schedules.MapTo<DailyExecutionReportViewModel.NLSViewModel>();

                        var view = RenderPartialViewToString("~/Views/Der/Display/_Nls.cshtml", nls);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region safety
                case "safety":
                    {
                        var viewModel = GetGeneralDerKpiInformations(11, layout, date, PeriodeType.Daily);
                        var target0 = layout.KpiInformations.SingleOrDefault(x => x.Position == 0);
                        var target1 = layout.KpiInformations.SingleOrDefault(x => x.Position == 1);
                        var target2 = layout.KpiInformations.SingleOrDefault(x => x.Position == 2);
                        var target3 = layout.KpiInformations.SingleOrDefault(x => x.Position == 3);
                        var target4 = layout.KpiInformations.SingleOrDefault(x => x.Position == 4);
                        var target5 = layout.KpiInformations.SingleOrDefault(x => x.Position == 5);
                        var target6 = layout.KpiInformations.SingleOrDefault(x => x.Position == 6);
                        var target7 = layout.KpiInformations.SingleOrDefault(x => x.Position == 7);
                        var target8 = layout.KpiInformations.SingleOrDefault(x => x.Position == 8);
                        var target9 = layout.KpiInformations.SingleOrDefault(x => x.Position == 9);
                        var target10 = layout.KpiInformations.SingleOrDefault(x => x.Position == 10);
                        viewModel.KpiInformationViewModels.Add(AddTarget(11, target0, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(12, target1, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(13, target2, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(14, target3, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(15, target4, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(16, target5, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(17, target6, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(18, target7, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(19, target8, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(20, target9, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(21, target10, date));

                        var view = RenderPartialViewToString("~/Views/Der/Display/_SafetyTable.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region security
                case "security":
                    {
                        var view = RenderPartialViewToString("~/Views/Der/Display/_Security.cshtml", GetGeneralDerKpiInformations(6, layout, date, PeriodeType.Daily));
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region lng and cds
                case "lng-and-cds":
                    {

                        var viewModel = GetGeneralDerKpiInformations(17, layout, date, PeriodeType.Daily);
                        var target4 = layout.KpiInformations.SingleOrDefault(x => x.Position == 4);
                        var target5 = layout.KpiInformations.SingleOrDefault(x => x.Position == 5);
                        var target6 = layout.KpiInformations.SingleOrDefault(x => x.Position == 6);
                        var target7 = layout.KpiInformations.SingleOrDefault(x => x.Position == 7);
                        var target8 = layout.KpiInformations.SingleOrDefault(x => x.Position == 8);
                        var target9 = layout.KpiInformations.SingleOrDefault(x => x.Position == 9);
                        viewModel.KpiInformationViewModels.Add(AddTarget(17, target4, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(18, target5, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(19, target6, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(20, target7, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(21, target8, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(22, target9, date));
                        var view = RenderPartialViewToString("~/Views/Der/Display/_LngAndCds.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    };
                #endregion
                #region dafwc
                case "dafwc":
                    {
                        var viewModel = new DisplayKpiInformationViewModel();

                        for (int i = 0; i < 3; i++)
                        {
                            var kpiInformationVm = new DisplayKpiInformationViewModel.KpiInformationViewModel { Position = i };
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                       new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                            if (item.Kpi != null)
                            {
                                kpiInformationVm = item.MapTo<DisplayKpiInformationViewModel.KpiInformationViewModel>();
                                if (item.ConfigType.Equals(ConfigType.KpiAchievement))
                                {
                                    var achievement = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, PeriodeType.Daily);
                                    kpiInformationVm.DerItemValue = achievement.MapTo<DerItemValueViewModel>();
                                }
                                else if (item.ConfigType.Equals(ConfigType.KpiTarget))
                                {
                                    var achievement = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, PeriodeType.Daily);
                                    kpiInformationVm.DerItemValue = achievement.MapTo<DerItemValueViewModel>();
                                }
                            }

                            viewModel.KpiInformationViewModels.Add(kpiInformationVm);
                        }
                        var view = RenderPartialViewToString("~/Views/Der/Display/_Dafwc.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region job pmts
                case "job-pmts":
                    {

                        var viewModel = GetGeneralDerKpiInformations(3, layout, date, PeriodeType.Daily);
                        var target1 = layout.KpiInformations.SingleOrDefault(x => x.Position == 1);
                        viewModel.KpiInformationViewModels.Add(AddTarget(3, target1, date));
                        var view = RenderPartialViewToString("~/Views/Der/Display/_JobPmts.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region total feed gas
                case "total-feed-gas":
                    {
                        var viewModel = GetGeneralDerKpiInformations(3, layout, date, PeriodeType.Daily);
                        var view = RenderPartialViewToString("~/Views/Der/Display/_TotalFeedGas.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region table tank
                case "table-tank":
                    {
                        /*var viewModel = new DisplayTableTankViewModel();
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
                        }*/
                        var viewModel = GetGeneralDerKpiInformations(12, layout, date, PeriodeType.Daily);
                        var view = RenderPartialViewToString("~/Views/Der/Display/_TableTank.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region MGDP
                case "mgdp":
                    {
                        var viewModel = GetGeneralDerKpiInformations(3, layout, date, PeriodeType.Daily);// new DisplayKpiInformationViewModel();
                        var target = layout.KpiInformations.SingleOrDefault(x => x.Position == 1);
                        viewModel.KpiInformationViewModels.Add(AddTarget(3, target, date));
                        var view = RenderPartialViewToString("~/Views/Der/Display/_MGDP.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region HHV
                case "hhv":
                    {
                        var viewModel = GetGeneralDerKpiInformations(2, layout, date, PeriodeType.Daily);
                        var target0 = layout.KpiInformations.SingleOrDefault(x => x.Position == 0);
                        var target1 = layout.KpiInformations.SingleOrDefault(x => x.Position == 1);
                        viewModel.KpiInformationViewModels.Add(AddTarget(2, target0, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(3, target1, date));
                        var view = RenderPartialViewToString("~/Views/Der/Display/_HHV.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region LNG and CDS Production
                case "lng-and-cds-production":
                    {

                        var viewModel = GetGeneralDerKpiInformations(9, layout, date, PeriodeType.Daily);

                        var view = RenderPartialViewToString("~/Views/Der/Display/_LngAndCdsProduction.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region weekly maintenance
                case "weekly-maintenance":
                    {
                        /*var viewModel = new DisplayWeeklyMaintenanceViewModel();
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
                        }*/

                        var viewModel = GetGeneralDerKpiInformations(3, layout, date, PeriodeType.Daily);
                        var target2 = layout.KpiInformations.SingleOrDefault(x => x.Position == 2);
                        viewModel.KpiInformationViewModels.Add(AddTarget(3, target2, date));
                        var view = RenderPartialViewToString("~/Views/Der/Display/_WeeklyMaintenance.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region critical PM
                case "critical-pm":
                    {
                        /*var viewModel = new DisplayCriticalPmViewModel();
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
                        }*/

                        var viewModel = GetGeneralDerKpiInformations(4, layout, date, PeriodeType.Daily);
                        var view = RenderPartialViewToString("~/Views/Der/Display/_CriticalPm.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region procurement
                case "procurement":
                    {
                        /*var viewModel = new DisplayProcurementViewModel();
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
                        }*/

                        var viewModel = GetGeneralDerKpiInformations(2, layout, date, PeriodeType.Daily);
                        var view = RenderPartialViewToString("~/Views/Der/Display/_Procurement.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Indicative Commercial Price
                case "indicative-commercial-price":
                    {
                        /*var viewModel = new DisplayIndicativeCommercialPriceViewModel();
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
                        }*/

                        var viewModel = GetGeneralDerKpiInformations(3, layout, date, PeriodeType.Daily);
                        var view = RenderPartialViewToString("~/Views/Der/Display/_IndicativeCommercialPrice.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Plant Availability
                case "plant-availability":
                    {
                        var viewModel = GetGeneralDerKpiInformations(11, layout, date, PeriodeType.Daily);
                        var target0 = layout.KpiInformations.SingleOrDefault(x => x.Position == 0);
                        var target1 = layout.KpiInformations.SingleOrDefault(x => x.Position == 1);
                        var target2 = layout.KpiInformations.SingleOrDefault(x => x.Position == 2);
                        var target3 = layout.KpiInformations.SingleOrDefault(x => x.Position == 3);
                        viewModel.KpiInformationViewModels.Add(AddTarget(11, target0, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(12, target1, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(13, target2, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(14, target3, date));
                        var view = RenderPartialViewToString("~/Views/Der/Display/_PlantAvalability.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Economic Indicator
                case "economic-indicator":
                    {
                        var viewModel = GetGeneralDerKpiInformations(15, layout, date, PeriodeType.Daily);
                        var view = RenderPartialViewToString("~/Views/Der/Display/_EconomicIndicator.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Key Equipment Status
                case "key-equipment-status":
                    {
                        var viewModel = new DisplayKeyEquipmentStatusViewModel();
                        for (int i = 0; i <= 23; i++)
                        {
                            var keyEquipmentViewModel = new DisplayKeyEquipmentStatusViewModel.KeyEquipmentStatusViewModel();
                            var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                                new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                            keyEquipmentViewModel.Position = item.Position;
                            string message = "N/A";
                            if (item.SelectOption != null)
                            {
                                var request = new GetHighlightRequest();
                                request.Date = date;
                                request.HighlightTypeId = item.SelectOption.Id;

                                var highlight = _highlightService.GetHighlightByPeriode(request);
                                if (!string.IsNullOrEmpty(highlight.Message)) message = highlight.Message;
                            }

                            keyEquipmentViewModel.highlight = message;
                            viewModel.KeyEquipmentStatusViewModels.Add(keyEquipmentViewModel);
                        }
                        var view = RenderPartialViewToString("~/Views/Der/Display/_KeyEquipmentStatus.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Global Stock Market
                case "global-stock-market":
                    {
                        var viewModel = GetGeneralDerKpiInformations(13, layout, date, PeriodeType.Daily);
                        int highlightId = 0;
                        if (layout.KpiInformations.SingleOrDefault(x => x.Position == 8) != null && layout.KpiInformations.Single(x => x.Position == 8).SelectOption != null)
                        {
                            highlightId = layout.KpiInformations.Single(x => x.Position == 8).SelectOption.Id;
                        }
                        var highlight =
                            _highlightService.GetHighlightByPeriode(new GetHighlightRequest
                            {
                                Date = date,
                                HighlightTypeId = highlightId
                            });
                        viewModel.KpiInformationViewModels.Single(x => x.Position == 8).DerItemValue.Value =
                            highlight.Message;
                        var view = RenderPartialViewToString("~/Views/Der/Display/_GlobalStockMarket.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Loading Duration
                case "loading-duration":
                    {
                        var viewModel = GetGeneralDerKpiInformations(4, layout, date, PeriodeType.Daily);
                        var target0 = layout.KpiInformations.SingleOrDefault(x => x.Position == 0);
                        var target2 = layout.KpiInformations.SingleOrDefault(x => x.Position == 2);
                        viewModel.KpiInformationViewModels.Add(AddTarget(4, target0, date));
                        viewModel.KpiInformationViewModels.Add(AddTarget(5, target2, date));
                        var view = RenderPartialViewToString("~/Views/Der/Display/_LoadingDuration.cshtml", viewModel);
                        var json = new { type = layout.Type.ToLowerInvariant(), view };
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Termometer
                case "termometer": {
                        var viewModel = GetGeneralDerKpiInformations(1, layout, date, PeriodeType.Daily);
                        return Json(new { GraphicType = "termometer", Value = viewModel.KpiInformationViewModels[0].DerItemValue.Value }, JsonRequestBehavior.AllowGet);
                    }
                #endregion
                #region Person On Board
                case "person-on-board":
                    {
                        var viewModel = GetGeneralDerKpiInformations(1, layout, date, PeriodeType.Daily);
                        var view = RenderPartialViewToString("~/Views/Der/Display/_PersonOnBoard.cshtml", viewModel);
                        return Json(new { type = layout.Type.ToLowerInvariant(), view = view }, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult Input()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Preview()
        {
            var activeDer = _derService.GetActiveDer();
            var id = activeDer.Id;
            var response = _derService.GetDerLayout(id);
            var viewModel = response.MapTo<DerDisplayViewModel>();
            return View("Preview2", viewModel);
        }

        [HttpPost]
        public ActionResult Generate(string date)
        {
            var theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var secretNumber = Guid.NewGuid().ToString();
            DerImageController.SecretNumber = secretNumber;
            var displayUrl = Url.Action("Preview", "DerImage", new { secretNumber = secretNumber, date = theDate.ToString("MM/dd/yyyy") }, this.Request.Url.Scheme);
            var htmlToPdf = new HtmlToPdfConverter();
            htmlToPdf.Size = PageSize.A3;
            if (!Directory.Exists(Server.MapPath(PathConstant.DerPath)))
            {
                Directory.CreateDirectory(Server.MapPath(PathConstant.DerPath));
            }

            var htmlToImageConverter = new HtmlToImageConverter();
            htmlToImageConverter.Height = Convert.ToInt32(Math.Round(1908.00));
            htmlToImageConverter.Width = Convert.ToInt32(Math.Round(1349.00));
            var imageName = "der_" + theDate.Ticks + ".png";
            var imagePath = Path.Combine(Server.MapPath(PathConstant.DerPath), imageName);
            htmlToImageConverter.GenerateImageFromFile(displayUrl, ImageFormat.Png, imagePath);
            var htmlContent = String.Format("<body><img src='{0}' /></body>", Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content(PathConstant.DerPath + "/" + imageName));
            var title = "DER/" + theDate.ToString("dd-MMM-yyyy");
            //if (id != 0) {
            //    title = _derService.GetDerById(id).Title;
            //}
            var pdfName = title.Replace('/', '-') + ".pdf";
            var pdfPath = Path.Combine(Server.MapPath(PathConstant.DerPath), pdfName);
            htmlToPdf.Margins.Top = 10;
            htmlToPdf.Margins.Bottom = 10;
            htmlToPdf.Margins.Left = 10;
            htmlToPdf.Margins.Right = 10;
            htmlToPdf.GeneratePdf(htmlContent, null, pdfPath);
            //htmlToPdf.GeneratePdfFromFile(displayUrl, null, pdfPath);
            var response = _derService.CreateOrUpdate(new CreateOrUpdateDerRequest {
                Filename = PathConstant.DerPath + "/" + pdfName,
                Title = title,
                Date = DateTime.Now,
                RevisionBy = UserProfile().UserId
            });
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            return base.ErrorPage(response.Message);
            //htmlToPdf.GeneratePdfFromFile(displayUrl, null, pdfPath);
            //return File(pdfPath, "application/pdf");
            //var htmlToImageConverter = new HtmlToImageConverter();
            //htmlToImageConverter.Height = 2481;
            //htmlToImageConverter.Width = 1754;
            //return File(htmlToImageConverter.GenerateImageFromFile(displayUrl, ImageFormat.Png), "image/png", "TheGraph.png");
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase derFile, string date)
        {
            var theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            int revision = 0;
            var title = "DER/" + theDate.ToString("dd-MMM-yyyy");
            string filename = title.Replace('/', '-');
            var isExisted = _derService.IsDerExisted(theDate, out revision);
            revision = isExisted ? (revision+1) : revision;
            //if (_derService.IsDerExisted(theDate, out revision))
            //{
            //    filename = title + "_" + revision;
            //}

            //var pdfName = title.Replace('/', '-') + ".pdf";



            if (derFile.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath(PathConstant.DerPath), string.Format("{0}_{1}.pdf", filename, revision));
                derFile.SaveAs(path);

                var response = _derService.CreateOrUpdate(new CreateOrUpdateDerRequest
                {
                    Filename = filename,
                    Title = title,
                    Date = DateTime.Now,
                    RevisionBy = UserProfile().UserId
                });
            }
            return RedirectToAction("Index");
        }

        private DisplayKpiInformationViewModel GetGeneralDerKpiInformations(int numberOfKpi, GetDerLayoutitemResponse layout, DateTime date, PeriodeType periodeType)
        {
            var viewModel = new DisplayKpiInformationViewModel();

            for (int i = 0; i < numberOfKpi; i++)
            {
                var kpiInformationVm = new DisplayKpiInformationViewModel.KpiInformationViewModel { Position = i };
                var item = layout.KpiInformations.FirstOrDefault(x => x.Position == i) ??
                           new GetDerLayoutitemResponse.KpiInformationResponse { Position = i };
                if (item.Kpi != null)
                {
                    kpiInformationVm = item.MapTo<DisplayKpiInformationViewModel.KpiInformationViewModel>();
                    //var achievement = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, periodeType);
                    //kpiInformationVm.DerItemValue = achievement.MapTo<DerItemValueViewModel>();
                    if (item.ConfigType.Equals(ConfigType.KpiAchievement))
                    {
                        var achievement = _kpiAchievementService.GetKpiAchievement(item.Kpi.Id, date, periodeType);
                        kpiInformationVm.DerItemValue = achievement.MapTo<DerItemValueViewModel>();
                    }
                    else if (item.ConfigType.Equals(ConfigType.KpiTarget))
                    {
                        var target = _kpiTargetService.GetKpiTargetByValue(new GetKpiTargetRequestByValue {Kpi_Id = item.Kpi.Id, periode = date, PeriodeType = periodeType.ToString()} );
                        kpiInformationVm.DerItemValue = target.MapTo<DerItemValueViewModel>();
                    }
                }

                viewModel.KpiInformationViewModels.Add(kpiInformationVm);
            }

            return viewModel;
        }

        private string GetDoubleToString(double? val)
        {
            return val.HasValue ? val.Value.ToString(CultureInfo.InvariantCulture) : "n/a";
        }

        private DisplayKpiInformationViewModel.KpiInformationViewModel AddTarget(int position, GetDerLayoutitemResponse.KpiInformationResponse target, DateTime date)
        {
            var kpiInformationVm = new DisplayKpiInformationViewModel.KpiInformationViewModel { Position = position };

            if (target != null)
            {
                kpiInformationVm.DerItemValue = new DerItemValueViewModel();
                var daily = _kpiTargetService.GetKpiTargetByValue(new GetKpiTargetRequestByValue
                {
                    Kpi_Id = target.Kpi.Id,
                    periode = date,
                    PeriodeType = PeriodeType.Daily.ToString()
                });
                var mtd = _kpiTargetService.GetKpiTargetByValue(new GetKpiTargetRequestByValue
                {
                    Kpi_Id = target.Kpi.Id,
                    periode = new DateTime(date.Year, date.Month, 1),
                    PeriodeType = PeriodeType.Monthly.ToString()
                });

                var ytd = _kpiTargetService.GetKpiTargetByValue(new GetKpiTargetRequestByValue
                {
                    Kpi_Id = target.Kpi.Id,
                    periode = new DateTime(date.Year, 1, 1),
                    PeriodeType = PeriodeType.Yearly.ToString()
                });
                kpiInformationVm.DerItemValue.Value = daily.Value.ToString(); ;
                kpiInformationVm.DerItemValue.Mtd = mtd.Value.ToString(); ;
                kpiInformationVm.DerItemValue.Ytd = ytd.Value.ToString(); ;
                var obj = new
                {
                    daily = daily.Remark,
                    mtd = mtd.Remark,
                    ytd = ytd.Remark
                };
                var json = new JavaScriptSerializer().Serialize(obj);
                kpiInformationVm.DerItemValue.Remark = json;

            }

            return kpiInformationVm;
        }
    }
}