using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        public bool IsActive { get; set; }
    }
}