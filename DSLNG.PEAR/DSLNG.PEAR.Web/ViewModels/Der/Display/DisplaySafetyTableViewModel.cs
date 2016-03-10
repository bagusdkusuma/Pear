using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplaySafetyTableViewModel
    {
        public DisplaySafetyTableViewModel()
        {
            SafetyTableViewModels = new List<SafetyTableViewModel>();    
        }

        public IList<SafetyTableViewModel> SafetyTableViewModels { get; set; }

        public class SafetyTableViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string CurrentDay { get; set; }
            public string Mtd { get; set; }
            public string Ytd { get; set; }
            public string AnnualTarget { get; set; }
            public string Itd { get; set; }  
        }
        
    }
}