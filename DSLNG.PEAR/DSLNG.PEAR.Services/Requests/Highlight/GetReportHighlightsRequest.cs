

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Requests.Highlight
{
    public class GetReportHighlightsRequest
    {
        public IList<DateTime> TimePeriodes { get; set; }
        public HighlightType Type { get; set; }
        public PeriodeType PeriodeType { get; set; }
    }
}
