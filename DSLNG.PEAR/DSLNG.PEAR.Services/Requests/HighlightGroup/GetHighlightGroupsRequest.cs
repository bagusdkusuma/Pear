
namespace DSLNG.PEAR.Services.Requests.HighlightGroup
{
    public class GetHighlightGroupsRequest : GridBaseRequest
    {

        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
        public bool OnlyIsActive { get; set; }
    }
}
