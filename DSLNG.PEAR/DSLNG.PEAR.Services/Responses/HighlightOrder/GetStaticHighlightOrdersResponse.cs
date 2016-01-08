using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.HighlightOrder
{
    public class GetStaticHighlightOrdersResponse
    {
        public IList<HighlightOrderResponse> HighlightOrders { get; set; }
        public int TotalRecords { get; set; }
        public class HighlightOrderResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int[] RoleGroupIds { get; set; }
        }
    }
}
