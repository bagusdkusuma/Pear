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
            public string ActualDaily { get; set; }
            public string ActualMtd { get; set; }
            public string ActualYtd { get; set; }     
        }
    }
}