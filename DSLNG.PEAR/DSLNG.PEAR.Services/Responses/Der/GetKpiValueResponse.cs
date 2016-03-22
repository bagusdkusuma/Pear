using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetKpiValueResponse : BaseResponse
    {
        public int Id { get; set; }
        public KpiResponse Kpi { get; set; }
        public double? Value { get; set; }
        public DateTime Periode { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public string Remark { get; set; }

        public bool IsActive { get; set; }

        public class KpiResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Measurement { get; set; }
            public string Remark { get; set; }
        }
    }
}
