

using DSLNG.PEAR.Data.Enums;
using System;
namespace DSLNG.PEAR.Services.Requests.Highlight
{
    public class GetHighlightsRequest : GridBaseRequest
    {
        public GetHighlightsRequest()
        {
            Except = new string[0];
            Include = new string[0];
        }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
        public string[] Except { get; set; }
        public string[] Include { get; set; }
        public DateTime? Date { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public bool IsActive { get; set; }
    }
}
