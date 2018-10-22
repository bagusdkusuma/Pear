using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.KpiTarget
{
    public class GetKpiTargetsResponse : BaseResponse
    {
        public GetKpiTargetsResponse()
        {
            KpiTargets = new List<KpiTarget>();
        }
        public IList<KpiTarget> KpiTargets { get; set; }

        public class KpiTarget
        {
            public int Id { get; set; }
            public string KpiName { get; set; }
            public int KpiId { get; set; }
            public string MeasurementName { get; set; }
            public string PeriodeType { get; set; }
            public double? Value { get; set; }
            public bool IsActive { get; set; }
            public DateTime Periode { get; set; }
        }
    }
}
