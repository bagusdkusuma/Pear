using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Privilege
{
    public class GetPrivilegesResponse : BaseResponse
    {

        public int TotalRecords { get; set; }
        public IList<RolePrivilege> Privileges { get; set; }
        public class RolePrivilege
        {
            public int Id { get; set; }
            public int RoleGroup_Id { get; set; }
            public string Name { get; set; }
            public string Descriptions { get; set; }
        }
    }

    public class GetPrivilegeResponse : BaseResponse
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

    public class GetMenuRolePrivilegeResponse : BaseResponse
    {
        public IList<MenuRolePrivilege> MenuRolePrivileges { get; set; }
        public int TotalRecords { get; set; }

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
            public Privilege RolePrivilege { get; set; }

            public class Privilege
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public RoleGroup Department { get; set; }

                public class RoleGroup
                {
                    public int Id { get; set; }
                    public string Name { get; set; }
                }
            }

            public class MenuPrivilege
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public string Url { get; set; }
            }
        }
    }

    public class SaveRolePrivilegeResponse : BaseResponse
    {
        public int Id { get; set; }
    }
}
