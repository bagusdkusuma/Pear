
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Highlight
{
    public class GetReportHighlightsResponse
    {
        public IList<HighlightResponse> Highlights { get; set; }
        public class HighlightResponse {
            public string Title { get; set; }
            public string Message { get; set; }
        }
    }
}
