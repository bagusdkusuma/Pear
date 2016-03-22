using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.Pop
{
    public class Signature
    {
        public int Id { get; set; }
        public PopDashboard PopDashboard { get; set; }
        public User User { get; set; }
        public SignatureType Type { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
        public string Note { get; set; }
    }
}
