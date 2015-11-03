using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Kpi
{
    public class GetKpisRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int PillarId { get; set; }
    }
}