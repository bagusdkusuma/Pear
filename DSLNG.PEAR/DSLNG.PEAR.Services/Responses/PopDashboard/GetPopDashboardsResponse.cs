using System;
using System.Collections.Generic;
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
            public int Id { get; set; }
            public string Title { get; set; }
            public string Number { get; set; }
            public string Subtitle { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
