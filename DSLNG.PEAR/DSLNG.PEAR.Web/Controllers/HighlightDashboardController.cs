

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
        public IHighlightOrderService _highlightOrderService;
        private IHighlightGroupService _highlightGroupService;
        public HighlightDashboardController(IHighlightOrderService highlightOrderService, IHighlightGroupService highlightGroupService) {
            _highlightOrderService = highlightOrderService;
            _highlightGroupService = highlightGroupService;
        }
        public ActionResult Index()
        {
            //var highlightOrder = _highlightOrderService.GetHighlights(new GetHighlightOrdersRequest());
            var viewModel = new HighlightOrderViewModel();
            viewModel.Groups = _highlightGroupService.GetHighlightGroups(new GetHighlightGroupsRequest
            {
                Take = -1,
                SortingDictionary = new Dictionary<string, SortOrder>{{"Order",SortOrder.Ascending}}
            }).HighlightGroups.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            viewModel.Groups.Insert(0, new SelectListItem { Value = "0", Text = "Choose Group" });
            return View(viewModel);
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var templates = _highlightOrderService.GetHighlights(new GetHighlightOrdersRequest
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
        public JsonResult Save(HighlightOrderViewModel viewModel) {
            var req = viewModel.MapTo<SaveHighlightOrderRequest>();
            return Json(_highlightOrderService.SaveHighlight(req));
        }
	}
}