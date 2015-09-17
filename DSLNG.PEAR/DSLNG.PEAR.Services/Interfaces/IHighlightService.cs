

using DSLNG.PEAR.Services.Requests.Highlight;
using DSLNG.PEAR.Services.Responses.Highlight;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IHighlightService
    {
        GetHighlightsResponse GetHighlights(GetHighlightsRequest request);
        SaveHighlightResponse SaveHighlight(SaveHighlightRequest request);
        GetReportHighlightsResponse GetReportHighlights(GetReportHighlightsRequest request);
    }
}
