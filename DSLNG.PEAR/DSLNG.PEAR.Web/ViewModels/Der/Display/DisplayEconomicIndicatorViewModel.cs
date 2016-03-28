using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayEconomicIndicatorViewModel
    {
        public DisplayEconomicIndicatorViewModel()
        {
            EconomicIndicatorViewModels = new List<EconomicIndicatorViewModel>();    
        }

        public IList<EconomicIndicatorViewModel> EconomicIndicatorViewModels { get; set; } 

        public class EconomicIndicatorViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string Daily { get; set; }
            public string Measurement { get; set; }
            public string Progress { get; set; }
        }
    }
}