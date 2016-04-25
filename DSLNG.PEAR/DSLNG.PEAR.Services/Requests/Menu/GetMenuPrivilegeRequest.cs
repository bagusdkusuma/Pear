using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Menu
{
    public class GetMenuPrivilegeRequest
    {
        public int Menu_Id { get; set; }
        public int RolePrivilege_Id { get; set; }
    }
}
