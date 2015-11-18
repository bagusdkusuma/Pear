

using System.ComponentModel.DataAnnotations;
namespace DSLNG.PEAR.Web.ViewModels.Buyer
{
    public class BuyerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}