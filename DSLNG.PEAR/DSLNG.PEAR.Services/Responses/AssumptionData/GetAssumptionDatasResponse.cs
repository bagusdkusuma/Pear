using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.AssumptionData
{
    public class GetAssumptionDatasResponse
    {
        public IList<AssumptionData> AssumptionDatas { get; set; }
        public int Count { get; set;  }
        public int TotalRecords { get; set; }
        public class AssumptionData
        {
            public int Id { get; set; }
            public string Scenario { get; set; }
            public string Config { get; set; }
            public double ActualValue { get; set; }
            public double ForecastValue { get; set; }
            public string Remark { get; set; }
        }
    }
}
