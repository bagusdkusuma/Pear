using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.User
{
    public class UpdateUserViewModel
    {
        public UpdateUserViewModel()
        {
            RoleGroupList = new List<SelectListItem>();
        }
        [Required]
        public int Id { get; set; }
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Username { get; set; }
        public string Password { get; set; }
        [Display(Name="Change Password")]
        public bool ChangePassword { get; set; }
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public List<SelectListItem> RoleGroupList { get; set; }
        [Display(Name="Default Landing Page")]
        public string ChangeModel { get; set; }
        [Display(Name="User Is Superadmin")]
        public bool IsSuperAdmin { get; set; }
    }
}