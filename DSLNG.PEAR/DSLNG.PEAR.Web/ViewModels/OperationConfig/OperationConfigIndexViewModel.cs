using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.OperationConfig
{
    public class OperationConfigIndexViewModel
    {
        public OperationConfigIndexViewModel()
        {
            OperationGroups = new List<SelectListItem>();
        }
        public IList<SelectListItem> OperationGroups { get; set; }
        public int KeyOperationGroupId { get; set; }
    }
}