using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.HighlightOrder
{
    public class GetHighlightOrdersResponse
    {
        public IList<HighlightOrderResponse> HighlightOrders { get; set; }
        public class HighlightOrderResponse {
            public int Id { get; set; }
            public string Text { get; set; }
            public string Value { get; set; }
            public int Order { get; set; }
        }
    }
}
