using System;
using System.Collections.Generic;
using System.Linq;

namespace DSLNG.PEAR.Services.Requests.AuditTrail
{
    public class GetAuditUserLoginsRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
