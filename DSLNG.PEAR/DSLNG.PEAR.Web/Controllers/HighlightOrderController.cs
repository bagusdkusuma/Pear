

using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.HighlightOrder;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.ViewModels.HighlightOrder;

namespace DSLNG.PEAR.Web.Controllers
{
    public class HighlightOrderController : BaseController
    {
        public IHighlightOrderService _highlightOrderService;
        public HighlightOrderController(IHighlightOrderService highlightOrderService) {
            _highlightOrderService = highlightOrderService;
        }
        public ActionResult Index()
        {
            var highlightOrder = _highlightOrderService.GetHighlights(new GetHighlightOrdersRequest());
            return View(highlightOrder.HighlightOrders.MapTo<HighlightOrderViewModel>());
        }

        [HttpPost]
        public JsonResult Save(HighlightOrderViewModel viewModel) {
            var req = viewModel.MapTo<SaveHighlightOrderRequest>();
            return Json(_highlightOrderService.SaveHighlight(req));
        }
	}
}