using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Operation
{
    public class OperationViewModel
    {
        public OperationViewModel()
        {
            KeyOperationGroups = new List<SelectListItem>();
            KPIS = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public IList<SelectListItem> KeyOperationGroups { get; set; }
        [Required]
        [Display(Name= "Operation Group")]
        public int IdKeyOperationGroup { get; set; }
        [Required]
        [Display(Name="KPI")]
        public int IdKPI { get; set; }
        public IList<SelectListItem> KPIS { get; set; }
        public int Order { get; set; }
        public string Desc { get; set; }
        public bool IsActive { get; set; }
    }
}