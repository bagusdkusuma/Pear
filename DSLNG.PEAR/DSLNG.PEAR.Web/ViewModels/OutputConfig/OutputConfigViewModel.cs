﻿

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace DSLNG.PEAR.Web.ViewModels.OutputConfig
{
    public class OutputConfigViewModel
    {
        public OutputConfigViewModel() {
            Formulas = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name="Category")]
        [Required]
        public int CategoryId { get; set; }
        [Display(Name="Measurement")]
        [Required]
        public int? MeasurementId { get; set; }
        public IList<SelectListItem> Measurements { get; set; }
        public IList<SelectListItem> OutputCategories { get; set; }
         [Required]
        public string Formula { get; set; }
        public IList<SelectListItem> Formulas { get; set; }
        public IList<int> KpiIds { get; set; }
        public IList<int> KeyAssumptionIds { get; set; }
         [Required]
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public IList<SelectListItem> Kpis { get; set; }
        public IList<SelectListItem> KeyAssumptions { get; set; }
    }
}