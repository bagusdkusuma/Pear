using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.User
{
    public class LoginUserResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public RoleGroup Role { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string ChangeModel { get; set; }
        public bool IsSuperAdmin { get; set; }
        public class RoleGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
