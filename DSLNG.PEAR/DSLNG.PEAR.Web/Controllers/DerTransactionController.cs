using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.DerTransaction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.ViewModels.DerTransaction;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using DSLNG.PEAR.Services.Requests.KpiTarget;
using DSLNG.PEAR.Web.ViewModels.Highlight;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.Highlight;
using Newtonsoft.Json;

namespace DSLNG.PEAR.Web.Controllers
{
    public class DerTransactionController : BaseController
    {
        private readonly IDerService _derService;
        private readonly IDerTransactionService _derTransactionService;
        private readonly IKpiAchievementService _kpiAchievementService;
        private readonly IKpiTargetService _kpiTargetService;
        private readonly IHighlightService _highlightService;

        public DerTransactionController(IDerService derService,IDerTransactionService derTransactionService, IKpiAchievementService kpiAchievementService, IKpiTargetService kpiTargetService, IHighlightService highlightService) {
            _derService = derService;
            _derTransactionService = derTransactionService;
            _kpiAchievementService = kpiAchievementService;
            _kpiTargetService = kpiTargetService;
            _highlightService = highlightService;
        }
        // GET: DerTransaction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Input()
        {
            return View();
        }

        public ActionResult EconomicIndicator(string date)
        {
            return View(GetDerValuesPerSection(date, 
                new int[] { 386, 79, 80, 128, 388, 389, 390, 383, 64, 384, 65, 385, 62, 63, 391, 392, 393, 394, 395, 397 }, //actual KpiIds 
                new int[] { }, //target KpiIds
                new int[] { 15,68 }  //highlightTypeIds
                ));
        }
        public ActionResult ForcastedIndicator(string date)
        {
            return View(GetDerValuesPerSection(date,
                new int[] { 386, 79, 80, 128, 388, 389, 390, 383, 64, 384, 65, 385, 62, 63, 391, 392, 393, 394, 395, 397 }, //actual KpiIds 
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
                    45,46,47,49,50,48,71,72,73,77,74,75,76,82,7,8,82,76
                }, //actual KpiIds 
                new int[] { 10, 9, 53, 12, 169, 174,166,171 }, //target KpiIds
                new int[] { 19,12,26}  //highlightTypeIds
                ));
        }

        public ActionResult MarineShipping(string date)
        {
            return View(GetDerValuesPerSection(date,
                new int[] { 375, 376, 377, 378 }, //actual KpiIds 
                new int[] { 375, 377 }, //target KpiIds
                new int[] { 52 }  //highlightTypeIds
                ));
        }

        public ActionResult MaintenanceSection(string date) {
            return View(GetDerValuesPerSection(date,
               new int[] { 59, 60, 61,371,372,373,374 }, //actual KpiIds 
               new int[] { 59}, //target KpiIds
               new int[] { 46,34,35,36,37,31,32,33,28,29,30,38,39,47,40,41,42,43,48,44,45,49,50,51,11}  //highlightTypeIds
               ));
        }

        public ActionResult QhsseSection(string date) {
            return View(GetDerValuesPerSection(date,
               new int[] { 273, 274,275,276,1,177,278,277,285,356,4,359,286,292 }, //actual KpiIds 
               new int[] { 1, 177,278,277,276,285}, //target KpiIds
               new int[] { 18, 13}  //highlightTypeIds
               ));
        }

        public ActionResult EnablerSection(string date) {
            return View(GetDerValuesPerSection(date,
           new int[] { 379,380,36 }, //actual KpiIds 
           new int[] { }, //target KpiIds
           new int[] { 66,53,14,8 }  //highlightTypeIds
           ));
        }
        
        [HttpPost]
        public ActionResult UpdateKpi(UpdateKpiOriginalViewModel viewModel) {
            var sPeriodeType = viewModel.Type.Split('-')[0];
            var periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), sPeriodeType, true);
            var theDate = DateTime.ParseExact(viewModel.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            switch (periodeType) {
                case PeriodeType.Monthly:
                    theDate = new DateTime(theDate.Year, theDate.Month, 1);
                    break;
                case PeriodeType.Yearly:
                    theDate = new DateTime(theDate.Year, 1, 1);
                    break;
            }
            switch (viewModel.Type) {
                case "daily-actual":
                case "monthly-actual":
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
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null
                        };
                        var resp = _kpiAchievementService.UpdateOriginalData(request);
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
                            Remark = viewModel.ValueType == "remark" ? viewModel.Value : null
                        };
                        var resp = _kpiTargetService.UpdateOriginalData(request);
                        return Json(resp);
                    }
            }
            
        }

        public ActionResult UpdateHighlight(HighlightViewModel viewModel) {
            var req = viewModel.MapTo<SaveHighlightRequest>();
            var resp = _highlightService.SaveHighlight(req);
            return Json(resp);
        }

        public ActionResult UpdateInfraGSM(HighlightViewModel viewModel)
        {
            var existingHighlight = _highlightService.GetHighlightByPeriode(new GetHighlightRequest {
                Date = viewModel.Date,
                HighlightTypeId = viewModel.TypeId
            });
            SaveHighlightRequest req = new SaveHighlightRequest();
            req.PeriodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), viewModel.PeriodeType, true);
            req.Date = viewModel.Date.Value;
            req.TypeId = viewModel.TypeId;
            if (existingHighlight.Id == 0)
            {
                req.Message = "{\"a\" : \"\",\"b\" : \"\",\"c\" : \"\",\"d\" : \"\" }";
            }
            else {
                req.Message = existingHighlight.Message;
                req.Id = existingHighlight.Id;
            }
            dynamic jsonObj = JsonConvert.DeserializeObject(req.Message);
            jsonObj[viewModel.Property] = viewModel.Message;
            req.Message = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            var resp = _highlightService.SaveHighlight(req);
            return Json(resp);
        }

        private DerValuesViewModel GetDerValuesPerSection(string date, int[] actualKpiIds, int[] targetKpiIds, int[] highlightTypeIds) {
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