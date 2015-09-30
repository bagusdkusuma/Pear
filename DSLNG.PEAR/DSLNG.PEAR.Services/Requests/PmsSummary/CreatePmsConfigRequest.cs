using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Common.PmsSummary;

namespace DSLNG.PEAR.Services.Requests.PmsSummary
{
    public class CreatePmsConfigRequest
    {
        public string ScoringType { get; set; }
        public int PillarId { get; set; }
        public int PmsSummaryId { get; set; }
        public double Weight { get; set; }
        public IEnumerable<ScoreIndicator> ScoreIndicators { get; set; }
    }
}
