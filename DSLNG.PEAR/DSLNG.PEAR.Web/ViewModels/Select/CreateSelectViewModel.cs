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
        }

        [Required]
        public string Name { get; set; }
        public IList<SelectOptionViewModel> Options { get; set; }
        public IList<SelectListItem> Types { get; set; }
        public int ParentId { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}