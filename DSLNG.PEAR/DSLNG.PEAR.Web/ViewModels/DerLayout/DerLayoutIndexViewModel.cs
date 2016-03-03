using System.Collections.Generic;

namespace DSLNG.PEAR.Web.ViewModels.DerLayout
{
    public class DerLayoutIndexViewModel
    {
        public DerLayoutIndexViewModel()
        {
            DerLayouts = new List<DerLayoutViewModel>();
        }

        public IList<DerLayoutViewModel> DerLayouts { get; set; }
    }
}