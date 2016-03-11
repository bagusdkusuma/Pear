using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.MidtermStrategyPlanning
{
    public class AddPlanningKpiViewModel
    {
        public int MidtermPlanningId { get; set; }
        public int KpiId { get; set; }
        public int OldKpiId { get; set; }
    }
}