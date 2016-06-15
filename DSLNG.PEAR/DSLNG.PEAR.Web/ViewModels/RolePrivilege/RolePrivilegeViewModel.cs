using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.RolePrivilege
{
    public class RolePrivilegeViewModel
    {
        public int Id { get; set; }
        [Required]
        public int RoleGroup_Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowHtml]
        public string Descriptions { get; set; }
        public RoleGroup Department { get; set; }

        public class RoleGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}