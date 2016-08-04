using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Requests.DerTransaction
{
    public class GetDerLayoutItemsRequest
    {
        public GetDerLayoutItemsRequest() {
            DerLayoutItemTypes = new List<DerLayoutItemType>();
            Positions = new List<Position>();
        }
        public IList<DerLayoutItemType> DerLayoutItemTypes { get; set; }
        public IList<Position> Positions { get; set; }
        public int LayoutId { get; set; }

        public class Position {
            public int Row { get; set; }
            public int Column { get; set; }
        }
    }
}
