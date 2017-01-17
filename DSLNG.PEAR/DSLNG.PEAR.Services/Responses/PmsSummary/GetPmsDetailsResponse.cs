

using System.Collections.Generic;
using DSLNG.PEAR.Services.Common.PmsSummary;

namespace DSLNG.PEAR.Services.Responses.PmsSummary
{
    public class GetPmsDetailsResponse : BaseResponse
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public string KpiTypeYearly { get; set; }
        public string KpiPeriodYearly { get; set; }
        public string KpiRemarkYearly { get; set; }
        public string KpiGroup { get; set; }
        public int KpiId { get; set; }
        public int MeasurementId { get; set; }
        public string MeasurementName { get; set; }
        public string KpiName { get; set; }
        public string KpiUnit { get; set; }
        public string KpiPeriod { get; set; }
        public double? KpiActualYearly { get; set; }
        public double? KpiActualMonthly { get; set; }
        public List<KpiAchievment> KpiAchievmentMonthly { get; set; }
        public List<KpiRelation> KpiRelations { get; set; }
        public IList<ScoreIndicator> ScoreIndicators { get; set; }
        public double Weight { get; set; }
        public string ScoringType { get; set; }
        public IList<Group> Groups { get; set; }

        public class KpiAchievment
        {
            public string Type { get; set; }
            public string Period { get; set; }
            public string Remark { get; set; }
        }

        public class KpiRelation
        {
            public string Name { get; set; }
            public string Unit { get; set; }
            public string Method { get; set; }
            public double? ActualYearly { get; set; }
            public double? ActualMonthly { get; set; }
            public double? ActualYtd { get; set; }
            public double? ActualMtd { get; set; }
            public double? ActualItd { get; set; }
        }

        public class Group
        {
            public string Name { get; set; }
            public string PerformanceIndicator { get; set; }
            public string Unit { get; set; }
            public string Periode { get; set; }
            public double? ActualYearly { get; set; }
            public double? ActualMonthly { get; set; }
            public double? ActualYtd { get; set; }
            public double? ActualMtd { get; set; }
            public double? ActualItd { get; set; }
        }
    }

    
}
