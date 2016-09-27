using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.User
{
    public class GetUsersResponse : BaseResponse
    {
        public IList<User> Users { get; set; }
        public int  TotalRecords { get; set; }

        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
            public RoleGroup Role { get; set; }
            ////public int RoleId { get; set; }
            public string RoleName { get; set; }
            public bool IsSuperAdmin { get; set; }
            public string FullName { get; set; }
            public string SignatureImage { get; set; }
            public string Position { get; set; }
        }

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
