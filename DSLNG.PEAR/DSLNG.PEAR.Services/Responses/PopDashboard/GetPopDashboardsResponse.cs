using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.PopDashboard
{
    public class GetPopDashboardsResponse
    {
        public int Count { get; set; }
        public int TotalRecords { get; set; }
        public IList<PopDashboard> PopDashboards { get; set; }

        public class PopDashboard
        {
            public PopDashboard() {
                Attachments = new List<Attachment>();
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
            public IList<Attachment> Attachments { get; set; }
        }

        public class Attachment
        {
            public int Id { get; set; }
            public string Filename { get; set; }
            public string Alias { get; set; }
            public string Type { get; set; }
        }

    }
}
