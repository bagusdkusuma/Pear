using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Signature
{
    public class SaveSignatureRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DashboardId { get; set; }
        public string Username { get; set; }
        public int TypeSignature { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
    }
}
