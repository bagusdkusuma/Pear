using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Web.ViewModels.Common.PmsSummary;

namespace DSLNG.PEAR.Web.ViewModels.PmsSummary
{
    public class PmsSummaryDetailsViewModel
    {
        public IList<SelectListItem> Pillars { get; set; }
        public IList<SelectListItem> Kpis { get; set; }
        public IList<PmsConfig> PmsConfigs { get; set; }
        public int[] PillarIds { get; set; }
        public int PmsSummaryId { get; set; }
        public int Year { get; set; }
        public double TotalScoreAllPillar
        {
            get
            {
                return PmsConfigs.Sum(pmsConfig => pmsConfig.Weight);
            }
        }

        public class PmsConfig
        {
            public PmsConfig()
            {
                PmsConfigDetailsList = new List<PmsConfigDetails>();
            }

            public int Id { get; set; }
            public int PillarId { get; set; }
            public string PillarName { get; set; }
            public double Weight { get; set; }
            public ScoringType ScoringType { get; set; }
            public IList<PmsConfigDetails> PmsConfigDetailsList { get; set; }
        }

        public class PmsConfigDetails
        {
            public int Id { get; set; }
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public double Weight { get; set; }
            public string KpiMeasurement { get; set; }
            public string ScoringType { get; set; }
            public IList<ScoreIndicatorViewModel> ScoreIndicators { get; set; }
        }


    }
}