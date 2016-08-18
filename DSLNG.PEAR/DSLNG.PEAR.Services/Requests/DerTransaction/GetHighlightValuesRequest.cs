
using DSLNG.PEAR.Services.Responses.DerTransaction;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Requests.DerTransaction
{
    public class GetHighlightValuesRequest
    {
        public GetHighlightValuesRequest() {
            HighlightTypeIds = new int[] { };
        }
        public DateTime Date { get; set; }
        public IEnumerable<int> HighlightTypeIds { get; set; }
    }
}
