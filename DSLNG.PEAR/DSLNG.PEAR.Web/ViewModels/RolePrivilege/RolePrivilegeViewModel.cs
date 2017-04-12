using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.RolePrivilege
{
    public class RolePrivilegeViewModel
    {
        public RolePrivilegeViewModel()
        {
        }
        public int Id { get; set; }
        [Required]
        public int RoleGroup_Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowHtml]
        public string Descriptions { get; set; }
        public RoleGroup Department { get; set; }
        public List<MenuRolePrivilege> MenuRolePrivileges { get; set; }

        public class RoleGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class MenuRolePrivilege
        {
            public int Menu_Id { get; set; }
            public int RolePrivilege_Id { get; set; }
            public bool AllowCreate { get; set; }
            public bool AllowUpdate { get; set; }
            public bool AllowDelete { get; set; }
            public bool AllowView { get; set; }
            public bool AllowDownload { get; set; }
            public bool AllowUpload { get; set; }
            public bool AllowPublish { get; set; }
            public bool AllowApprove { get; set; }
            public bool AllowInput { get; set; }
            public MenuPrivilege Menu { get; set; }
            public class MenuPrivilege
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }
    }

    public class MenuRolePrivilegeViewModel
    {
        [Key, Column(Order = 0)]
        public int Menu_Id { get; set; }
        [Key, Column(Order = 1)]
        public int RolePrivilege_Id { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowUpdate { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowView { get; set; }
        public bool AllowDownload { get; set; }
        public bool AllowUpload { get; set; }
        public bool AllowPublish { get; set; }
        public bool AllowApprove { get; set; }
        public bool AllowInput { get; set; }
        public MenuPrivilege Menu { get; set; }
        public class MenuPrivilege
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }

    public class AddRolePrivilegeViewModel
    {
        public AddRolePrivilegeViewModel()
        {
            RoleGroupList = new List<SelectListItem>();
        }
        public int Id { get; set; }
        [Required]
        public int RoleGroup_Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowHtml]
        public string Descriptions { get; set; }
        public RoleGroup Department { get; set; }

        public List<SelectListItem> RoleGroupList { get; set; }

        public class RoleGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}