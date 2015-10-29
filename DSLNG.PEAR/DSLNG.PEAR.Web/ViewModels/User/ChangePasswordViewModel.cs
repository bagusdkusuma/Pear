using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.User
{
    public class ChangePasswordViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "New Password")]
        public string New_Password { get; set; }
        
        [Required]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("New_Password")]
        public string Confirm_Password { get; set; }
        [Remote("CheckPassword", "Account", ErrorMessage = "Wrong currentpassword")]
        [Required]
        [Display(Name="Current Password")]
        public string Old_Password { get; set; }
    }
}