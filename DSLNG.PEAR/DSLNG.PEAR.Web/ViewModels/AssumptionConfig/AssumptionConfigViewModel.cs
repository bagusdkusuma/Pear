using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.AssumptionConfig
{
    public class AssumptionConfigViewModel
    {

        public AssumptionConfigViewModel()
        {
            Measurements = new List<SelectListItem>();
            Categories = new List<SelectListItem>();
        }


        public int Id { get; set; }
        [Display(Name="Key Assumption")]
        public string Name { get; set; }
        [Required]
        [Display(Name="Category")]
        public int IdCategory { get; set; }
        public IList<SelectListItem> Categories { get; set; }
        [Required]
        [Display(Name = "Measurement")]
        public int? IdMeasurement { get; set; }
        public IList<SelectListItem> Measurements { get; set; }
        [Required]
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}