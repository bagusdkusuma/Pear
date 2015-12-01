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
            Kpis = new List<Kpi>();
        }

        public int Id { get; set; }
        public IList<SelectListItem> KeyOperationGroups { get; set; }
        [Required]
        [Display(Name= "Key Operation Group")]
        public int KeyOperationGroupId { get; set; }
        [Required]
        [Display(Name="Key Operation")]
        public int KpiId { get; set; }
        public IList<Kpi> Kpis { get; set; }
        [Required]
        public int Order { get; set; }
        public string Desc { get; set; }
        public bool IsActive { get; set; }

        public class Kpi
        {
            public int Id { get; set; }
            public string Name { get; set; } 
        }
    }
}