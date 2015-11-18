using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Buyer
{
    public class GetBuyerForGridResponse
    {
        public IList<BuyerForGrid> BuyerForGrids { get; set; }
        public int TotalRecords;
        public class BuyerForGrid
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
