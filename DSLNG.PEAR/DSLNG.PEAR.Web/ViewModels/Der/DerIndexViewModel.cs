using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Der
{
    public class DerIndexViewModel
    {
        public DerIndexViewModel()
        {
            Items = new List<DerItem>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<DerItem> Items { get; set; }

        public class DerItem
        {
            public string Type { get; set; } //text, highlight, artifact, image
            public int? ComponentId { get; set; }
            public string Text { get; set; }
            public string FileLocation { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
        }
    }
}