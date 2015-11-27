using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.EconomicSummary
{
    public class GetEconomicSummariesRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
        public string Term { get; set; }
    }
}
