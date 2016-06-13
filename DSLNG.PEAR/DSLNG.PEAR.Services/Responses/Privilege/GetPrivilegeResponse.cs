using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Privilege
{
    public class GetPrivilegesResponse : BaseResponse
    {

        public int TotalRecords { get; set; }
        public IList<RolePrivilege> Privileges { get; set; }
        public class RolePrivilege
        {
            public int Id { get; set; }
            public int RoleGroup_Id { get; set; }
            public string Name { get; set; }
            public string Descriptions { get; set; }
        }
    }

    public class GetPrivilegeResponse : BaseResponse
    {
        public int Id { get; set; }
        public int RoleGroup_Id { get; set; }
        public string Name { get; set; }
        public string Descriptions { get; set; }
        public RoleGroup Department { get; set; }

        public class RoleGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
