﻿
namespace DSLNG.PEAR.Services.Requests.Buyer
{
    public class GetBuyersRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string Term { get; set; }
        public bool OnlyCount { get; set; }
    }
}
