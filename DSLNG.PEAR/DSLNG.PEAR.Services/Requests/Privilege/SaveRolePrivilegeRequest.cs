using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Privilege
{
    public class SaveRolePrivilegeRequest : BaseRequest
    {
        public SaveRolePrivilegeRequest()
        {
            MenuRolePrivileges = new List<MenuRolePrivilege>();
        }
        public int Id { get; set; }
        public int RoleGroup_Id { get; set; }
        public string Name { get; set; }
        public string Descriptions { get; set; }
        //public int UserId { get; set; }
        public List<MenuRolePrivilege> MenuRolePrivileges { get; set; }
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
        }
    }

    public class UpdateMenuRolePrivilegeRequest
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
    }
    public class BatchUpdateMenuRolePrivilegeRequest
    {
        public BatchUpdateMenuRolePrivilegeRequest()
        {
            BatchUpdateMenuRolePrivilege = new List<UpdateMenuRolePrivilegeRequest>();
        }
        public IList<UpdateMenuRolePrivilegeRequest> BatchUpdateMenuRolePrivilege { get; set; }
    }
}
