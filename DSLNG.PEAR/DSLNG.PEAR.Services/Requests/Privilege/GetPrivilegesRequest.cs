using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Privilege
{
    public class GetPrivilegesRequest : GridBaseRequest
    {
        public int RoleId { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
