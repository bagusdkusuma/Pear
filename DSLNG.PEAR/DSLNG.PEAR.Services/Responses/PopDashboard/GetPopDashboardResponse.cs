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
        public string DashboardObjective { get; set; }
        public string StructureOwner { get; set; }
        public string Team { get; set; }
        public double BudgetOpex { get; set; }
        public double BudgetCapex { get; set; }
        public string AffectedKPI { get; set; }
        public DateTime ProjectStart { get; set; }
        public DateTime ProjectEnd { get; set; }
        public string Status { get; set; }

        public IList<Attachment> Attachments { get; set; }

        //old
        public IList<PopInformation> PopInformations { get; set; }
        public IList<Signature> Signatures { get; set; }

        public class Attachment {
            public int Id { get; set; }
            public string Filename { get; set; }
            public string Alias { get; set; }
            public string Type { get; set; }
        }

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
            public string Note { get; set; }
        }
    }
}
