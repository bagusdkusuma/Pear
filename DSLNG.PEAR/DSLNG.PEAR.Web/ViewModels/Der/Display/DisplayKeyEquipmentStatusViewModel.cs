using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DisplayKeyEquipmentStatusViewModel
    {
        public DisplayKeyEquipmentStatusViewModel()
        {
            KeyEquipmentStatusViewModels = new List<KeyEquipmentStatusViewModel>();    
        }

        public IList<KeyEquipmentStatusViewModel> KeyEquipmentStatusViewModels { get; set; }

        public class KeyEquipmentStatusViewModel
        {
            public int Position { get; set; }
            public string KpiName { get; set; }
            public string highlight { get; set; }
        }
    }
}