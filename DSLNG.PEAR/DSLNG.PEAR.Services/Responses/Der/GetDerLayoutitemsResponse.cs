using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetDerLayoutitemsResponse : BaseResponse
    {
        public GetDerLayoutitemsResponse()
        {
            Items = new List<LayoutItem>();
        }

        public IList<LayoutItem> Items { get; set; }

        public class LayoutItem
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
            public int DerLayoutId { get; set; }
        }

    }

}
