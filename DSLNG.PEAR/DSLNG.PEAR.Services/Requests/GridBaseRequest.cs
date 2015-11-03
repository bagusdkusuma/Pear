using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests
{
    public class GridBaseRequest
    {
        public string Search { get; set; }
        public IDictionary<string, SortOrder> SortingDictionary { get; set; }
    }
}
