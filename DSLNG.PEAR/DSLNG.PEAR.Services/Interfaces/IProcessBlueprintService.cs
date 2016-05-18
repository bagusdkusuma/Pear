using DSLNG.PEAR.Services.Requests.FileManagerRolePrivilege;
using DSLNG.PEAR.Services.Requests.ProcessBlueprint;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.ProcessBlueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IProcessBlueprintService
    {
        GetProcessBlueprintsResponse All();
        GetProcessBlueprintsResponse Gets(GetProcessBlueprintsRequest request);
        GetProcessBlueprintResponse Get(GetProcessBlueprintRequest request);
        SaveProcessBlueprintResponse Save(SaveProcessBlueprintRequest request);
        BaseResponse Delete(int Id);
        GetProcessBlueprintPrivilegesResponse GetPrivileges(GetProcessBlueprintPrivilegeRequest request);
        BaseResponse BatchUpdateFilePrivilege(BatchUpdateFilePrivilegeRequest request);
        BaseResponse InsertOwnerPrivilege(FilePrivilegeRequest request);
    }
}
