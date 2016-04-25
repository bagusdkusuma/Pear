using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Menu
{
    public class UpdateMenuViewModel
    {
        public UpdateMenuViewModel()
        {
            RoleGroupOptions = new List<SelectListItem>();
            MenuOptions = new List<SelectListItem>();
        }

        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
        public string Module { get; set; }
        [Required]
        [Display(Name="Activate?")]
        public bool IsActive { get; set; }
        [Display(Name="Set As Root Menu")]
        public bool IsRoot { get; set; }
        [Display(Name = "Select Role Groups")]
        public List<int> RoleGroupIds { get; set; }
        [Display(Name = "Parent Menu")]
        public int? ParentId { get; set; }
        [Display(Name = "Select Role Groups")]
        public List<SelectListItem> RoleGroupOptions { get; set; }
        [Display(Name = "Parent Menu")]
        public List<SelectListItem> MenuOptions { get; set; }
        [Display(Name = "Select Icon")]
        public string Icon { get; set; }
        [Display(Name = "Url Action")]
        public string Url { get; set; }
        public String[] IconList
        {
            get
            {
                return new String[6] { "fa-dashboard", "fa-calendar", "fa-gavel", "fa-edit", "fa-bar-chart-o", "fa-cog" };
            }
        }
    }
}