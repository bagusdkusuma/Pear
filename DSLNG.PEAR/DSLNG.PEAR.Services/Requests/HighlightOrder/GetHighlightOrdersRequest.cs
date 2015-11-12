

namespace DSLNG.PEAR.Services.Requests.HighlightOrder
{
    public class GetHighlightOrdersRequest : GridBaseRequest
    {
        public string Term { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
    }
}
