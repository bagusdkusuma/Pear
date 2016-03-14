using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Signature
{
    public class ApproveSignatureResponse : BaseResponse
    {
        public int Id { get; set; }
        public int DashboardId { get; set; }
        public int UserId { get; set; }
        public SignatureType Type { get; set; }
        public bool Approve { get; set; }
        public bool Reject { get; set; }
        public string SignatureImage { get; set; }
        public string Note { get; set; }
    }
}
