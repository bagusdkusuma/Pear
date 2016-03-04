using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.DerLayout
{
    public class DerCreateLayoutItemViewModel
    {
        public int DerLayoutId { get; set; }
        public IList<SelectListItem> Types { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public string Type { get; set; }
    }
}