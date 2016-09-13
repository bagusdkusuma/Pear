using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayKpiInformationViewModel
    {
        public DisplayKpiInformationViewModel()
        {
            KpiInformationViewModels = new List<KpiInformationViewModel>();
        }

        public DateTime DateInfo { get; set; }

        public IList<KpiInformationViewModel> KpiInformationViewModels { get; set; }

        public class KpiInformationViewModel
        {
            public KpiInformationViewModel()
            {
                DerItemValue = new DerItemValueViewModel();
            }
            public int Position { get; set; }
            public string KpiLabel { get; set; }
            public string KpiMeasurement { get; set; }
            public string KpiName { get; set; }

            public DerItemValueViewModel DerItemValue { get; set; }
        }
        
    }
}