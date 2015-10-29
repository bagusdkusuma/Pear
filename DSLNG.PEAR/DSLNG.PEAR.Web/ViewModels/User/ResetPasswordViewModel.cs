using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.User
{
    public class ResetPasswordViewModel
    {
        [EmailAddress]
        [Remote("ValidateEmail", "Account", ErrorMessage = "Email are not registered")]
        [Required]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        
    }
}