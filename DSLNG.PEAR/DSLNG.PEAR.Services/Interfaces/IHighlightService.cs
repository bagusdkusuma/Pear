

using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Services.Responses.Highlight;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IHighlightService
    {
        GetHighlightsResponse GetHighlights(GetHighlightsRequest request);
        GetHighlightResponse GetHighlight(GetHighlightRequest request);
        GetHighlightResponse GetHighlightByPeriode(GetHighlightRequest request, bool latest = false);
        SaveHighlightResponse SaveHighlight(SaveHighlightRequest request);
        GetReportHighlightsResponse GetReportHighlights(GetReportHighlightsRequest request);
        DeleteResponse DeleteHighlight(DeleteRequest request);
        GetDynamicHighlightsResponse GetDynamicHighlights(GetDynamicHighlightsRequest request);
        GetHighlightsResponse GetHighlightsForGrid(GetHighlightsRequest request);
    }
}
