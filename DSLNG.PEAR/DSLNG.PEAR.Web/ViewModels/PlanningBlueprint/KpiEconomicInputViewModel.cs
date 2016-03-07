using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class KpiEconomicInputViewModel
    {
        public double Value { get; set; }
        public int KpiId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}