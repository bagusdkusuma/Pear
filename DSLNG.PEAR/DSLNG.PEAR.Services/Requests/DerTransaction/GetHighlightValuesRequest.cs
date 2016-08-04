
using DSLNG.PEAR.Services.Responses.DerTransaction;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Requests.DerTransaction
{
    public class GetHighlightValuesRequest
    {
        public DateTime Date { get; set; }
        public IList<GetDerLayoutItemsResponse.DerHighlight> DerHighlights { get; set; }
    }
}
