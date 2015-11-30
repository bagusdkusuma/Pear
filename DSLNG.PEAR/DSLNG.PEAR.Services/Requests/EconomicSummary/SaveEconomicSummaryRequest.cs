using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.EconomicSummary
{
    public class SaveEconomicSummaryRequest
    {
        public SaveEconomicSummaryRequest()
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
