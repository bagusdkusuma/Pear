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
    }
}
