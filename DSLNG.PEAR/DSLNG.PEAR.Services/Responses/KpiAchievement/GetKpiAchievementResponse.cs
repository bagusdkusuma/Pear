using DSLNG.PEAR.Data.Enums;
using System;

namespace DSLNG.PEAR.Services.Responses.KpiAchievement
{
    public class GetKpiAchievementResponse : BaseResponse
    {
        public int Id { get; set; }
        public KpiResponse Kpi { get; set; }
        public double? Value { get; set; }
        public double? Mtd { get; set; }
        public double? Ytd { get; set; }
        public double? Itd { get; set; }
        public DateTime Periode { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public string Remark { get; set; }
        public string Deviation { get; set; }
        public string MtdDeviation { get; set; }
        public string YtdDeviation { get; set; }
        public string ItdDeviation { get; set; }
        public bool IsActive { get; set; }

        public class KpiResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Measurement { get; set; }
            public string Remark { get; set; }
            public string KpiLabel { get; set; }
            public string KpiMeasurement { get; set; }
        }
    }
}
