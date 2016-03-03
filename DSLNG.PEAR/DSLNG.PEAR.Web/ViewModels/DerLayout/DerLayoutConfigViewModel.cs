using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.DerLayout
{
    public class DerLayoutConfigViewModel
    {
        public DerLayoutConfigViewModel()
        {
            Items = new List<LayoutItem>();
        }

        public int DerLayoutId { get; set; }
        public IList<LayoutItem> Items { get; set; }

        public class LayoutItem
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
            public int DerLayoutId { get; set; }
        }
    }
}