using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.AssumptionCategory
{
    public class AssumptionCategoryViewModel
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "Key Assumption Category")]
        public string Name { get; set; }
        public string Desc { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public int Order { get; set; }

    }
}