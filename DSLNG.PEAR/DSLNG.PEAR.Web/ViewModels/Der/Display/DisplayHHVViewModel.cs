using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayHHVViewModel
    {
        public DisplayHHVViewModel()
        {
            HHVViewModels = new List<HHVViewModel>();    
        }

        public IList<HHVViewModel> HHVViewModels { get; set; } 

        public class HHVViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string Daily { get; set; }
            public string Measurement { get; set; }
        }
    }
}