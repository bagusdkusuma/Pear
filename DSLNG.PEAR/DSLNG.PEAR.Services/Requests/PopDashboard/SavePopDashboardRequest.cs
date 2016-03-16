using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.PopDashboard
{
    public class SavePopDashboardRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string Subtitle { get; set; }
        public bool IsActive { get; set; }
        public string Attachment { get; set; }
        public string Status { get; set; }
    }
}
