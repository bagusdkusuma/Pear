

using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.DerTransaction
{
    public class GetHighlightValuesResponse
    {
        public GetHighlightValuesResponse() {
            Highlights = new List<DerHighlight>();
        }
        public IList<DerHighlight> Highlights { get; set; }
        public class DerHighlight {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string HighlightMessage { get; set; }
            public string HighlightTitle { get; set; }
            public int HighlightTypeId { get; set; }
            public string HighlightTypeValue { get; set; }
            public string Type { get; set; }
        }
    }
}
