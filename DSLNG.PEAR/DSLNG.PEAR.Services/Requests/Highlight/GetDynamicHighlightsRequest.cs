using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Highlight
{
    public class GetDynamicHighlightsRequest
    {
        public PeriodeType PeriodeType { get; set; }
        public DateTime? Periode { get; set; }
    }
}
