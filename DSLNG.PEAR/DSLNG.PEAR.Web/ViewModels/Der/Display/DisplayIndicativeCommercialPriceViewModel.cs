using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayIndicativeCommercialPriceViewModel
    {
        public DisplayIndicativeCommercialPriceViewModel()
        {
            IndicativeCommercialPriceViewModels = new List<IndicativeCommercialPriceViewModel>();    
        }

        public IList<IndicativeCommercialPriceViewModel> IndicativeCommercialPriceViewModels { get; set; } 

        public class IndicativeCommercialPriceViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string Daily { get; set; }
            public string Measurement { get; set; }
        }
    }
}