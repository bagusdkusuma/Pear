using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Common.PmsSummary;

namespace DSLNG.PEAR.Services.Requests.PmsSummary
{
    public class UpdatePmsConfigDetailsRequest
    {
        public int Id { get; set; }

        public int KpiId { get; set; }
        public string ScoringType { get; set; }
        public string Target { get; set; }
        public double Weight { get; set; }
        public IEnumerable<ScoreIndicator> ScoreIndicators { get; set; }
    }
}
