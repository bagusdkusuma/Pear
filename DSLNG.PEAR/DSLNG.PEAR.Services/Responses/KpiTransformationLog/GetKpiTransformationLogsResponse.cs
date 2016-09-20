using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.KpiTransformationLog
{
    public class GetKpiTransformationLogsResponse
    {
        public IList<KpiTransformationLogResponse> Logs { get; set; }
        public int TotalRecords { get; set; }
        public class KpiTransformationLogResponse
        {
            public int Id { get; set; }
            public string KpiName { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public string KpiMeasurement { get; set; }
            public DateTime Periode { get; set; }
            public KpiTransformationStatus Status { get; set; }
            public string StatusName { get {
                    return Enum.GetName(typeof(KpiTransformationStatus), this.Status);
                } }
            public string Notes { get; set; }
        }
    }
}
