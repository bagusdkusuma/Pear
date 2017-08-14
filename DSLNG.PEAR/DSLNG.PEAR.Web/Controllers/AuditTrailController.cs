using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.AuditTrail;
using DSLNG.PEAR.Services.Responses.AuditTrail;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.AuditTrail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                SortingDictionary = gridParams.SortingDictionary
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

        public JsonResult Details()
        {
            var data = "";
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}