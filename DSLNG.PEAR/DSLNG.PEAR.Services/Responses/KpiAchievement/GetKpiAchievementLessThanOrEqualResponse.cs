using DSLNG.PEAR.Data.Enums;
using System;

namespace DSLNG.PEAR.Services.Responses.KpiAchievement
{
    public class GetKpiAchievementLessThanOrEqualResponse : BaseResponse
    {
        public int Id { get; set; }
        public KpiResponse Kpi { get; set; }
        public string Value { get; set; }
        public string Mtd { get; set; }
        public string Ytd { get; set; }
        public string Itd { get; set; }
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
