using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayProcurementViewModel
    {
        public DisplayProcurementViewModel()
        {
            ProcurementViewModels = new List<ProcurementViewModel>();    
        }

        public IList<ProcurementViewModel> ProcurementViewModels { get; set; }

        public class ProcurementViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string Daily { get; set; }
            public string Measurement { get; set; }
            public string Remarks { get; set; }
        }
    }
}