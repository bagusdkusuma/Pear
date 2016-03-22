using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayJobPmtsViewModel
    {
        public DisplayJobPmtsViewModel()
        {
            JobPmtsViewModels = new List<JobPmtsViewModel>();    
        }

        public IList<JobPmtsViewModel> JobPmtsViewModels { get; set; } 

        public class JobPmtsViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string Daily { get; set; }
            public string Mtd { get; set; }
            public string Ytd { get; set; }
            public string Measurement { get; set; }
        }
    }
}