using System.Collections.Generic;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Common.PmsSummary;
using DSLNG.PEAR.Services.Responses.Pillar;

namespace DSLNG.PEAR.Services.Responses.PmsSummary
{
    public class GetPmsSummaryConfigurationResponse : BaseResponse
    {
        public IList<Pillar> Pillars { get; set; }
        public IList<Kpi> Kpis { get; set; }
        public IList<PmsConfig> PmsConfigs { get; set; }
        public int Year { get; set; }
        public class Pillar
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Kpi
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Measurement { get; set; }
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
            public Kpi Kpi { get; set; }
            public double Weight { get; set; }
            //public bool KpiAsGraphic { get; set; }
            public IList<ScoreIndicator> ScoreIndicators { get; set; }
            public ScoringType ScoringType { get; set; }
        }
     
    }
}
