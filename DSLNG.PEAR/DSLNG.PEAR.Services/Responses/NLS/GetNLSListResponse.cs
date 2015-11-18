

using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.NLS
{
    public class GetNLSListResponse
    {
        public IList<NLSResponse> NLSList { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class NLSResponse {
            public int Id { get; set; }
            public string Vessel { get; set; }
            public DateTime ETA { get; set; }
            public DateTime ETD { get; set; }
            public string Remark { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
