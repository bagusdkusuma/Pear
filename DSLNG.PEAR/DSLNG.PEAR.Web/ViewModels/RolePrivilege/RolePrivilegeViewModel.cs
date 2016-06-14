using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.RolePrivilege
{
    public class RolePrivilegeViewModel
    {
        public int Id { get; set; }
        public int RoleGroup_Id { get; set; }
        public string Name { get; set; }
        public string Descriptions { get; set; }
        public RoleGroup Department { get; set; }

        public class RoleGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}