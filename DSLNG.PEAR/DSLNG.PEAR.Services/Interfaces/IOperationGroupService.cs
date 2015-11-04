using DSLNG.PEAR.Services.Requests.OperationGroup;
using DSLNG.PEAR.Services.Responses.OperationGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IOperationGroupService
    {
        GetOperationGroupsResponse GetOperationGroups(GetOperationGroupsRequest request);
        SaveOperationGroupResponse SaveOperationGroup(SaveOperationGroupRequest request);
        GetOperationGroupResponse GetOperationGroup(GetOperationGroupRequest request);
        DeleteOperationGroupResponse DeleteOperationGroup(DeleteOperationGroupRequest request);
    }
}
