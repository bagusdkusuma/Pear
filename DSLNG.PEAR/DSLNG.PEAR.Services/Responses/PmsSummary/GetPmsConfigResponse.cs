using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Common.PmsSummary;

namespace DSLNG.PEAR.Services.Responses.PmsSummary
{
    public class GetPmsConfigResponse : BaseResponse
    {
        public int Id { get; set; }
        public int PillarId { get; set; }
        public string PillarName { get; set; }
        public double Weight { get; set; }
        public string ScoringType { get; set; }
        public bool IsActive { get; set; }
        public int PmsSummaryId { get; set; }
        public IEnumerable<ScoreIndicator> ScoreIndicators { get; set; }
        public string Color { get; set; }
    }
}
