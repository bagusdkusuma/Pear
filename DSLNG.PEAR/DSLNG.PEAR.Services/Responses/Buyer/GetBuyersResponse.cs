

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Buyer
{
    public class GetBuyersResponse
    {
        public IList<BuyerResponse> Buyers { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class BuyerResponse {
            public int id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
