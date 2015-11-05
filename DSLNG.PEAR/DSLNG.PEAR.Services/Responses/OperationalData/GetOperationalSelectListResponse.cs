using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.OperationalData
{
    public class GetOperationalSelectListResponse
    {

        public IList<Operation> Operations { get; set; }
        public IList<KPI> KPIS { get; set; }
        public class Operation
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class KPI
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
