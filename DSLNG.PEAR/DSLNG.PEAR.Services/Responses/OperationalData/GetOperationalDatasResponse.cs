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
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class OperationalData
        {
            public int Id { get; set; }
            public string Scenario { get; set; }
            public string KeyOperation { get; set; }
            public string Kpi { get; set; }
            public double? Value { get; set; }
            public string Remark { get; set; }
            public DateTime Periode { get; set; }
            public string PeriodeType { get; set; }
        }
    }
}
