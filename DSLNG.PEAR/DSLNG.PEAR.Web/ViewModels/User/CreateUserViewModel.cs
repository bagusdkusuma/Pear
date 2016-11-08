using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.User
{
    public class CreateUserViewModel
    {
        public CreateUserViewModel()
        {
            RoleGroupList = new List<SelectListItem>();
            RolePrivilegeOption = new List<SelectListItem>();
        }

        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }
        [Display(Name = "Default Landing Page")]
        public string ChangeModel { get; set; }
        public bool IsActive { get; set; }
        [Display(Name="Role")]
        public int RoleId { get; set; }
        public bool IsSuperAdmin { get; set; }
        [Display(Name = "Full Name")]
        [MaxLength(150)]
        public string FullName { get; set; }
        public string SignatureImage { get; set; }
        [Display(Name ="Position")]
        [MaxLength(150)]
        public string Position { get; set; }
        public List<SelectListItem> RoleGroupList { get; set; }
        [Display(Name ="Privileges")]
        public List<int> RolePrivilegeIds { get; set; }
        public List<SelectListItem> RolePrivilegeOption { get; set; }
    }
}