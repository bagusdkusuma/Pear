using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Select
{
    public class CreateSelectViewModel
    {
        public CreateSelectViewModel()
        {
            Options = new List<SelectOptionViewModel>
                {
                    new SelectOptionViewModel()
                };
            Types = new List<SelectListItem>();
            Parents = new List<SelectListItem>();
            ParentOptions = new List<SelectListItem>();
            IsDashBoard = true;
        }

        [Required]
        public string Name { get; set; }
        public IList<SelectOptionViewModel> Options { get; set; }
        public IList<SelectListItem> Types { get; set; }
        public IList<SelectListItem> Parents { get; set; }
        public IList<SelectListItem> ParentOptions { get; set; }
        [Display(Name="Parent")]
        public int ParentId { get; set; }
        [Display(Name = "Parent Option")]
        public int ParentOptionId { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsDashBoard { get; set; }
        public bool IsDer { get; set; }
    }
}