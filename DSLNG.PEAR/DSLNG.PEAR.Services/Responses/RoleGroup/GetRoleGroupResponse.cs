using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.RoleGroup
{
    public class GetRoleGroupResponse : BaseResponse
    {
        public GetRoleGroupResponse()
        {
            Privileges = new List<RolePrivilege>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; }
        public Level Level { get; set; }
        public IList<RolePrivilege> Privileges { get; set; }
        
        public class RolePrivilege
        {
            public int Id { get; set; }
            public int RoleGroup_Id { get; set; }
            public string Name { get; set; }
            public string Descriptions { get; set; }
        } 
        
    }
}
