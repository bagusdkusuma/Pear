using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Operation
{
    public class UpdateOperationRequest
    {
        public int Id { get; set; }
        public int? Order { get; set; }
        public bool? IsActive { get; set; }
        public int KeyOperationGroupId { get; set; }
        public int KpiId { get; set; }
    }
}
