using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.OutputCategory
{
    public class OutputCategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name="Key Output Category")]
        public string Name { get; set; }
        [Required]
        public int Order { get; set; }
        [Required]
        public string Remark { get; set; }
        public bool IsActive { get; set; }       

    }
}