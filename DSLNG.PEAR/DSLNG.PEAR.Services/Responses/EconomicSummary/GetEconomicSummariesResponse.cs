using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EconomicSummary
{
    public class GetEconomicSummariesResponse
    {
        public IList<EconomicSummary> EconomicSummaries { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class EconomicSummary
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public bool IsActive { get; set; }
            public string Scenarios { get; set; }
        }
    }
}
