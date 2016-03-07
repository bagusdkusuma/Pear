using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.PlanningBlueprint
{
    public class KpiTargetInputRequest
    {
        public double Value { get; set; }
        public int KpiId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
