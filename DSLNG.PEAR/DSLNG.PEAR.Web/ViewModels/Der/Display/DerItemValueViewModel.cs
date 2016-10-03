using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der.Display
{
    public class DerItemValueViewModel
    {
        public string Value { get; set; }
        public string Mtd { get; set; }
        public string Ytd { get; set; }
        public string Itd { get; set; }
        public string Remark { get; set; }
        public string Deviation { get; set; }
        public string MtdDeviation { get; set; }
        public string YtdDeviation { get; set; }
        public string ItdDeviation { get; set; }
    }
}