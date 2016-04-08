

using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.HighlightOrder;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.ViewModels.HighlightOrder;
using DSLNG.PEAR.Web.Grid;
using System.Collections.Generic;
using DSLNG.PEAR.Services.Requests.HighlightGroup;
using System.Linq;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Web.Controllers
{
    public class HighlightDashboardController : BaseController
    {
        public IHighlightOrderService HighlightOrderService;
        private readonly IHighlightGroupService _highlightGroupService;
        private readonly IRoleGroupService _roleService;
        public HighlightDashboardController(IHighlightOrderService highlightOrderService,
            IHighlightGroupService highlightGroupService,
            IRoleGroupService roleGroupService)
        {
            HighlightOrderService = highlightOrderService;
            _highlightGroupService = highlightGroupService;
            _roleService = roleGroupService;
        }
        public ActionResult Index()
        {
            //var highlightOrder = _highlightOrderService.GetHighlights(new GetHighlightOrdersRequest());
            var viewModel = new HighlightOrderViewModel();
            viewModel.Groups = _highlightGroupService.GetHighlightGroups(new GetHighlightGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Order", SortOrder.Ascending } }
            }).HighlightGroups.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            viewModel.Groups.Insert(0, new SelectListItem { Value = "0", Text = "Choose Group" });

            viewModel.RoleGroupOptions = _roleService.GetRoleGroups(new Services.Requests.RoleGroup.GetRoleGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder> { { "Name", SortOrder.Ascending } }
            }).RoleGroups.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            return View(viewModel);
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var templates = HighlightOrderService.GetHighlights(new GetHighlightOrdersRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                SortingDictionary = gridParams.SortingDictionary,
                Search = gridParams.Search
            });

            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalRecords = templates.HighlightOrders.Count,
                iTotalDisplayRecords = templates.TotalRecords,
                aaData = templates.HighlightOrders
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Save(HighlightOrderViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveHighlightOrderRequest>();
            return Json(HighlightOrderService.SaveHighlight(req));
        }
        [HttpPost]
        public JsonResult SaveStatic(HighlightOrderViewModel viewModel)
        {
            var req = viewModel.MapTo<SaveStaticHighlightOrderRequest>();
            return Json(HighlightOrderService.SaveStaticHighlight(req));
        }
    }
}