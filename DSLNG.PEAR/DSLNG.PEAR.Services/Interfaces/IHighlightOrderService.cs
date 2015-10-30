

using DSLNG.PEAR.Services.Requests.HighlightOrder;
using DSLNG.PEAR.Services.Responses.HighlightOrder;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IHighlightOrderService
    {
        GetHighlightOrdersResponse GetHighlights(GetHighlightOrdersRequest request);
        SaveHighlightOrderResponse SaveHighlight(SaveHighlightOrderRequest request);
    }
}
