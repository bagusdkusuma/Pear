using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.EconomicConfig
{
    public class SaveEconomicConfigRequest
    {
        public int Id { get; set; }
        public int IdScenario { get; set; }
        public int IdEconomicSummary { get; set; }
        public bool IsActive { get; set; }

    }
}
