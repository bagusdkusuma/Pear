using DevExpress.Web.Mvc;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Web.ViewModels.Highlight;
using System;
using System.Web.Mvc;
using PeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.Select;
using System.Linq;
using DSLNG.PEAR.Services.Requests.NLS;
using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Services.Requests.HighlightOrder;
using System.Data.SqlClient;
using System.Collections.Generic;
using DSLNG.PEAR.Services.Requests.HighlightGroup;
using System.Globalization;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Common.Contants;

namespace DSLNG.PEAR.Web.Controllers
{
    public class HighlightController : BaseController
    {
        private IHighlightService _highlightService;
        private ISelectService _selectService;
        private INLSService _nlsService;
        private IVesselScheduleService _vesselScheduleService;
        private IWeatherService _waetherService;
        private IHighlightOrderService _highlightOrderService;
        private IHighlightGroupService _highlightGroupService;

        public HighlightController(IHighlightService highlightService,
            ISelectService selectService,
            INLSService nlsService,
            IVesselScheduleService vesselScheduleService,
            IWeatherService weatherService,
            IHighlightOrderService highlightOrderService,
            IHighlightGroupService highlightGroupService
            )
        {
            _highlightService = highlightService;
            _selectService = selectService;
            _nlsService = nlsService;
            _vesselScheduleService = vesselScheduleService;
            _waetherService = weatherService;
            _highlightOrderService = highlightOrderService;
            _highlightGroupService = highlightGroupService;
        }
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
        public ActionResult MonthlyPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridArtifactIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return MonthlyBindingCore(viewModel);
        }
        public ActionResult YearlyPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridArtifactIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return YearlyBindingCore(viewModel);
        }

        PartialViewResult BindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                GetDataRowCount,
                GetData
            );
            return PartialView("_IndexGridPartial", gridViewModel);
        }
        PartialViewResult MonthlyBindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                MonthlyGetDataRowCount,
                MonthlyGetData
            );
            return PartialView("_MonthlyGridPartial", gridViewModel);
        }

        PartialViewResult YearlyBindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                YearlyGetDataRowCount,
                YearlyGetData
            );
            return PartialView("_YearlyGridPartial", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("PeriodeType");
            viewModel.Columns.Add("Type");
            viewModel.Columns.Add("Title");
            viewModel.Columns.Add("Date");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridHighlightIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _highlightService.GetHighlights(new GetHighlightsRequest { PeriodeType = PeriodeType.Daily, OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _highlightService.GetHighlights(new GetHighlightsRequest
            {
                PeriodeType = PeriodeType.Daily,
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Highlights;
        }
        public void MonthlyGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _highlightService.GetHighlights(new GetHighlightsRequest { PeriodeType = PeriodeType.Monthly, OnlyCount = true }).Count;
        }

        public void MonthlyGetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _highlightService.GetHighlights(new GetHighlightsRequest
            {
                PeriodeType = PeriodeType.Monthly,
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Highlights;
        }
        public void YearlyGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _highlightService.GetHighlights(new GetHighlightsRequest { PeriodeType = PeriodeType.Yearly, OnlyCount = true }).Count;
        }

        public void YearlyGetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _highlightService.GetHighlights(new GetHighlightsRequest
            {
                PeriodeType = PeriodeType.Yearly,
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Highlights;
        }

        //
        // GET: /Highlight/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Highlight/Create
        public ActionResult Create()
        {
            ViewBag.TurnOffDevexpress = true;
            var viewModel = new HighlightViewModel();
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            viewModel.Types = _selectService.GetSelect(new GetSelectRequest { Name = "highlight-types" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Id.ToString() }).ToList();
            //viewModel.AlertConditions = _selectService.GetSelect(new GetSelectRequest { Name = "alert-conditions" }).Options
            //    .Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();

            var TypeId = string.IsNullOrEmpty(Request.QueryString["TypeId"]) ? 0 : int.Parse(Request.QueryString["TypeId"]);
            var PeriodeType = string.IsNullOrEmpty(Request.QueryString["PeriodeType"]) ? "Daily"
                : Request.QueryString["PeriodeType"];
            var periodeQS = !string.IsNullOrEmpty(Request.QueryString["Periode"]) ? Request.QueryString["Periode"] : null;
            viewModel.TypeId = TypeId;
            viewModel.PeriodeType = PeriodeType;
            switch (PeriodeType)
            {
                case "Monthly":
                    viewModel.PeriodeType = PeriodeType;
                    if (!string.IsNullOrEmpty(periodeQS))
                    {
                        viewModel.Date = DateTime.ParseExact("01/" + periodeQS, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }

                    break;
                case "Yearly":
                    viewModel.PeriodeType = PeriodeType;
                    if (!string.IsNullOrEmpty(periodeQS))
                    {
                        viewModel.Date = DateTime.ParseExact("01/01/" + periodeQS, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }

                    break;
                default:
                    viewModel.PeriodeType = PeriodeType;
                    if (!string.IsNullOrEmpty(periodeQS))
                    {
                        viewModel.Date = DateTime.ParseExact(periodeQS, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }

                    break;
            }

            return View(viewModel);
        }

        //
        // POST: /Highlight/Create
        [HttpPost]
        public ActionResult Create(HighlightViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveHighlightRequest>();
            var resp = _highlightService.SaveHighlight(req);
            TempData["IsSuccess"] = resp.IsSuccess;
            TempData["Message"] = resp.Message;
            if (resp.IsSuccess)
            {
                return RedirectToAction("Display");
            }
            return RedirectToAction("Create");
        }

        //
        // GET: /Highlight/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.TurnOffDevexpress = true;
            var viewModel = _highlightService.GetHighlight(new GetHighlightRequest { Id = id }).MapTo<HighlightViewModel>();
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            viewModel.Types = _selectService.GetSelect(new GetSelectRequest { Name = "highlight-types" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Id.ToString() }).ToList();
            return View(viewModel);
        }

        //
        // POST: /Highlight/Edit/5
        [HttpPost]
        public ActionResult Edit(HighlightViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveHighlightRequest>();
            var resp = _highlightService.SaveHighlight(req);
            TempData["IsSuccess"] = resp.IsSuccess;
            TempData["Message"] = resp.Message;
            if (resp.IsSuccess)
            {
                return RedirectToAction("Display");
            }
            return RedirectToAction("Edit", new { id = viewModel.Id });
        }

        // GET: /Highlight/Edit/5
        public ActionResult Manage()
        {
            var viewModel = new HighlightViewModel();
            var id = Request.QueryString.AllKeys.Contains("HighlightId") ? int.Parse(Request.QueryString["HighlightId"]) : 0;
            if (id == 0)
            {
                var TypeId = string.IsNullOrEmpty(Request.QueryString["TypeId"]) ? 0 : int.Parse(Request.QueryString["TypeId"]);
                var PeriodeType = string.IsNullOrEmpty(Request.QueryString["PeriodeType"]) ? "Daily"
                    : Request.QueryString["PeriodeType"];
                var periodeQS = !string.IsNullOrEmpty(Request.QueryString["Periode"]) ? Request.QueryString["Periode"] : null;
                viewModel.TypeId = TypeId;
                viewModel.PeriodeType = PeriodeType;
                switch (PeriodeType)
                {
                    case "Monthly":
                        viewModel.PeriodeType = PeriodeType;
                        if (!string.IsNullOrEmpty(periodeQS))
                        {
                            viewModel.Date = DateTime.ParseExact("01/" + periodeQS, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                        break;
                    case "Yearly":
                        viewModel.PeriodeType = PeriodeType;
                        if (!string.IsNullOrEmpty(periodeQS))
                        {
                            viewModel.Date = DateTime.ParseExact("01/01/" + periodeQS, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                        break;
                    default:
                        viewModel.PeriodeType = PeriodeType;
                        if (!string.IsNullOrEmpty(periodeQS))
                        {
                            viewModel.Date = DateTime.ParseExact(periodeQS, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }

                        break;
                }
            }
            else
            {
                ViewBag.TurnOffDevexpress = true;
                viewModel = _highlightService.GetHighlight(new GetHighlightRequest { Id = id }).MapTo<HighlightViewModel>();
            }
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            viewModel.Types = _selectService.GetSelect(new GetSelectRequest { Name = "highlight-types" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Id.ToString() }).ToList();
            return View(viewModel);
        }
        // POST: /Highlight/Edit/5
        [HttpPost]
        public ActionResult Manage(HighlightViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveHighlightRequest>();
            if (viewModel.AsNew) {
                req.Id = 0;
            }
            var resp = _highlightService.SaveHighlight(req);
            TempData["IsSuccess"] = resp.IsSuccess;
            TempData["Message"] = resp.Message;
            if (resp.IsSuccess)
            {
                return RedirectToAction("Display");
            }
            return Redirect(Request.UrlReferrer.ToString());
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            _highlightService.DeleteHighlight(new DeleteRequest { Id = id });
            return RedirectToAction("Index");
        }

        public ActionResult Display()
        {
            //var nlsList = _nlsService.GetNLSList(new GetNLSListRequest { TheActiveOnes = true });
            var vesselSchedules = _vesselScheduleService.GetVesselSchedules(new GetVesselSchedulesRequest
            {
                allActiveList = true,
                Skip = 0,
                Take = 20,
            });
            var viewModel = new DailyExecutionReportViewModel();
            viewModel.CurrentUserRoleId = UserProfile().RoleId;
             var staticHighlightsResp = _highlightOrderService.GetStaticHighlights(new GetStaticHighlightOrdersRequest { Take = -1 });
             viewModel.IsAuthorizedToManageAlert = staticHighlightsResp.HighlightOrders.First(x => x.Name == "Weather").RoleGroupIds.Contains(viewModel.CurrentUserRoleId);
             viewModel.IsAuthorizedToManageVesselSchedule = staticHighlightsResp.HighlightOrders.First(x => x.Name == "Vessel Schedule").RoleGroupIds.Contains(viewModel.CurrentUserRoleId);
             viewModel.IsAuthorizedToManageAlert = staticHighlightsResp.HighlightOrders.First(x => x.Name == "Alert").RoleGroupIds.Contains(viewModel.CurrentUserRoleId);
            var periodeTypeQS = !string.IsNullOrEmpty(Request.QueryString["PeriodeType"]) ? Request.QueryString["PeriodeType"].ToLower() : "daily";
            var periodeQS = !string.IsNullOrEmpty(Request.QueryString["Periode"]) ? Request.QueryString["Periode"] : null;
            switch (periodeTypeQS)
            {
                case "monthly":
                    viewModel.PeriodeType = PeriodeType.Monthly;
                    if (!string.IsNullOrEmpty(periodeQS))
                    {
                        viewModel.Periode = DateTime.ParseExact("01/" + periodeQS, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (viewModel.Periode.Value.Month == DateTime.Now.Month
                       && viewModel.Periode.Value.Year == DateTime.Now.Year)
                        {
                            viewModel.Periode = null;
                        }
                    }

                    break;
                case "yearly":
                    viewModel.PeriodeType = PeriodeType.Yearly;
                    if (!string.IsNullOrEmpty(periodeQS))
                    {
                        viewModel.Periode = DateTime.ParseExact("01/01/" + periodeQS, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (viewModel.Periode.Value.Year == DateTime.Now.Year)
                        {
                            viewModel.Periode = null;
                        }
                    }

                    break;
                default:
                    viewModel.PeriodeType = PeriodeType.Daily;
                    if (!string.IsNullOrEmpty(periodeQS))
                    {
                        viewModel.Periode = DateTime.ParseExact(periodeQS, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        if (viewModel.Periode.Value.Day == DateTime.Now.Day
                        && viewModel.Periode.Value.Month == DateTime.Now.Month
                        && viewModel.Periode.Value.Year == DateTime.Now.Year)
                        {
                            viewModel.Periode = null;
                        }
                    }

                    break;
            }

            viewModel.NLSList = vesselSchedules.VesselSchedules.MapTo<DailyExecutionReportViewModel.NLSViewModel>();
            viewModel.Weather = _waetherService.GetWeather(new GetWeatherRequest { Date = viewModel.Periode, ByDate = true }).MapTo<DailyExecutionReportViewModel.WeatherViewModel>();

            viewModel.HighlightGroupTemplates = _highlightGroupService.GetHighlightGroups(new GetHighlightGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Order", SortOrder.Ascending } },
                OnlyIsActive = true
            }).HighlightGroups.MapTo<DailyExecutionReportViewModel.HighlightGroupViewModel>();
            var dynamicHighlights = _highlightService.GetDynamicHighlights(new GetDynamicHighlightsRequest
            {
                PeriodeType = viewModel.PeriodeType,
                Periode = viewModel.Periode
            });
            viewModel.HighlightGroups = dynamicHighlights.HighlightGroups.MapTo<DailyExecutionReportViewModel.HighlightGroupViewModel>();

            //viewModel.Highlights = _highlightService.GetHighlights(new GetHighlightsRequest { Except = new string[1] { "Alert"}, Date = DateTime.Now.Date, IsActive = true }).Highlights.MapTo<DailyExecutionReportViewModel.HighlightViewModel>();
            //viewModel.PlantOperations = _highlightService.GetHighlights(new GetHighlightsRequest { Include = new string[4] { "Process Train", "Storage And Loading", "Utility", "Upstream" }, Date = DateTime.Now.Date, IsActive = true }).Highlights.MapTo<DailyExecutionReportViewModel.HighlightViewModel>();
            viewModel.Alert = _highlightService.GetHighlight(new GetHighlightRequest { Type = "Alert", Date = viewModel.Periode }).MapTo<DailyExecutionReportViewModel.AlertViewModel>();
            if (viewModel.Alert.TypeId == 0) {
                viewModel.Alert.TypeId = _selectService.GetSelect(new GetSelectRequest { Name = "Alert" }).Id;
            }
            var highlightOrders = _highlightOrderService.GetHighlights(new GetHighlightOrdersRequest { Take = -1, SortingDictionary = new Dictionary<string, SortOrder> { { "Order", SortOrder.Ascending } } });
            foreach (var highlight in highlightOrders.HighlightOrders)
            {
                var highlightVM = viewModel.Highlights.FirstOrDefault(x => x.Type == highlight.Value);
                if (highlightVM != null)
                {
                    highlightVM.Order = highlight.Order;
                }
            }
            viewModel.Highlights = viewModel.Highlights.OrderBy(x => x.Order).ToList();
            if (!viewModel.Periode.HasValue)
            {
                viewModel.Periode = dynamicHighlights.Periode;
            }
            return View(viewModel);
        }

        public JsonResult MessageOptions()
        {
            var parentOptionId = int.Parse(Request.QueryString["value"]);
            var select = _selectService.GetSelect(new GetSelectRequest { ParentName = "highlight-types", ParentOptionId = parentOptionId });
            if (select != null)
            {
                return Json(select.Options, JsonRequestBehavior.AllowGet);
            }
            return Json(new string[0] { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Grid(GridParams gridParams, string periodeType)
        {
            var highlight = _highlightService.GetHighlightsForGrid(new GetHighlightsRequest
                {
                    Skip = gridParams.DisplayStart,
                    Take = gridParams.DisplayLength,
                    Search = gridParams.Search,
                    SortingDictionary = gridParams.SortingDictionary
                });
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = highlight.TotalRecords,
                iTotalRecords = highlight.Highlights.Count,
                aaData = highlight.Highlights.Where(x => x.PeriodeType == (PeriodeType)Enum.Parse(typeof(PeriodeType),periodeType)).Select(x => new
                {
                    x.Id,
                    Date = x.Date.ToString(DateFormat.DateForGrid),
                    x.IsActive,
                    PeriodeType = x.PeriodeType.ToString(),
                    x.Type,
                    x.Title,
                    x.Message
                })
            };

            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult View(int typeId, string periodeType) {
            var periodeTypeEnum = (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);
            var date = DateTime.Now.Date;
            switch (periodeTypeEnum) {
                case PeriodeType.Monthly:
                    date = new DateTime(date.Year, date.Month, 1);
                    break;
                case PeriodeType.Yearly:
                    date = new DateTime(date.Year, 1, 1);
                    break;
            }
            var highlight = _highlightService.GetHighlightByPeriode(new GetHighlightRequest {
                Date = date,
                HighlightTypeId = typeId,
                PeriodeType = periodeTypeEnum
            }, true);

            var jsonResult = Json(highlight, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }       
    }
}
