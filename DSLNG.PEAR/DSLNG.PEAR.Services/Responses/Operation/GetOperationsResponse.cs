using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Operation
{
    public class GetOperationsResponse
    {
        public IList<Operation> Operations { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class Operation
        {
            public int Id { get; set; }
            //public string KeyOperationGroup { get; set; }
            public int KeyOperationGroupId { get; set; }
            public string Kpi { get; set; }
            public string KpiId { get; set; }
            public string Desc { get; set; }
            public bool IsActive { get; set; }
            public int Order { get; set; }
        }
    }
}
