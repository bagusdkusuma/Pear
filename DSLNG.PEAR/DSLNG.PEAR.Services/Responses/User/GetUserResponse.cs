


using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.User
{
    public class GetUserResponse : BaseResponse
    {
        public GetUserResponse()
        {
            RolePrivileges = new List<RolePrivilege>();
        }
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public RoleGroup Role { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string FullName { get; set; }
        public string SignatureImage { get; set; }
        public string Position { get; set; }
        public string ChangeModel { get; set; }
        public IList<RolePrivilege> RolePrivileges { get; set; }
        public class RoleGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }

            public Level Level { get; set; }
        }

        public class Level {
            public int Id { get; set; }

            public string Code { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
            public string Remark { get; set; }

            public bool IsActive { get; set; }
        }

        public class RolePrivilege
        {
            public int Id { get; set; }
            public int RoleGroup_Id { get; set; }
            public string Name { get; set; }
            public string Descriptions { get; set; }
            public RoleGroup RoleGroup { get; set; }
        }
    }

    
}
