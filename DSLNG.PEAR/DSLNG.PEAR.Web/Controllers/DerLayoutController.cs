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
using DSLNG.PEAR.Services.Requests.User;
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
        private readonly ISelectService _selectService;
        private readonly IUserService _userService;

        public DerLayoutController(IDropdownService dropdownService, IDerService derService, IMeasurementService measurementService, IHighlightService highlightService, ISelectService selectService, IUserService userService)
        {
            _dropdownService = dropdownService;
            _derService = derService;
            _measurementService = measurementService;
            _selectService = selectService;
            _userService = userService;
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
                #region edit
                var response = _derService.GetDerLayoutItem(viewModel.Id);
                var editViewModel = response.MapTo<DerLayoutItemViewModel>();
                editViewModel.Types = _dropdownService.GetDerItemTypes().OrderBy(x => x.Text).MapTo<SelectListItem>();
                editViewModel.Type = response.Type;
                switch (response.Type.ToLowerInvariant())
                {
                    //case "line":
                    //    {
                    //        var lineChart = new LineChartViewModel();
                    //        editViewModel.LineChart = response.Artifact.MapPropertiesToInstance<LineChartViewModel>(lineChart);
                    //        var series = new LineChartViewModel.SeriesViewModel();
                    //        editViewModel.LineChart.Series.Insert(0, series);
                    //        editViewModel.Artifact.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                    //        {
                    //            Take = -1,
                    //            SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                    //        }).Measurements
                    //        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                    //        break;
                    //    }
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
                    case "line":
                    case "multiaxis":
                    case "jcc-monthly-trend":
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
                    case "speedometer":
                    case "barmeter":
                        {
                            var speedometerChart = new SpeedometerChartViewModel();
                            editViewModel.SpeedometerChart = response.Artifact.MapPropertiesToInstance<SpeedometerChartViewModel>(speedometerChart);
                            if (response.Artifact.Series.Count > 0)
                            {
                                speedometerChart.LabelSeries = new SpeedometerChartViewModel.SeriesViewModel
                                {
                                    KpiId = response.Artifact.Series[0].KpiId,
                                    KpiName = response.Artifact.Series[0].KpiName,
                                    Label = response.Artifact.Series[0].Label
                                };
                            }
                            else {
                                speedometerChart.LabelSeries = new SpeedometerChartViewModel.SeriesViewModel();
                            }
                            var plot = new SpeedometerChartViewModel.PlotBand();
                            editViewModel.SpeedometerChart.PlotBands.Insert(0, plot);
                            break;
                        }
                    case "highlight":
                        {
                            var result = _selectService.GetHighlightTypesDropdown();
                            editViewModel.Highlights = result.Select(item => new SelectListItem() { Text = item.Text, Value = item.Value }).ToList();
                            editViewModel.HighlightId = response.Highlight.SelectOptionId;
                            break;
                        }
                    case "key-equipment-status":
                        {
                            var result = _selectService.GetHighlightTypesDropdown();
                            editViewModel.Highlights = result.Select(item => new SelectListItem() { Text = item.Text, Value = item.Value }).ToList();
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 24);
                            for (int i = 0; i < response.KpiInformations.Count; i++)
                            {
                                if (response.KpiInformations[i].SelectOption != null)
                                {
                                    editViewModel.KpiInformations[i].HighlightId = response.KpiInformations[i].SelectOption.Id;
                                }
                            }
                        }
                        break;
                    case "termometer":
                    case "person-on-board":
                    case "total-commitment":
                    case "temperature":
                    case "ph":
                    case "oil-grease":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 1);
                            break;
                        }                    
                    case "procurement":                    
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 2);
                            break;
                        }
                    case "hhv":
                    case "mgdp":                                        
                    case "job-pmts":
                    case "no2":
                    case "so2":
                    case "particulate":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 3);
                            break;
                        }                   
                    case "total-feed-gas":
                    case "weekly-maintenance":
                    case "critical-pm":
                    case "loading-duration":
                    case "dafwc":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 4);
                            break;
                        }
                    case "indicative-commercial-price":
                    case "avg-ytd-key-statistic":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 5);
                            break;
                        }
                    case "security":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 6);
                            break;
                        }
                    case "plant-availability":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 7);
                            break;
                        }                    
                    case "lng-and-cds-production":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 11);
                            break;
                        }
                    case "safety":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 14);
                            break;
                        }
                    case "global-stock-market":
                    case "table-tank":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 15);
                            var result = _selectService.GetHighlightTypesDropdown();
                            editViewModel.Highlights = result.Select(item => new SelectListItem() { Text = item.Text, Value = item.Value }).ToList();
                            break;
                        }                    
                        
                    case "economic-indicator":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 16);
                            var result = _selectService.GetHighlightTypesDropdown();
                            editViewModel.Highlights = result.Select(item => new SelectListItem() { Text = item.Text, Value = item.Value }).ToList();
                            break;
                        }
                    case "lng-and-cds":
                        {
                            editViewModel.KpiInformations = AddEmptyKpiInformations(editViewModel.KpiInformations, 17);
                            break;
                        }
                    case "prepared-by":
                    case "reviewed-by":
                        {
                            editViewModel.SignedBy = response.SignedBy;
                            var result = _userService.GetUsers(new GetUsersRequest { SortingDictionary = new Dictionary<string, SortOrder>(), Take = 1000 });
                            editViewModel.Users =
                                result.Users.Select(
                                    item => new SelectListItem() { Text = item.Username, Value = item.Id.ToString() }).ToList();
                            break;
                        }

                }
                return View("EditLayoutItem", editViewModel);
                #endregion
            }
            else
            {
                #region create
                viewModel.Types = _dropdownService.GetDerItemTypes().OrderBy(x => x.Text).MapTo<SelectListItem>();
                var rowCol = viewModel.Row.ToString() + "-and-" + viewModel.Column.ToString();
                switch (rowCol)
                {
                    case "0-and-0":
                        {
                            viewModel.Type = "avg-ytd-key-statistic";
                            break;
                        };
                    case "0-and-2":
                        {
                            viewModel.Type = "speedometer";
                            break;
                        }
                    case "1-and-0":                        
                    case "1-and-1":
                    case "1-and-2":
                    case "1-and-3":
                    case "15-and-4":                 
                    case "5-and-2":
                    case "5-and-3":
                    case "5-and-4":
                        {
                            viewModel.Type = "multiaxis";
                            break;
                        }
                    case "2-and-0":
                        {
                            viewModel.Type = "dafwc";
                            break;
                        }
                    case "2-and-1":
                        {
                            viewModel.Type = "weather";
                            break;
                        }
                    case "2-and-2":
                        {
                            viewModel.Type = "wave";
                            break;
                        }
                    case "2-and-3":
                        {
                            viewModel.Type = "temperature";
                            break;
                        }
                    case "3-and-0":
                        {
                            viewModel.Type = "safety";
                            break;
                        }
                    case "3-and-1":
                    case "3-and-2":
                    case "3-and-3":
                    case "0-and-1":
                    case "0-and-3":
                    case "3-and-5":
                    case "10-and-2":
                    case "11-and-2":
                    case "11-and-1":
                    case "12-and-0":
                    case "12-and-2":
                    case "13-and-0":
                    case "14-and-2":
                    case "15-and-1":
                    case "15-and-2":
                    case "16-and-4":
                    case "16-and-5":
                    case "16-and-6":
                    case "16-and-7":
                        {
                            viewModel.Type = "highlight";
                            break;
                        }
                    case "3-and-4":
                        {
                            viewModel.Type = "pie";
                            break;
                        }
                    case "3-and-6":
                        {
                            viewModel.Type = "no2";
                            break;
                        }
                    case "3-and-7":
                        {
                            viewModel.Type = "so2";
                            break;
                        }
                    case "3-and-8":
                        {
                            viewModel.Type = "particulate";
                            break;
                        }
                    case "3-and-9":
                        {
                            viewModel.Type = "ph";
                            break;
                        }
                    case "3-and-10":
                        {
                            viewModel.Type = "oil-grease";
                            break;
                        }
                    case "4-and-0":
                        {
                            viewModel.Type = "dafwc";
                            break;
                        }
                    case "4-and-1":
                        {
                            viewModel.Type = "security";
                            break;
                        }
                    case "4-and-2":
                        {
                            viewModel.Type = "alert";
                            break;
                        }
                    case "5-and-0":
                        {
                            viewModel.Type = "hhv";
                            break;
                        }
                    case "5-and-1":
                        {
                            viewModel.Type = "total-commitment";
                            break;
                        }                    
                    case "6-and-0":
                        {
                            viewModel.Type = "total-feed-gas";
                            break;
                        }
                    case "6-and-2":
                        {
                            viewModel.Type = "plant-availability";
                            break;
                        }
                    case "6-and-3":
                    case "6-and-4":
                    case "6-and-6":
                    case "6-and-7":
                    case "6-and-8":
                    case "6-and-9":
                    case "6-and-10":
                    case "6-and-11":
                        {
                            viewModel.Type = "barmeter";
                            break;
                        }
                    case "6-and-5":
                        {
                            viewModel.Type = "termometer";
                            break;

                        }
                    case "6-and-12":
                        {
                            viewModel.Type = "flare";
                            break;
                        }
                    case "7-and-0":
                        {
                            viewModel.Type = "lng-and-cds-production";
                            break;
                        }
                    case "7-and-1":
                        {
                            viewModel.Type = "lng-and-cds";
                            break;
                        }
                    case "8-and-0":
                    case "8-and-1":
                    case "8-and-2":
                    case "8-and-3":
                        {
                            viewModel.Type = "tank";
                            break;
                        }
                    case "8-and-4":
                        {
                            viewModel.Type = "nls";
                            break;
                        }
                    case "9-and-0":
                        {
                            viewModel.Type = "table-tank";
                            break;
                        }
                    case "10-and-0":
                        {
                            viewModel.Type = "weekly-maintenance";
                            break;
                        }
                    case "10-and-1":
                        {
                            viewModel.Type = "key-equipment-status";
                            break;
                        }
                    case "11-and-0":
                        {
                            viewModel.Type = "critical-pm";
                            break;
                        }
                    case "12-and-1":
                        {
                            viewModel.Type = "loading-duration";
                            break;
                        }
                    case "14-and-0":
                        {
                            viewModel.Type = "procurement";
                            break;
                        }
                    case "15-and-0":
                        {
                            viewModel.Type = "indicative-commercial-price";
                            break;
                        }
                    case "14-and-1":
                        {
                            viewModel.Type = "economic-indicator";
                            break;
                        }
                    case "16-and-1":
                        {
                            viewModel.Type = "global-stock-market";
                            break;
                        }
                    case "16-and-2":
                        {
                            viewModel.Type = "prepared-by";
                            break;
                        }
                    case "16-and-3":
                        {
                            viewModel.Type = "reviewed-by";
                            break;
                        }
                    case "15-and-3":
                        {
                            viewModel.Type = "person-on-board";
                            break;
                        }
                    case "15-and-5":
                        {
                            viewModel.Type = "jcc-monthly-trend";
                            break;
                        }
                }
                return View("LayoutItem", viewModel);
                #endregion
            }
        }

        public ActionResult LayoutSetting(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "barmeter":
                case "speedometer":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.Artifact = new DerLayoutItemViewModel.DerLayoutItemArtifactViewModel();
                        viewModel.Artifact.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest
                        {
                            Take = -1,
                            SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                        }).Measurements
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                        viewModel.SpeedometerChart = new SpeedometerChartViewModel();
                        var plot = new SpeedometerChartViewModel.PlotBand();
                        viewModel.SpeedometerChart.PlotBands.Add(plot);
                        return PartialView("LayoutType/_Speedometer", viewModel);
                    }
                case "line":
                case "multiaxis":
                case "jcc-monthly-trend":
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
                case "nls":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.Type = type;
                        return PartialView("LayoutType/_StaticHighlight", viewModel);
                    }
                case "temperature":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(1);
                        return PartialView("LayoutType/_Temperature", viewModel);
                    }

                case "avg-ytd-key-statistic":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(5);
                        return PartialView("LayoutType/_AvgYtdKeyStatistic", viewModel);
                    }
                case "flare":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(9);
                        return PartialView("LayoutType/_Flare", viewModel);
                    }
                case "safety":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(14);
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
                        viewModel.KpiInformations = GetKpiInformations(17);
                        return PartialView("LayoutType/_LngAndCds", viewModel);
                    }
                case "dafwc":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(4);
                        return PartialView("LayoutType/_Dafwc", viewModel);
                    }
                case "job-pmts":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(3);
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
                        viewModel.KpiInformations = GetKpiInformations(15);
                        return PartialView("LayoutType/_TableTank", viewModel);
                    }
                case "mgdp":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(3);
                        return PartialView("LayoutType/_MGDP", viewModel);
                    }
                case "hhv":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(3);
                        return PartialView("LayoutType/_HHV", viewModel);
                    }
                case "lng-and-cds-production":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(11);
                        return PartialView("LayoutType/_LngAndCdsProduction", viewModel);
                    }
                case "weekly-maintenance":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(3);
                        return PartialView("LayoutType/_WeeklyMaintenance", viewModel);
                    }
                case "critical-pm":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(4);
                        return PartialView("LayoutType/_CriticalPm", viewModel);
                    }
                case "procurement":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(2);
                        return PartialView("LayoutType/_Procurement", viewModel);
                    }
                case "indicative-commercial-price":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(5);
                        return PartialView("LayoutType/_IndicativeCommercialPrice", viewModel);
                    }
                case "plant-availability":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(7);
                        return PartialView("LayoutType/_PlantAvailability", viewModel);
                    }
                case "economic-indicator":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(15);
                        return PartialView("LayoutType/_EconomicIndicator", viewModel);
                    }
                case "key-equipment-status":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(24);
                        var result = _selectService.GetHighlightTypesDropdown();
                        viewModel.Highlights = result.Select(item => new SelectListItem() { Text = item.Text, Value = item.Value }).ToList();
                        return PartialView("LayoutType/_KeyEquipmentStatus", viewModel);
                    }
                case "global-stock-market":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(13);
                        var result = _selectService.GetHighlightTypesDropdown();
                        viewModel.Highlights = result.Select(item => new SelectListItem() { Text = item.Text, Value = item.Value }).ToList();
                        return PartialView("LayoutType/_GlobalStockMarket", viewModel);
                    }
                case "prepared-by":
                case "reviewed-by":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        var result = _userService.GetUsers(new GetUsersRequest { SortingDictionary = new Dictionary<string, SortOrder>(), Take = 1000 });
                        viewModel.Users =
                            result.Users.Select(
                                item => new SelectListItem() { Text = item.Username, Value = item.Id.ToString() }).ToList();
                        return PartialView("LayoutType/_User", viewModel);
                    }
                case "termometer":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(1);
                        return PartialView("LayoutType/_Termometer", viewModel);

                    }
                case "loading-duration":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(4);
                        return PartialView("LayoutType/_LoadingDuration", viewModel);
                    }
                case "person-on-board":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(1);
                        return PartialView("LayoutType/_PersonOnBoard", viewModel);
                    }
                case "total-commitment":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(1);
                        return PartialView("LayoutType/_TotalCommitment", viewModel);

                    }
                case "no2":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(3);
                        return PartialView("LayoutType/_NO2", viewModel);
                    }
                case "so2":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(3);
                        return PartialView("LayoutType/_SO2", viewModel);
                    }
                case "particulate":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(3);
                        return PartialView("LayoutType/_Particulate", viewModel);
                    }
                case "ph":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(1);
                        return PartialView("LayoutType/_PH", viewModel);
                    }
                case "oil-grease":
                    {
                        var viewModel = new DerLayoutItemViewModel();
                        viewModel.KpiInformations = GetKpiInformations(1);
                        return PartialView("LayoutType/_OilGrease", viewModel);
                    }

            }

            return Content("Error");
        }

        public ActionResult Delete(int id, string type)
        {
            var response = _derService.DeleteLayoutItem(id, type);

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
                case "multiaxis":
                case "jcc-monthly-trend":
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
                case "barmeter":
                case "speedometer":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.Artifact = layoutItemViewModel.Artifact.MapTo<SaveLayoutItemRequest.LayoutItemArtifact>();
                        request.Artifact.Speedometer = layoutItemViewModel.SpeedometerChart.MapTo<SaveLayoutItemRequest.LayoutItemArtifactSpeedometer>();
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
                case "nls":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.Type = layoutItemViewModel.Type;
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }
                case "safety":
                case "security":
                case "avg-ytd-key-statistic":
                case "temperature":
                case "lng-and-cds":
                case "total-feed-gas":
                case "table-tank":
                case "hhv":
                case "lng-and-cds-production":
                case "weekly-maintenance":
                case "critical-pm":
                case "procurement":
                case "indicative-commercial-price":
                case "economic-indicator":
                case "key-equipment-status":
                case "plant-availability":
                case "job-pmts":
                case "mgdp":
                case "global-stock-market":
                case "dafwc":
                case "termometer":
                case "loading-duration":
                case "person-on-board":
                case "flare":
                case "total-commitment":
                case "no2":
                case "so2":
                case "ph":
                case "particulate":
                case "oil-grease":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.KpiInformations = layoutItemViewModel.KpiInformations.MapTo<SaveLayoutItemRequest.DerKpiInformationRequest>();
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }
                case "prepared-by":
                case "reviewed-by":
                    {
                        request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                        request.SignedBy = layoutItemViewModel.SignedBy;
                        response = _derService.SaveLayoutItem(request);
                        break;
                    }
                    /*case "job-pmts":
                    case "mgdp":
                        {
                            request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                            request.KpiInformations = layoutItemViewModel.KpiInformations.MapTo<SaveLayoutItemRequest.DerKpiInformationRequest>();
                            var mbbtuKpi = request.KpiInformations.FirstOrDefault(x => x.Position == 1);
                            if (mbbtuKpi != null)
                            {
                                var newMbbtuKpiTarget = new SaveLayoutItemRequest.DerKpiInformationRequest();
                                newMbbtuKpiTarget.ConfigType = ConfigType.KpiTarget;
                                newMbbtuKpiTarget.KpiId = mbbtuKpi.KpiId;
                                newMbbtuKpiTarget.Position = 3;
                                request.KpiInformations.Add(newMbbtuKpiTarget);
                            }
                            response = _derService.SaveLayoutItem(request);
                            break;
                        }*/
                    /*case "lng-and-cds":
                        {
                            request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                            request.KpiInformations =
                                layoutItemViewModel.KpiInformations.MapTo<SaveLayoutItemRequest.DerKpiInformationRequest>();
                            response = _derService.SaveLayoutItem(request);
                            break;
                        }*/
                    /*case "dafwc":
                        {
                            request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                            response = _derService.SaveLayoutItem(request);
                            break;
                        }*/
                    /*case "plant-availability":
                    {
                            request = layoutItemViewModel.MapTo<SaveLayoutItemRequest>();
                            request.KpiInformations = layoutItemViewModel.KpiInformations.MapTo<SaveLayoutItemRequest.DerKpiInformationRequest>();
                            var mbbtuKpi = request.KpiInformations.FirstOrDefault(x => x.Position == 0 || x.Position == 1 || x.Position == 2 ||
                                x.Position ==3);
                            if (mbbtuKpi != null)
                            {
                                var newMbbtuKpiTarget = new SaveLayoutItemRequest.DerKpiInformationRequest();
                                newMbbtuKpiTarget.ConfigType = ConfigType.KpiTarget;
                                newMbbtuKpiTarget.KpiId = mbbtuKpi.KpiId;
                                newMbbtuKpiTarget.Position = 3;
                                request.KpiInformations.Add(newMbbtuKpiTarget);
                            }
                            response = _derService.SaveLayoutItem(request);
                            break;
                        }*/
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

        private IList<DerLayoutItemViewModel.DerKpiInformationViewModel> AddEmptyKpiInformations(IList<DerLayoutItemViewModel.DerKpiInformationViewModel> kpiInformations, int i)
        {
            var listKpiInformation = new List<DerLayoutItemViewModel.DerKpiInformationViewModel>();
            for (int j = 0; j < i; j++)
            {
                var kpiInformation = kpiInformations.SingleOrDefault(x => x.Position == j);
                if (kpiInformation == null)
                {
                    listKpiInformation.Add(new DerLayoutItemViewModel.DerKpiInformationViewModel() { Position = j });
                }
                else
                {
                    listKpiInformation.Add(kpiInformation);
                }

            }

            return listKpiInformation;
        }

        /* private IList<DerLayoutItemViewModel.DerKpiInformationViewModel> GetWeeklyMaintenance(int numberOfKpi)
         {
             var list = new List<DerLayoutItemViewModel.DerKpiInformationViewModel>();
             for (int i = 0; i < numberOfKpi; i++)
             {
                 if (i < 3)
                 {
                     list.Add(new DerLayoutItemViewModel.DerKpiInformationViewModel {Position = i});
                 }
                 else
                 {
                     list.Add();
                 }

             }

             return list;
         } */

    }
}