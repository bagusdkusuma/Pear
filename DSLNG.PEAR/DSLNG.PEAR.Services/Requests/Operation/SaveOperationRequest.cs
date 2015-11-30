using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Operation
{
    public class SaveOperationRequest
    {
        public int Id { get; set; }
        public int KeyOperationGroupId { get; set; }
        public int KpiId { get; set; }
        public int Order { get; set; }
        public string Desc { get; set; }
        public bool IsActive { get; set; }
    }
}
