using DSLNG.PEAR.Services.Requests.RoleGroup;
using DSLNG.PEAR.Services.Responses.RoleGroup;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IRoleGroupService
    {
        GetRoleGroupsResponse All();
        GetRoleGroupsResponse GetRoleGroups(GetRoleGroupsRequest request);
        GetRoleGroupResponse GetRoleGroup(GetRoleGroupRequest request);
        CreateRoleGroupResponse Create(CreateRoleGroupRequest request);
        UpdateRoleGroupResponse Update(UpdateRoleGroupRequest request);
        DeleteRoleGroupResponse Delete(int id);
    }
}
