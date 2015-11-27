using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EconomicSummary
{
    public class GetEconomicSummaryResponse
    {
        public GetEconomicSummaryResponse()
        {
            Scenarios = new List<Scenario>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public bool IsActive { get; set; }
        public IList<Scenario> Scenarios { get; set; }
        public class Scenario
        {
            public int Id { get; set; }
            public string Name { get; set; }     
        }
    }
}
