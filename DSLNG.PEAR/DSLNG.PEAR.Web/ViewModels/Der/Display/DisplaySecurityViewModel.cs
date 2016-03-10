using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplaySecurityViewModel
    {
        public DisplaySecurityViewModel()
        {
            SecurityViewModels = new List<SecurityViewModel>();    
        }

        public IList<SecurityViewModel> SecurityViewModels { get; set; } 

        public class SecurityViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string Value { get; set; }     
        }
        
    }
}