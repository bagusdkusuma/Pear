using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayMGDPViewModel
    {
        public DisplayMGDPViewModel()
        {
            MGDPViewModels = new List<MGDPViewModel>();
        }

        public IList<MGDPViewModel> MGDPViewModels { get; set; }

        public class MGDPViewModel
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