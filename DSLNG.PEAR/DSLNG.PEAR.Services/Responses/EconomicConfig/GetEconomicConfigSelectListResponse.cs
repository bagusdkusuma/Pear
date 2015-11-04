using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EconomicConfig
{
    public class GetEconomicConfigSelectListResponse
    {
        public IList<Scenario> Scenarios { get; set; }
        public IList<EconomicSummary> EconomicSummaries { get; set; }

        public class Scenario
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class EconomicSummary
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
