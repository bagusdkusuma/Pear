using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Signature
{
    public class ApproveSignatureRequest
    {
        public int Id { get; set; }
        public int DashboardId { get; set; }
        public int UserId { get; set; }
        public int Type { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
        public string Note { get; set; }
    }
}
