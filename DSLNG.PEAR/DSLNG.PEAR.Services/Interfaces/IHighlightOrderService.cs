

using DSLNG.PEAR.Services.Requests.HighlightOrder;
using DSLNG.PEAR.Services.Responses.HighlightOrder;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IHighlightOrderService
    {
        GetHighlightOrdersResponse GetHighlights(GetHighlightOrdersRequest request);
        GetStaticHighlightOrdersResponse GetStaticHighlights(GetStaticHighlightOrdersRequest request);
        SaveHighlightOrderResponse SaveHighlight(SaveHighlightOrderRequest request);
        SaveStaticHighlightOrderResponse SaveStaticHighlight(SaveStaticHighlightOrderRequest request);
    }
}
