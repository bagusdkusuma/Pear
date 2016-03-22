using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Signature
{
    public class SaveSignatureResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string SignatureImage { get; set; }
        public SignatureType TypeSignature { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
    }
}
