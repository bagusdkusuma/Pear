using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.OperationalData
{
    public class GetOperationIdResponse : BaseResponse
    {
        public GetOperationIdResponse()
        {
            OperationDatas = new List<OperationData>();
        }

        public IList<OperationData> OperationDatas { get; set; }
        public class OperationData
        {
            public int Id { get; set; }
            public int Scenario { get; set; }
            public int KeyOperationConfig { get; set; }
            public int Kpi { get; set; }
            public double? Value { get; set; }
            public string Remark { get; set; }
            public DateTime Periode { get; set; }
            public PeriodeType PeriodeType { get; set; }
        }

    }
}
