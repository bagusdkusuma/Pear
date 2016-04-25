using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Menu
{
    public class CreateMenuViewModel
    {
        public CreateMenuViewModel()
        {
            RoleGroupOptions = new List<SelectListItem>();
            MenuOptions = new List<SelectListItem>();
        }
        [Required]
        public string Name { get; set; }
        public int Order { get; set; }
        //public ICollection<RoleGroup> RoleGroups { get; set; }
        public string Remark { get; set; }
        public string Module { get; set; }
        [Display(Name="Select Icon")]
        public string Icon { get; set; }
        [Display(Name="Url Action")]
        public string Url { get; set; }
        [Required]
        [Display(Name="Active?")]
        public bool IsActive { get; set; }
        [Display(Name = "Select Role Groups")]
        public List<int> RoleGroupIds { get; set; }
        [Display(Name = "Parent Menu")]
        public int? ParentId { get; set; }
        [Display(Name="Auto Add Parent Menu Privilege?")]
        public bool AddParent { get; set; }
        public List<SelectListItem> RoleGroupOptions { get; set; }
        public List<SelectListItem> MenuOptions { get; set; }
        public String[] IconList { 
            get { 
                return new String[6]  { "fa-dashboard", "fa-calendar", "fa-gavel", "fa-edit", "fa-bar-chart-o", "fa-cog" };
            }
        }
    }
}