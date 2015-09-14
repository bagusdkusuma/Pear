
namespace DSLNG.PEAR.Services.Requests.Buyer
{
    public class GetBuyersRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
    }
}
