using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.OperationGroup
{
    public class OperationGroupViewModel
    {
        public int Id { get; set; }
        [Display(Name="Key Operation Group")]
        [Required]
        public string Name { get; set; }
         [Required]
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}