using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayTableTankViewModel
    {
        public DisplayTableTankViewModel()
        {
            TableTankViewModels = new List<TableTankViewModel>();    
        }

        public IList<TableTankViewModel> TableTankViewModels { get; set; } 

        public class TableTankViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string Daily { get; set; }
            public string Measurement { get; set; }
        }
    }
}