using DSLNG.PEAR.Common.Contants;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.DerLoadingSchedule;
using DSLNG.PEAR.Services.Requests.DerTransaction;
using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using DSLNG.PEAR.Services.Requests.KpiTarget;
using DSLNG.PEAR.Services.Requests.Select;
using DSLNG.PEAR.Services.Requests.Wave;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Web.Attributes;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.DerTransaction;
using DSLNG.PEAR.Web.ViewModels.Highlight;
using DSLNG.PEAR.Web.ViewModels.Wave;
using DSLNG.PEAR.Web.ViewModels.Weather;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerTransactionController : BaseController
    {
        private readonly IDerService _derService;
        private readonly IDerTransactionService _derTransactionService;
        private readonly IKpiAchievementService _kpiAchievementService;
        private readonly IKpiTargetService _kpiTargetService;
        private readonly IHighlightService _highlightService;
        private readonly ISelectService _selectService;
        private readonly IWaveService _waveService;
        private readonly IWeatherService _weatherService;
        private readonly IDerLoadingScheduleService _derLoadingScheduleService;
        private readonly IKpiTransformationService _kpiTransformationService;

        public DerTransactionController(IDerService derService,
            IDerTransactionService derTransactionService,
            IKpiAchievementService kpiAchievementService,
            IKpiTargetService kpiTargetService,
            IHighlightService highlightService,
            ISelectService selectService,
            IWaveService waveService,
            IWeatherService weatherService,
            IDerLoadingScheduleService derLoadingScheduleService,
            IKpiTransformationService kpiTransformationService)
        {
            _derService = derService;
            _derTransactionService = derTransactionService;
            _kpiAchievementService = kpiAchievementService;
            _kpiTargetService = kpiTargetService;
            _highlightService = highlightService;
            _selectService = selectService;
            _waveService = waveService;
            _weatherService = weatherService;
            _derLoadingScheduleService = derLoadingScheduleService;
            _kpiTransformationService = kpiTransformationService;
        }
        // GET: DerTransaction
        public ActionResult Index()
        {
            ViewBag.KpiTransformations = _kpiTransformationService.Get(new Services.Requests.KpiTransformation.GetKpiTransformationsRequest
            {
                Skip = 0,
                Take = -1
            }).KpiTransformations.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            return View("Input");
        }

        public ActionResult Input()
        {
            ViewBag.KpiTransformations = _kpiTransformationService.Get(new Services.Requests.KpiTransformation.GetKpiTransformationsRequest
            {
                Skip = 0,
                Take = -1
            }).KpiTransformations.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            return View("Input");
        }       

        public ActionResult EconomicIndicator(string date)
        {
            return View(GetDerValuesPerSection(date,
                new int[] { 386, 79, 80, 128, 388, 389, 390, 383, 64, 384, 65, 385, 62, 63, 391, 392, 393, 394, 395, 397 }, //actual KpiIds 
                new int[] { }, //target KpiIds
                new int[] { 15, 68, 83 }  //highlightTypeIds
                ));
        }

        public ActionResult ForcastedIndicator(string date)
        {
            return View(GetDerValuesPerSection(date,
                new int[] { 386, 79, 80, 128, 388, 389, 390, 383, 64, 384, 65, 385, 62, 63, 391, 392, 393, 394, 395, 397, 409, 410, 411, 412, 413, 414, 415, 416 }, //actual KpiIds 
                new int[] { }, //target KpiIds
                new int[] { 63 }  //highlightTypeIds
                ));
        }

        public ActionResult OperationSection(string date)
        {
            return View(GetDerValuesPerSection(date,
                new int[] { 194,176,100,99,38,40,41,39,165,168,169,166,170,173,174,171,398,401,402,399,11,101,102,175,110,
                    103,105,104,78,42,91,92,93,94,95,96,97,251,252,253,254,255,256,257,403,258,259,260,261,262,263,264,265,266,
                    267,268,269,270,271,404,405,406,56,57,58,368,70,369,360,361,362,363,364,87,86,85,370,185,184,43,5,6,
                    45,46,47,49,50,48,71,72,73,77,74,75,76,82,7,8,82,76,365,433,366,367,369,15,241,239,240,423,430,434, 441,442,443,108
                }, //actual KpiIds 
                new int[] { 10, 9, 53, 12, 169, 174, 166, 171, 15, 241, 239, 240, 468 }, //target KpiIds
                new int[] { 19, 12, 69 }  //highlightTypeIds
                ));
        }

        public ActionResult MarineShipping(string date)
        {
            var dateTime = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var viewModel = GetDerValuesPerSection(date,
                new int[] { 375, 376, 377, 378 }, //actual KpiIds 
                new int[] { 375, 377 }, //target KpiIds
                new int[] { 52 }  //highlightTypeIds
                );
            viewModel.DerLoadingSchedule = _derLoadingScheduleService.Get(new GetDerLoadingSchedulesRequest { Periode = dateTime }).MapTo<DerValuesViewModel.DerLoadingScheduleViewModel>();
            return View(viewModel);
        }

        public ActionResult MaintenanceSection(string date)
        {
            return View(GetDerValuesPerSection(date,
               new int[] { 59, 60, 61, 371, 372, 373, 374 }, //actual KpiIds 
               new int[] { 59, 374 }, //target KpiIds
               new int[] { 46, 34, 35, 36, 37, 31, 32, 33, 28, 29, 30, 38, 39, 47, 40, 41, 42, 43, 48, 44, 45, 49, 50, 51, 11, 77, 78, 79, 60, 61 }  //highlightTypeIds
               ));
        }

        public ActionResult QhsseSection(string date)
        {
            var viewModel = GetDerValuesPerSection(date,
               new int[] { 273, 274, 275, 276, 1, 177, 278, 277, 285, 356, 4, 359, 286, 292, 421, 422, 284, 357, 358, 435, 436, 
               /*CR Sept 2017*/ 504, 506, 507,508, 509, 518, 511, 512, 513, 514, 515 }, //actual KpiIds 
               new int[] { 1, 177, 278, 277, 276, 285, 421, 422, 284, 357, 358,
               /*CR Sept 2017*/ 504, 505, 506, 507,508, 509, 518, 511, 514, 515 }, //target KpiIds
               new int[] { 18, 13, 20, 7, 80, 59 }  //highlightTypeIds
               );
            var theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var wave = _waveService.GetWave(new GetWaveRequest
            {
                Date = theDate,
                ByDate = true
            });
            if (wave.Id != 0)
            {
                viewModel.Wave = wave.MapTo<WaveViewModel>();
                if (viewModel.Wave.ValueId != 0)
                {
                    viewModel.Wave.WindDirectValueType = "now";
                }

                if (!string.IsNullOrEmpty(viewModel.Wave.Tide))
                {
                    viewModel.Wave.TideValueType = "now";
                }

                if (!string.IsNullOrEmpty(viewModel.Wave.Speed))
                {
                    viewModel.Wave.SpeedValueType = "now";
                }
            }
            else
            {
                wave = _waveService.GetWave(new GetWaveRequest
                {
                    Date = theDate.AddDays(-1),
                    ByDate = true
                });

                if (wave.Id != 0)
                {
                    viewModel.Wave = wave.MapTo<WaveViewModel>();
                    
                    viewModel.Wave.WindDirectValueType = "prev";
                    viewModel.Wave.TideValueType = "prev";
                    viewModel.Wave.SpeedValueType = "prev";
                    viewModel.Wave.Id = 0;
                }
            }
            if (viewModel.Wave == null) viewModel.Wave = new WaveViewModel();
            viewModel.Wave.Values = _selectService.GetSelect(new GetSelectRequest { Name = "wave-values" }).Options
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList();
            var weather = _weatherService.GetWeather(new GetWeatherRequest
            {
                Date = theDate,
                ByDate = true
            });
            if (weather.Id != 0)
            {
                viewModel.Weather = weather.MapTo<WeatherViewModel>();
                viewModel.Weather.DerValueType = "now";
            }
            else
            {
                weather = _weatherService.GetWeather(new GetWeatherRequest
                {
                    Date = theDate.AddDays(-1),
                    ByDate = true
                });
                if (wave.Id != 0)
                {
                    viewModel.Weather = weather.MapTo<WeatherViewModel>();
                    viewModel.Weather.DerValueType = "prev";
                    viewModel.Weather.Id = 0;
                }
            }
            if (viewModel.Weather == null) viewModel.Weather = new WeatherViewModel();
            viewModel.Weather.Values = _selectService.GetSelect(new GetSelectRequest { Name = "weather-values" }).Options
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList();
            viewModel.AlertOptions = _selectService.GetSelect(new GetSelectRequest { ParentName = "highlight-types", ParentOptionId = 7 }).Options
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text }).ToList();
            return View(viewModel);
        }

        public ActionResult EnablerSection(string date)
        {
            return View(GetDerValuesPerSection(date,
           new int[] { 379, 380, 36 }, //actual KpiIds 
           new int[] { 380 }, //target KpiIds
           new int[] { 66, 53, 14, 8, 58, 21, 53, 67 }  //highlightTypeIds
           ));
        }

        public ActionResult Input2()
        {
            ViewBag.KpiTransformations = _kpiTransformationService.Get(new Services.Requests.KpiTransformation.GetKpiTransformationsRequest
            {
                Skip = 0,
                Take = -1
            }).KpiTransformations.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            return View();
        }

        public ActionResult EconomicIndicator2(string date)
        {
            return View(GetDerValuesPerSection(date,
                new int[] { 386, 79, 80, 128, 388, 389, 390, 383, 64, 384, 65, 385, 62, 63, 391, 392, 393, 394, 395, 397 }, //actual KpiIds 
                new int[] { }, //target KpiIds
                new int[] { 15, 68 }  //highlightTypeIds
                ));
        }

        public ActionResult ForcastedIndicator2(string date)
        {
            return View(GetDerValuesPerSection(date,
                new int[] { 386, 79, 80, 128, 388, 389, 390, 383, 64, 384, 65, 385, 62, 63, 391, 392, 393, 394, 395, 397, 409, 410, 411, 412, 413, 414, 415, 416 }, //actual KpiIds 
                new int[] { }, //target KpiIds
                new int[] { 63 }  //highlightTypeIds
                ));
        }

        public ActionResult OperationSection2(string date)
        {
            return View(GetDerValuesPerSection(date,
                new int[] { 194,176,100,99,38,40,41,39,165,168,169,166,170,173,174,171,398,401,402,399,11,101,102,175,110,
                    103,105,104,78,42,91,92,93,94,95,96,97,251,252,253,254,255,256,257,403,258,259,260,261,262,263,264,265,266,
                    267,268,269,270,271,404,405,406,56,57,58,368,70,369,360,361,362,363,364,87,86,85,370,185,184,43,5,6,
                    45,46,47,49,50,48,71,72,73,77,74,75,76,82,7,8,82,76,365,433,366,367,369,15,241,239,240,423,430,434, 441,442,443,108
                }, //actual KpiIds 
                new int[] { 10, 9, 53, 12, 169, 174, 166, 171, 15, 241, 239, 240 }, //target KpiIds
                new int[] { 19, 12, 69 }  //highlightTypeIds
                ));
        }

        public ActionResult MarineShipping2(string date)
        {
            var dateTime = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var viewModel = GetDerValuesPerSection(date,
                new int[] { 375, 376, 377, 378 }, //actual KpiIds 
                new int[] { 375, 377 }, //target KpiIds
                new int[] { 52 }  //highlightTypeIds
                );
            viewModel.DerLoadingSchedule = _derLoadingScheduleService.Get(new GetDerLoadingSchedulesRequest { Periode = dateTime }).MapTo<DerValuesViewModel.DerLoadingScheduleViewModel>();
            return View(viewModel);
        }

        public ActionResult MaintenanceSection2(string date)
        {
            return View(GetDerValuesPerSection(date,
               new int[] { 59, 60, 61, 371, 372, 373, 374 }, //actual KpiIds 
               new int[] { 59, 374 }, //target KpiIds
               new int[] { 46, 34, 35, 36, 37, 31, 32, 33, 28, 29, 30, 38, 39, 47, 40, 41, 42, 43, 48, 44, 45, 49, 50, 51, 11, 77, 78, 79, 60, 61 }  //highlightTypeIds
               ));
        }

        //public ActionResult QhsseSection2(string date)
        //{
        //    var viewModel = GetDerValuesPerSection(date,
        //       new int[] { 273, 274, 275, 276, 1, 177, 278, 277, 285, 356, 4, 359, 286, 292, 421, 422, 284, 357, 358, 435, 436 }, //actual KpiIds 
        //       new int[] { 1, 177, 278, 277, 276, 285, 421, 422, 284, 357, 358 }, //target KpiIds
        //       new int[] { 18, 13, 20, 7, 80, 59 }  //highlightTypeIds
        //       );
        //    var theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        //    var wave = _waveService.GetWave(new GetWaveRequest
        //    {
        //        Date = theDate,
        //        ByDate = true
        //    });
        //    if (wave.Id != 0)
        //    {
        //        viewModel.Wave = wave.MapTo<WaveViewModel>();
        //        viewModel.Wave.DerValueType = "now";
        //    }
        //    else
        //    {
        //        wave = _waveService.GetWave(new GetWaveRequest
        //        {
        //            Date = theDate.AddDays(-1),
        //            ByDate = true
        //        });
        //        if (wave.Id != 0)
        //        {
        //            viewModel.Wave = wave.MapTo<WaveViewModel>();
        //            viewModel.Wave.DerValueType = "prev";
        //        }
        //    }
        //    if (viewModel.Wave == null) viewModel.Wave = new WaveViewModel();
        //    viewModel.Wave.Values = _selectService.GetSelect(new GetSelectRequest { Name = "wave-values" }).Options
        //        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList();
        //    var weather = _weatherService.GetWeather(new GetWeatherRequest
        //    {
        //        Date = theDate,
        //        ByDate = true
        //    });
        //    if (weather.Id != 0)
        //    {
        //        viewModel.Weather = weather.MapTo<WeatherViewModel>();
        //        viewModel.Weather.DerValueType = "now";
        //    }
        //    else
        //    {
        //        weather = _weatherService.GetWeather(new GetWeatherRequest
        //        {
        //            Date = theDate.AddDays(-1),
        //            ByDate = true
        //        });
        //        if (wave.Id != 0)
        //        {
        //            viewModel.Weather = weather.MapTo<WeatherViewModel>();
        //            viewModel.Weather.DerValueType = "prev";
        //        }
        //    }
        //    if (viewModel.Weather == null) viewModel.Weather = new WeatherViewModel();
        //    viewModel.Weather.Values = _selectService.GetSelect(new GetSelectRequest { Name = "weather-values" }).Options
        //        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Text }).ToList();
        //    viewModel.AlertOptions = _selectService.GetSelect(new GetSelectRequest { ParentName = "highlight-types", ParentOptionId = 7 }).Options
        //        .Select(x => new SelectListItem { Value = x.Value, Text = x.Text }).ToList();
        //    return View(viewModel);
        //}

        public ActionResult EnablerSection2(string date)
        {
            return View(GetDerValuesPerSection(date,
           new int[] { 379, 380, 36 }, //actual KpiIds 
           new int[] { 380 }, //target KpiIds
           new int[] { 66, 53, 14, 8, 58, 21, 53, 67 }  //highlightTypeIds
           ));
        }

        [HttpPost]
        public ActionResult UpdateKpi(UpdateKpiOriginalViewModel viewModel)
        {
            if (viewModel.Id == 0 && viewModel.Value == null)
            {
                return null;
            }
            var sPeriodeType = viewModel.Type.Split('-')[0];
            var periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), sPeriodeType, true);
            var theDate = DateTime.ParseExact(viewModel.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            switch (periodeType)
            {
                case PeriodeType.Monthly:
                    theDate = new DateTime(theDate.Year, theDate.Month, 1);
                    break;
                case PeriodeType.Yearly:
                    theDate = new DateTime(theDate.Year, 1, 1);
                    break;
            }
            switch (viewModel.Type)
            {
                case "daily-actual":
                    {
                        var request = new UpdateKpiAchievementItemRequest
                        {
                            Periode = theDate,
                            PeriodeType = periodeType,
                            Id = viewModel.Id,
                            KpiId = viewModel.KpiId,
                            UserId = UserProfile().UserId,
                            Value = viewModel.ValueType == "value" ? viewModel.Value : null,
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null,
                            ControllerName = "Der Input Form",
                            ActionName = string.Format("UpdateKPI daily-actual KPI:{0}", viewModel.KpiId),
                            ValueType = viewModel.ValueType
                        };
                        var resp = _kpiAchievementService.UpdateOriginalData(request, true);
                        return Json(resp);
                    }
                case "monthly-actual":
                    {
                        var request = new UpdateKpiAchievementItemRequest
                        {
                            Periode = theDate,
                            PeriodeType = periodeType,
                            Id = viewModel.Id,
                            KpiId = viewModel.KpiId,
                            UserId = UserProfile().UserId,
                            Value = viewModel.ValueType == "value" ? viewModel.Value : null,
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null,
                            ControllerName = "Der Input Form",
                            ActionName = string.Format("UpdateKPI monthly-actual KPI:{0}", viewModel.KpiId),
                            ValueType = viewModel.ValueType
                        };
                        var resp = _kpiAchievementService.UpdateOriginalData(request, true);
                        return Json(resp);
                    }
                case "yearly-actual":
                    {
                        var request = new UpdateKpiAchievementItemRequest
                        {
                            Periode = theDate,
                            PeriodeType = periodeType,
                            Id = viewModel.Id,
                            KpiId = viewModel.KpiId,
                            UserId = UserProfile().UserId,
                            Value = viewModel.ValueType == "value" ? viewModel.Value : null,
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null,
                            ControllerName = "Der Input Form",
                            ActionName = string.Format("UpdateKPI yearly-actual KPI:{0}",viewModel.KpiId),
                            ValueType = viewModel.ValueType
                        };
                        var resp = _kpiAchievementService.UpdateOriginalData(request, true);
                        return Json(resp);
                    }
                // case gila-gilaan
                case "monthly-actual-prev":
                    {
                        var request = new UpdateKpiAchievementItemRequest
                        {
                            Periode = theDate.AddMonths(-1),
                            PeriodeType = periodeType,
                            Id = viewModel.Id,
                            KpiId = viewModel.KpiId,
                            UserId = UserProfile().UserId,
                            Value = viewModel.ValueType == "value" ? viewModel.Value : null,
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null,
                            ControllerName = "Der Input Form",
                            ActionName = string.Format("UpdateKPI monthly-actual-prev KPI:{0}",viewModel.KpiId),
                            ValueType = viewModel.ValueType
                        };
                        var resp = _kpiAchievementService.UpdateOriginalData(request, true);
                        return Json(resp);
                    }
                case "monthly-actual-jcc":
                    {
                        var request = new UpdateKpiAchievementItemRequest
                        {
                            Periode = theDate,
                            PeriodeType = periodeType,
                            Id = viewModel.Id,
                            KpiId = viewModel.KpiId,
                            UserId = UserProfile().UserId,
                            Value = viewModel.ValueType == "value" ? viewModel.Value : null,
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null,
                            ControllerName = "Der Input Form",
                            ActionName = string.Format("UpdateKPI monthly-actual-jcc KPI:{0}", viewModel.KpiId),
                        };
                        var resp = _kpiAchievementService.UpdateCustomJccFormula(request);
                        return Json(resp);
                    }
                case "monthly-actual-bunker":
                    {
                        var request = new UpdateKpiAchievementItemRequest
                        {
                            Periode = theDate,
                            PeriodeType = periodeType,
                            Id = viewModel.Id,
                            KpiId = viewModel.KpiId,
                            UserId = UserProfile().UserId,
                            Value = viewModel.ValueType == "value" ? viewModel.Value : null,
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null,
                            ControllerName = "Der Input Form",
                            ActionName = string.Format("UpdateKPI monthly-actual-bunker KPI:{0}", viewModel.KpiId),
                        };
                        var resp = _kpiAchievementService.UpdateCustomBunkerPriceFormula(request);
                        return Json(resp);
                    }
                case "daily-actual-dafwc":
                    {
                        string value = viewModel.ValueType == "value" ? viewModel.Value : null;
                        if (viewModel.Value != null && viewModel.ValueType == "remark")
                        {
                            DateTime lastDAFWC;
                            DateTime.TryParse(viewModel.Value, out lastDAFWC);

                            if (lastDAFWC != null)
                            {
                                value = (theDate - lastDAFWC).TotalDays.ToString();
                            }

                        }
                        var request = new UpdateKpiAchievementItemRequest
                        {
                            Periode = theDate,
                            PeriodeType = periodeType,
                            Id = viewModel.Id,
                            KpiId = viewModel.KpiId,
                            UserId = UserProfile().UserId,
                            Value = value,
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null,
                            ControllerName = "Der Input Form",
                            ActionName = string.Format("UpdateKPI monthly-actual-dafwc KPI:{0}", viewModel.KpiId),
                            ValueType = viewModel.ValueType
                        };
                        var resp = _kpiAchievementService.UpdateOriginalData(request, true);
                        return Json(resp);
                    }
                default:
                    {
                        var request = new SaveKpiTargetRequest
                        {
                            Periode = theDate,
                            PeriodeType = periodeType,
                            Id = viewModel.Id,
                            KpiId = viewModel.KpiId,
                            UserId = UserProfile().UserId,
                            Value = viewModel.ValueType == "value" ? viewModel.Value : null,
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null,
                            ControllerName = "Der Input Form",
                            ActionName = string.Format("UpdateKPI Save KPI Target Request KPI:{0}", viewModel.KpiId)
                        };
                        var resp = _kpiTargetService.UpdateOriginalData(request);
                        return Json(resp);
                    }
            }

        }

        public ActionResult UpdateHighlight(HighlightViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveHighlightRequest>();
            req.UserId = UserProfile().UserId;
            req.ControllerName = "Der Input Form";
            req.ActionName = string.Format("Update Highlight HighlightType:{0}", viewModel.TypeId);

            var resp = _highlightService.SaveHighlight(req);
            return Json(resp);
        }

        public ActionResult UpdateInfraGSM(HighlightViewModel viewModel)
        {
            var existingHighlight = _highlightService.GetHighlightByPeriode(new GetHighlightRequest
            {
                Date = viewModel.Date,
                HighlightTypeId = viewModel.TypeId
            });
            SaveHighlightRequest req = new SaveHighlightRequest();
            req.PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType, true);
            req.Date = viewModel.Date.Value;
            req.TypeId = viewModel.TypeId;
            req.UserId = UserProfile().UserId;
            req.ControllerName = "Der Input Form";
            req.ActionName = string.Format("Update Infra GSM HighlightType:{0}",viewModel.TypeId);

            if (existingHighlight.Id == 0)
            {
                req.Message = "{\"a\" : \"\",\"b\" : \"\",\"c\" : \"\",\"d\" : \"\" }";
            }
            else
            {
                req.Message = existingHighlight.Message;
                req.Id = existingHighlight.Id;
            }
            dynamic jsonObj = JsonConvert.DeserializeObject(req.Message);
            jsonObj[viewModel.Property] = viewModel.Message;
            req.Message = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            var resp = _highlightService.SaveHighlight(req);
            return Json(resp);
        }

        public ActionResult UpdateBrenfut(HighlightViewModel viewModel)
        {
            var existingHighlight = _highlightService.GetHighlightByPeriode(new GetHighlightRequest
            {
                Date = viewModel.Date,
                HighlightTypeId = viewModel.TypeId
            });
            SaveHighlightRequest req = new SaveHighlightRequest();
            req.PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType, true);
            req.Date = viewModel.Date.Value;
            req.TypeId = viewModel.TypeId;
            req.UserId = UserProfile().UserId;
            req.ControllerName = "Der Input Form";
            req.ActionName = string.Format("Update Brenfutt HighlightType:{0}",viewModel.TypeId);
            if (existingHighlight.Id == 0)
            {
                req.Message = "{\"a\" : { \"label\" : \"\", \"value\" : \"\" },\"b\" : { \"label\" : \"\", \"value\" : \"\" },\"c\" : { \"label\" : \"\", \"value\" : \"\" },\"d\" : { \"label\" : \"\", \"value\" : \"\" } }";
            }
            else
            {
                req.Message = existingHighlight.Message;
                req.Id = existingHighlight.Id;
            }
            dynamic jsonObj = JsonConvert.DeserializeObject(req.Message);
            jsonObj[viewModel.Property][viewModel.ValueType] = viewModel.Message;
            req.Message = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            var resp = _highlightService.SaveHighlight(req);
            return Json(resp);
        }

        public ActionResult UpdateWave(WaveViewModel viewModel)
        {
            var wave = _waveService.GetWave(new GetWaveRequest
            {
                Date = viewModel.Date,
                ByDate = true
            });
            if (wave.Id == 0)
            {
                var request = viewModel.MapTo<SaveWaveRequest>();
                request.UserId = UserProfile().UserId;
                request.ControllerName = "Der Input Form";
                request.ActionName = "Insert Wave";
                var resp = _waveService.SaveWave(request);
                return Json(resp);
            }
            else
            {
                var request = viewModel.MapTo<SaveWaveRequest>();
                request.Id = wave.Id;
                request.Tide = viewModel.Property == "tide" ? viewModel.Tide : wave.Tide;
                request.ValueId = viewModel.Property == "wind-direction" ? viewModel.ValueId : wave.ValueId;
                request.Speed = viewModel.Property == "speed" ? viewModel.Speed : wave.Speed;
                request.UserId = UserProfile().UserId;
                request.ControllerName = "Der Input Form";
                request.ActionName = "Update Wave";
                var resp = _waveService.SaveWave(request);
                return Json(resp);
            }
        }

        public ActionResult UpdateWeeklyAlarm(HighlightViewModel viewModel)
        {
            var existingHighlight = _highlightService.GetHighlightByPeriode(new GetHighlightRequest
            {
                Date = viewModel.Date,
                HighlightTypeId = viewModel.TypeId
            });
            SaveHighlightRequest req = new SaveHighlightRequest();
            req.PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType, true);
            req.Date = viewModel.Date.Value;
            req.TypeId = viewModel.TypeId;
            req.UserId = UserProfile().UserId;
            req.ControllerName = "Der Input Form";
            req.ActionName = "Update Weekly Alarm";
            if (existingHighlight.Id == 0)
            {
                req.Message = "{\"period\" : \"\",\"processtrain\" : \"\",\"utilities\" : \"\",\"remark\" : \"\" }";
            }
            else
            {
                req.Message = existingHighlight.Message;
                req.Id = existingHighlight.Id;
            }
            dynamic jsonObj = JsonConvert.DeserializeObject(req.Message);
            jsonObj[viewModel.Property] = viewModel.Message;
            req.Message = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            var resp = _highlightService.SaveHighlight(req);
            return Json(resp);
        }

        public ActionResult UpdateWeather(WeatherViewModel viewModel)
        {
            var weather = _weatherService.GetWeather(new GetWeatherRequest
            {
                Date = viewModel.Date,
                ByDate = true
            });
            if (weather.Id == 0)
            {
                var request = viewModel.MapTo<SaveWeatherRequest>();
                request.UserId = UserProfile().UserId;
                request.ControllerName = "Der Input Form";
                request.ActionName = "Input Weather";
                var resp = _weatherService.SaveWeather(request);
                return Json(resp);
            }
            else
            {
                var request = viewModel.MapTo<SaveWeatherRequest>();
                request.Id = weather.Id;
                request.Temperature = weather.Temperature;
                request.ValueId = viewModel.ValueId;
                request.UserId = UserProfile().UserId;
                request.ControllerName = "Der Input Form";
                request.ActionName = "Update Weather";
                var resp = _weatherService.SaveWeather(request);
                return Json(resp);
            }
        }

        public ActionResult Activity()
        {
            var viewModel = new ActivityViewModel();
            return View(viewModel);
        }

        public ActionResult DerInputFileGrid(GridParams gridParams, string Date)
        {
            var derInputFiles = _derTransactionService.GetDerInputFiles(new Services.Requests.Der.GetDerInputFilesRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search,
                Date = Date
            });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = derInputFiles.TotalRecords,
                iTotalRecords = derInputFiles.DerInputFiles.Count,
                aaData = derInputFiles.DerInputFiles
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateActivity()
        {
            var viewModel = new CreateActivityViewModel();
            return View(viewModel);
        }

        [HttpPost]
        //[AuthorizeUser(AccessLevel = "AllowUpload")]
        public ActionResult UploadActivity(IEnumerable<HttpPostedFileBase> files, string date)
        {
            var theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            if(files.Count() > 0)
            {
                foreach (var file in files)
                {
                    var response = new BaseResponse();
                    var cleanName = string.Join("_", file.FileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.').Replace('&', '-');
                    var title = Path.GetFileNameWithoutExtension(cleanName) + "_" + theDate.ToString("dd-MMM-yyyy") + "_" + new Random().Next(1, 100) + Path.GetExtension(cleanName);
                    
                    string filename = title.Replace('/', '-');

                    if (!Directory.Exists(Server.MapPath(PathConstant.DerInputFile)))
                    {
                        Directory.CreateDirectory(Server.MapPath(PathConstant.DerInputFile));
                    }

                    if (file.ContentLength > 0)
                    {
                        var path = Path.Combine(Server.MapPath(PathConstant.DerInputFile), filename);
                        file.SaveAs(path);

                        response = _derTransactionService.CreateDerInputFile(new CreateDerInputFileRequest
                        {
                            FileName = PathConstant.DerInputFile + "/" + filename,
                            Date = theDate,
                            Title = file.FileName,
                            CreatedBy = UserProfile().UserId,
                            UserId = UserProfile().UserId,
                            ControllerName = "DER Input form",
                            ActionName = "Upload DER files reference"
                        });
                    }

                    TempData["Message"] = response.Message;
                    TempData["IsSuccess"] = response.IsSuccess;
                }
            }


            TempData["ExpandedFileAttachment"] = true;
            return RedirectToAction("Input", new { date = theDate.ToString("MM/dd/yyyy") });
        }

        [HttpPost]
        public ActionResult DeleteActivity(int id)
        {
            var response = _derTransactionService.DeleteDerInputFile(new Services.Requests.Der.DerDeleteRequest {
                Id = id,
                ControllerName = "DER Input",
                ActionName = "Delete Activity",
                UserId = UserProfile().UserId
            });
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            TempData["ExpandedFileAttachment"] = true;
            return RedirectToAction("Index", new { date = response.Date.ToString("MM/dd/yyyy") });
        }

        private DerValuesViewModel GetDerValuesPerSection(string date, int[] actualKpiIds, int[] targetKpiIds, int[] highlightTypeIds)
        {
            var theDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var kpiInformationValuesRequest = new GetKpiInformationValuesRequest
            {
                Date = theDate,
                ActualKpiIds = actualKpiIds,
                TargetKpiIds = targetKpiIds
            };
            var kpiInformationValuesResponse = _derTransactionService.GetKpiInformationValues(kpiInformationValuesRequest);
            var highlightValuesRequest = new GetHighlightValuesRequest
            {
                Date = theDate,
                HighlightTypeIds = highlightTypeIds
            };
            var highlightValuesResponse = _derTransactionService.GetHighlightValues(highlightValuesRequest);
            var viewModel = new DerValuesViewModel();
            viewModel.Highlights = highlightValuesResponse.Highlights.MapTo<DerValuesViewModel.DerHighlightValuesViewModel>();
            viewModel.KpiInformations = kpiInformationValuesResponse.KpiInformations.MapTo<DerValuesViewModel.KpiInformationValuesViewModel>();
            return viewModel;
        }
    }
}