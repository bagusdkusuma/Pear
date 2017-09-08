using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.AuditTrail;
using DSLNG.PEAR.Services.Responses.AuditTrail;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.AuditTrail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DSLNG.PEAR.Web.Controllers
{
    [Authorize]
    public class AuditTrailController : BaseController
    {
        private readonly IAuditTrailService _auditService;
        public AuditTrailController(IAuditTrailService auditService)
        {
            _auditService = auditService;
        }
        // GET: AuditTrail
        public ActionResult Index()
        {
            var viewModel = new GetAuditTrailViewModel();
            if (Request.QueryString["month"] == null)
            {
                viewModel.Month = DateTime.Now.Month;
            }
            else
            {
                viewModel.Month = int.Parse(Request.QueryString["month"]);
            }
            if (Request.QueryString["year"] == null)
            {
                viewModel.Year = DateTime.Now.Year;
            }
            else
            {
                viewModel.Year = int.Parse(Request.QueryString["year"]);
            }
            for (var i = 2011; i < 2030; i++)
            {
                viewModel.YearList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return View(viewModel);
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var audit = _auditService.GetAuditTrails(new GetAuditTrailsRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                Search = gridParams.Search,
                SortingDictionary = gridParams.SortingDictionary,
                StartDate = string.IsNullOrEmpty(Request["StartDate"]) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0) : DateTime.ParseExact(Request["StartDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture),
                EndDate = string.IsNullOrEmpty(Request["EndDate"]) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) : DateTime.ParseExact(Request["EndDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture)
            });
            IList<AuditTrailsResponse.AuditTrail> datas = audit.AuditTrails;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = audit.TotalRecords,
                iTotalRecords = audit.AuditTrails.Count,
                aaData = datas
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            var response = _auditService.GetAuditTrailDetails(id);
            //var items = new JavaScriptSerializer().DeserializeObject(response.AuditTrails[0].OldValue);
            //var items = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.AuditTrails[0].OldValue);

            var viewModel = response.MapTo<AuditTrailsDetailsViewModel>();
            return PartialView("_Details", viewModel);
        }

        public ActionResult UserLogin()
        {
            return View();
        }

        public ActionResult LoginGrid(GridParams gridParams)
        {
            var audit = _auditService.GetUserLogins(new GetAuditUserLoginsRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                Search = gridParams.Search,
                SortingDictionary = gridParams.SortingDictionary,
                StartDate = string.IsNullOrEmpty(Request["StartDate"]) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0) : DateTime.ParseExact(Request["StartDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture),
                EndDate = string.IsNullOrEmpty(Request["EndDate"]) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) : DateTime.ParseExact(Request["EndDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture)
            });
            IList<AuditUserLoginsResponse.UserLogin> datas = audit.UserLogins.ToList();
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = audit.TotalRecords,
                iTotalRecords = audit.UserLogins.Count,
                aaData = datas
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginDetails(int Id)
        {
            var data = _auditService.GetUserLogin(new GetAuditUserLoginRequest { Id = Id });
            var viewModel = data.MapTo<GetAuditUserLoginViewModel>();
            return PartialView("_LoginDetails",viewModel);
        }

        public ActionResult AuditUserGrid(GridParams gridParams, int loginId)
        {
            var audit = _auditService.GetAuditUsers(new GetAuditUsersRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                Search = gridParams.Search,
                SortingDictionary = gridParams.SortingDictionary,
                LoginId = loginId
            });

            IList<AuditUsersResponse.AuditUser> datas = audit.AuditUsers.ToList();
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = audit.TotalRecords,
                iTotalRecords = audit.AuditUsers.Count,
                aaData = datas
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        } 

        public ActionResult AuditUser(int Id)
        {
            var data = _auditService.GetUserLogin(new GetAuditUserLoginRequest { Id = Id });
            var viewModel = data.MapTo<GetAuditUserLoginViewModel>();
            return PartialView("_AuditUser", viewModel);
            //return View(viewModel);
        }
    }
}