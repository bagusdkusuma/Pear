﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Common.PmsSummary;

namespace DSLNG.PEAR.Services.Responses.PmsSummary
{
    public class GetPmsConfigDetailsResponse : BaseResponse
    {
        public int Id { get; set; }
        public double Weight { get; set; }
        public string ScoringType { get; set; }
        public int PillarId { get; set; }
        public string PillarName { get; set; }
        public int KpiId { get; set; }
        public string KpiName { get; set; }
        public IList<ScoreIndicator> ScoreIndicators { get; set; }

        /*public class Pillar
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public class Kpi 
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public int PillarId { get; set; }
        }*/
        
    }
}