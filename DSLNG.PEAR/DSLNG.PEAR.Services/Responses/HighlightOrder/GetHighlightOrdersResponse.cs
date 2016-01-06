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
        public int Count { get; set; }
        public int TotalRecords { get; set; }
        public class HighlightOrderResponse {
            public int Id { get; set; }
            public string Text { get; set; }
            public string Value { get; set; }
            public int Order { get; set; }
            public int GroupId { get; set; }
            public int[] RoleGroupIds { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
