using DSLNG.PEAR.Services.Requests.Privilege;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.Privilege;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IRolePrivilegeService
    {
        GetPrivilegesResponse GetRolePrivileges(GetPrivilegeByRoleRequest request);
        GetPrivilegesResponse GetRolePrivileges(GetPrivilegesRequest request);
        GetPrivilegeResponse GetRolePrivilege(GetPrivilegeRequest request);
        GetMenuRolePrivilegeResponse GetMenuRolePrivileges(GetPrivilegeByRolePrivilegeRequest request);
        SaveRolePrivilegeResponse SaveRolePrivilege(SaveRolePrivilegeRequest request);
        BaseResponse DeleteRolePrivilege(DeleteRolePrivilegeRequest request);
    }
}
