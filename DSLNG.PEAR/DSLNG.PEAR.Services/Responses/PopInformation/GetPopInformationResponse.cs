using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.PopInformation
{
    public class GetPopInformationResponse
    {
        public int Id { get; set; }
        public int DashboardId { get; set; }
        public PopInformationType Type { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
    }
}
