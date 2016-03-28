using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Requests.Der
{
    public class GetKpiValueRequest
    {
        public ConfigType ConfigType { get; set; }
        public int KpiId { get; set; }
        public DateTime Periode { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public YtdFormula YtdFormula { get; set; }
        public RangeFilter RangeFilter { get; set; }
    }
}
