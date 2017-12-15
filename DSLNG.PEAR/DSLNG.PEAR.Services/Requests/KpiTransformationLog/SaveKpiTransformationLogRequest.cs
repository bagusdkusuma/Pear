using DSLNG.PEAR.Data.Enums;
using System;

namespace DSLNG.PEAR.Services.Requests.KpiTransformationLog
{
    public class SaveKpiTransformationLogRequest
    {
        public int Id { get; set; }
        public int KpiTransformationScheduleId { get; set; }
        public int KpiId { get; set; }
        public DateTime Periode { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public KpiTransformationStatus Status { get; set; }
        public int MethodId { get; set; }
        public string Notes { get; set; }
        public bool NeedCleanRowWhenError { get; set; }
    }
}
