using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Select
{
    public class SelectOptionViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public HttpPostedFileBase ValueFile { get; set; }
        public bool IsEdited { get; set; }
        public bool IsActive { get; set; }
    }
}