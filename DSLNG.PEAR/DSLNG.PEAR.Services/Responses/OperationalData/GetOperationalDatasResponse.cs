using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.OperationalData
{
    public class GetOperationalDatasResponse
    {
        public IList<OperationalData> OperationalDatas { get; set; }
        public int Count { get; set; }
        public class OperationalData
        {
            public int Id { get; set; }
            public string KeyOperation { get; set; }
            public string KPI { get; set; }
            public double ActualValue { get; set; }
            public double ForecastValue {get; set;}
            public string Remark { get; set; }
        }
    }
}
