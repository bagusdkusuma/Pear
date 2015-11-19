

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Highlight
{
    public class GetHighlightsResponse
    {
        public IList<HighlightResponse> Highlights { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class HighlightResponse {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public string Type { get; set; }
            public DateTime Date { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
