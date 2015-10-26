


namespace DSLNG.PEAR.Services.Responses.User
{
    public class GetUserResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public RoleGroup Role { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSuperAdmin { get; set; }
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
    }

    
}
