using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.PopDashboard
{
    public class GetPopDashboardResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string Subtitle { get; set; }
        public bool IsActive { get; set; }
        public IList<PopInformation> PopInformations { get; set; }
        public IList<Signature> Signatures { get; set; }

        public class PopInformation
        {
            public int Id { get; set; }
            public PopInformationType Type { get; set; }
            public string Title { get; set; }
            public string Value { get; set; }
        }
        

        public class Signature
        {
            public int Id {get; set;}
            public int UserId { get; set; }
            public string User { get; set; }
            public SignatureType Type {get; set;}
            public string SignatureImage { get; set; }
            public bool IsApprove { get; set; }
            public bool IsReject { get; set; }
        }
    }
}
