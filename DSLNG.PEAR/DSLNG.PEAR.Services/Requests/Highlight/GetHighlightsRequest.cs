

namespace DSLNG.PEAR.Services.Requests.Highlight
{
    public class GetHighlightsRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
    }
}
