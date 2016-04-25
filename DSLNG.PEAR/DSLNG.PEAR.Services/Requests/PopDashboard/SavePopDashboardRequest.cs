using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.PopDashboard
{
    public class SavePopDashboardRequest
    {
        public SavePopDashboardRequest() {
            AttachmentFiles = new List<Attachment>();
        }
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

        public IList<Attachment> AttachmentFiles { get; set; }

        public class Attachment {
            public int Id { get; set; }
            public string Alias { get; set; }
            public string FileName { get; set; }
            public string Type { get; set; }
        }
    }
}
