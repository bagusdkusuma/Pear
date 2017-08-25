using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.AuditTrail
{
    public class CreateAuditUserRequest : BaseRequest
    {
        public int Login_Id { get; set; }
        public string Url { get; set; }
        public string Activity { get; set; }
        public string Remarks { get; set; }
    }
}
