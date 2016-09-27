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
            RolePrivilegeOption = new List<SelectListItem>();
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
        [Display(Name ="Department")]
        public int RoleId { get; set; }
        public List<SelectListItem> RoleGroupList { get; set; }
        [Display(Name ="Privileges")]
        public List<int> RolePrivilegeIds { get; set; }
        public List<SelectListItem> RolePrivilegeOption { get; set; }
        [Display(Name="Default Landing Page")]
        public string ChangeModel { get; set; }
        [Display(Name="User Is Superadmin")]
        public bool IsSuperAdmin { get; set; }
        [Display(Name = "Full Name")]
        [MaxLength(150)]
        public string FullName { get; set; }
        public string SignatureImage { get; set; }
        [Display(Name = "Position")]
        [MaxLength(150)]
        public string Position { get; set; }
    }
}