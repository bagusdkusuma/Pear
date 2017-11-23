using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Web.ViewModels.KpiTransformation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.KpiTransformation;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Requests.KpiTransformationSchedule;
using DSLNG.PEAR.Web.Scheduler;
using DSLNG.PEAR.Services.Requests.KpiTransformationLog;
using DSLNG.PEAR.Web.Attributes;
using DSLNG.PEAR.Services.Responses;

namespace DSLNG.PEAR.Web.Controllers
{
    [Authorize]
    public class KpiTransformationController : BaseController
    {
        private readonly IRoleGroupService _roleService;
        private readonly IKpiTransformationService _kpiTransformationService;
        private readonly IKpiTransformationScheduleService _kpiTransformationScheduleService;
        private readonly IKpiTransformationJob _kpiTransformationJob;
        private readonly IKpiTransformationLogService _kpiTransformationLogService;

        public KpiTransformationController(IRoleGroupService roleService,
            IKpiTransformationService kpiTransformationService,
            IKpiTransformationScheduleService kpiTransformationScheduleService,
            IKpiTransformationJob kpiTransformationJob,
            IKpiTransformationLogService kpiTransformationLogService)
        {
            _roleService = roleService;
            _kpiTransformationService = kpiTransformationService;
            _kpiTransformationScheduleService = kpiTransformationScheduleService;
            _kpiTransformationJob = kpiTransformationJob;
            _kpiTransformationLogService = kpiTransformationLogService;
        }
        // GET: KpiTransformation
        [AuthorizeUser(AccessLevel = "AllowView")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var userId = UserProfile().UserId;
            var templates = _kpiTransformationService.Get(new GetKpiTransformationsRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search,
                UserId = userId
            });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalRecords = templates.KpiTransformations.Count,
                iTotalDisplayRecords = templates.TotalRecords,
                aaData = templates.KpiTransformations
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [AuthorizeUser(AccessLevel = "AllowCreate")]
        public ActionResult Create()
        {
            var viewModel = new KpiTransformationViewModel()
            {
                RoleGroupOptions = new MultiSelectList(_roleService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
                {
                    Take = -1,
                    SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
                }).RoleGroups, "Id", "Name")
            };
            SetPeriodeTypes(viewModel.PeriodeTypes);
            ViewBag.Title = "Create Transformation Schedule";
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeUser(AccessLevel = "AllowCreate")]
        public ActionResult Create(KpiTransformationViewModel viewModel)
        {
            return Save(viewModel);
        }

        [HttpPost]
        [AuthorizeUser(AccessLevel = "AllowDelete")]
        public ActionResult Delete(int id)
        {
            var response = new BaseResponse();
            response = _kpiTransformationService.Delete(new DeleteTransformationRequest {
                Id = id,
                UserId = this.UserProfile().UserId,
                ControllerName = "KPI Transformation",
                ActionName = "Delete"
            });
            @TempData["IsSuccess"] = response.IsSuccess;
            @TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }
        [AuthorizeUser(AccessLevel = "AllowUpdate")]
        public ActionResult Edit(int id)
        {
            var viewModel = _kpiTransformationService.Get(id).MapTo<KpiTransformationViewModel>();
            viewModel.RoleGroupOptions = new MultiSelectList(_roleService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            }).RoleGroups, "Id", "Name", viewModel.RoleGroupIds);
            SetPeriodeTypes(viewModel.PeriodeTypes);
            ViewBag.Title = "Edit Transformation Schedule: " + viewModel.Name;
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeUser(AccessLevel = "AllowUpdate")]
        public ActionResult Edit(KpiTransformationViewModel viewModel)
        {
            return Save(viewModel);
        }

        [AuthorizeUser(AccessLevel = "AllowInput")]
        public ActionResult Process(int id)
        {
            var viewModel = _kpiTransformationService.Get(id).MapTo<KpiTransformationViewModel>();
            SetPeriodeTypes(viewModel.PeriodeTypes);
            SetProcessingTypes(viewModel.ProcessingTypes);
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeUser(AccessLevel = "AllowInput")]
        public ActionResult Process(KpiTransformationScheduleViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveKpiTransformationScheduleRequest>();
            request.UserId = UserProfile().UserId;
            request.ControllerName = "KPI Transformation Scheduler";
            request.ActionName = "Process KPI Transformation Scheduler";
            var response = _kpiTransformationScheduleService.Save(request);
            _kpiTransformationJob.Process(response);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AuthorizeUser(AccessLevel = "AllowInput")]
        public ActionResult ProcessDefault(int Id, string date)
        {
            var data = _kpiTransformationService.Get(Id).MapTo<KpiTransformationViewModel>();
            var viewModel = new KpiTransformationScheduleViewModel();
            viewModel = data.MapTo<KpiTransformationScheduleViewModel>();
            viewModel.KpiTransformationId = data.Id;
            viewModel.StartInDisplay = date.ToString();
            viewModel.EndInDisplay = date.ToString();
            var request = viewModel.MapTo<SaveKpiTransformationScheduleRequest>();
            request.UserId = UserProfile().UserId;
            var response = _kpiTransformationScheduleService.Save(request);
            _kpiTransformationJob.Process(response);
            return Json(response.Message, JsonRequestBehavior.AllowGet);
        }

        private ActionResult Save(KpiTransformationViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveKpiTransformationRequest>();
            req.UserId = this.UserProfile().UserId;
            req.ControllerName = "KPI Transformation";
            req.ActionName = "Save";
            var resp = _kpiTransformationService.Save(req);
            TempData["IsSuccess"] = resp.IsSuccess;
            TempData["Message"] = resp.Message;
            return RedirectToAction("Index");
        }
        [AuthorizeUser(AccessLevel = "AllowView")]
        public ActionResult Log(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        [AuthorizeUser(AccessLevel = "AllowDelete")]
        public ActionResult DeleteSchedule(int id)
        {
            int logId = 0;
            var log = _kpiTransformationScheduleService.Get(id);
            logId = log.KpiTransformation_Id;

            var result = _kpiTransformationScheduleService.Delete(new DeleteKPITransformationScheduleRequest {
                Id = id,
                UserId = this.UserProfile().UserId,
                ControllerName = "KPI Transformation Schedule",
                ActionName = "Delete Transformation Schedule"
            });

            @TempData["IsSuccess"] = result.IsSuccess;
            @TempData["Message"] = result.Message;
            return RedirectToAction("Log", new { id = logId });
        }
        public ActionResult LogGrid(int id, GridParams gridParams)
        {
            var templates = _kpiTransformationScheduleService.Get(new GetKpiTransformationSchedulesRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search,
                KpiTransformationId = id
            });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalRecords = templates.Schedules.Count,
                iTotalDisplayRecords = templates.TotalRecords,
                aaData = templates.Schedules
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [AuthorizeUser(AccessLevel = "AllowView")]
        public ActionResult LogDetails(int id)
        {
            ViewBag.Id = id;
            var log = _kpiTransformationScheduleService.Get(id);
            ViewBag.LogId = log.KpiTransformation_Id;
            return View();
        }

        public ActionResult LogDetailsGrid(int id, GridParams gridParams)
        {
            var templates = _kpiTransformationLogService.Get(new GetKpiTransformationLogsRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search,
                ScheduleId = id
            });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalRecords = templates.Logs.Count,
                iTotalDisplayRecords = templates.TotalRecords,
                aaData = templates.Logs
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        private void SetPeriodeTypes(IList<SelectListItem> periodeTypes)
        {
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly") && !name.Equals("Itd"))
                {
                    periodeTypes.Add(new SelectListItem { Text = name, Value = ((int)(PeriodeType)Enum.Parse(typeof(PeriodeType), name)).ToString() });
                }
            }
        }
        private void SetProcessingTypes(IList<SelectListItem> processingTypes)
        {
            foreach (var name in Enum.GetNames(typeof(ProcessingType)))
            {
                processingTypes.Add(new SelectListItem { Text = name, Value = ((int)(ProcessingType)Enum.Parse(typeof(ProcessingType), name)).ToString() });
            }
        }

    }
}