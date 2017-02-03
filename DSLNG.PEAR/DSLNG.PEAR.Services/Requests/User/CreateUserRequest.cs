using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.User
{
    public class CreateUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string ChangeModel { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string FullName { get; set; }
        public string SignatureImage { get; set; }
        public string Position { get; set; }
        public List<int> RolePrivilegeIds { get; set; }
    }

}
