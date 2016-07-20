using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Der
{
    public class GenerateViewModel
    {
        public string Date { get; set; }
        [AllowHtml]
        public string Content { get; set; }
    }
}