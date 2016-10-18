using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Privilege
{
    public class GetPrivilegeByRoleRequest
    {
        public int RoleId { get; set; }
    }

    public class GetPrivilegeByRolePrivilegeRequest
    {
        public int RoleId { get; set; }
        public int RolePrivilegeId { get; set; }
    }
}