using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.PlanningBlueprint
{
    public class RejectVoyagePlanRequest
    {
        public int PlanningBlueprintId { get; set; }
        public string Notes { get; set; }
    }
}
