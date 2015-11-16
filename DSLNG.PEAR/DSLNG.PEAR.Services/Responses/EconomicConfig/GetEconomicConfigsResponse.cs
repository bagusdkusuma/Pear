using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EconomicConfig
{
    public class GetEconomicConfigsResponse
    {
        public IList<EconomicConfig> EconomicConfigs { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class EconomicConfig
        {
            public int Id { get; set; }
            public string Scenario { get; set; }
            public string EconomicSummary { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
