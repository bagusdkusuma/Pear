

using System.ComponentModel.DataAnnotations;
namespace DSLNG.PEAR.Web.ViewModels.HighlightOrder
{
    public class HighlightOrderViewModel
    {
        public int Id { get; set; }
        [Display(Name="Highlight Type")]
        public string Text { get; set; }
        public int Order { get; set; }
    }
}