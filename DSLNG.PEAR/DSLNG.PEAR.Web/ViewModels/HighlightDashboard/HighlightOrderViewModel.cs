

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace DSLNG.PEAR.Web.ViewModels.HighlightOrder
{
    public class HighlightOrderViewModel
    {
        public HighlightOrderViewModel() { 
        
        }

        public int Id { get; set; }
        [Display(Name="Highlight Type")]
        public string Text { get; set; }
        public int? Order { get; set; }
        public int GroupId { get; set; }
        public IList<SelectListItem> Groups { get; set; }
        public List<int> RoleGroupIds { get; set; }
        public IList<SelectListItem> RoleGroupOptions { get; set; }
        public bool? IsActive { get; set; }
    }
}