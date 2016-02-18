using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Der
{
    public class ManageDerItemViewModel
    {
        public ManageDerItemViewModel()
        {
            Types = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Type { get; set; } //text, highlight, artifact, image
        public int? ComponentId { get; set; }
        public string Text { get; set; }
        public string FileLocation { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
    }
}