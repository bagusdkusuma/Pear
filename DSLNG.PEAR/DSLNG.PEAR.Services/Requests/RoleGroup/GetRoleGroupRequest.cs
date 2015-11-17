using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.RoleGroup
{
    public class GetRoleGroupRequest
    {
        public int Id { get; set; }
    }

    public class GetRoleGroupsRequest : GridBaseRequest 
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
