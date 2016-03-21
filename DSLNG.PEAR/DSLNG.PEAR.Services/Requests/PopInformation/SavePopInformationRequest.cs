using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.PopInformation
{
    public class SavePopInformationRequest
    {
        public int Id { get; set; }
        public int DashboardId { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string Leader { get; set; }
        public string Owner { get; set; }
        public string Financial { get; set; }
        public string Remarks { get; set; }
    }
}
