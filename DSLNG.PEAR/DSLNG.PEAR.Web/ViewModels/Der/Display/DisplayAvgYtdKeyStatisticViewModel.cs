using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayAvgYtdKeyStatisticViewModel
    {
        public DisplayAvgYtdKeyStatisticViewModel()
        {
            AvgYtdKeyStatistics = new List<AvgYtdKeyStatisticViewModel>();    
        }

        public IList<AvgYtdKeyStatisticViewModel> AvgYtdKeyStatistics { get; set; } 

        public class AvgYtdKeyStatisticViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string Value { get; set; }     
        }
        
    }
}