using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Privilege
{
    public class SaveRolePrivilegeRequest
    {
        public int Id { get; set; }
        public int RoleGroup_Id { get; set; }
        public string Name { get; set; }
        public string Descriptions { get; set; }
        public int UserId { get; set; }
    }
}
