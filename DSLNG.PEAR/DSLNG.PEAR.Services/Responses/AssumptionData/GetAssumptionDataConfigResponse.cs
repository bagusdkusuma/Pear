using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.AssumptionData
{
    public class GetAssumptionDataConfigResponse
    {
        public IList<AssumptionDataConfig> AssumptionDataConfigs { get; set; }
        public IList<Scenario> Scenarios { get; set; }
        public class AssumptionDataConfig
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Measurement { get; set; }
        }

        public class Scenario
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
