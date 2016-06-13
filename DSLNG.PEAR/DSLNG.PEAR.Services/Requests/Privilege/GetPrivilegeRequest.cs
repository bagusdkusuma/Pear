using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Privilege
{
    public class GetPrivilegeRequest
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
    }
}