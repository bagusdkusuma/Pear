using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayCriticalPmViewModel
    {
        public DisplayCriticalPmViewModel()
        {
            CriticalPmViewModels = new List<CriticalPmViewModel>();    
        }

        public IList<CriticalPmViewModel> CriticalPmViewModels { get; set; }
        public string Date { get; set; }
        public class CriticalPmViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string Weekly { get; set; }
            public string Measurement { get; set; }
            public string Remarks { get; set; }
        }
    }
}