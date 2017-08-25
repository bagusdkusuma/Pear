
using DSLNG.PEAR.Services.Requests.AuditTrail;
using DSLNG.PEAR.Services.Responses.AuditTrail;
using System.Collections;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IAuditTrailService
    {
        AuditTrailsResponse GetAuditTrails(GetAuditTrailsRequest request);
        AuditTrailsResponse GetAuditTrail(GetAuditTrailRequest request);
        AuditTrailsResponse GetAuditTrailDetails(int recordId);
        AuditUsersResponse GetAuditUsers(GetAuditUsersRequest request);
        AuditUsersResponse GetAuditUser(GetAuditUserRequest request);
        AuditUserLoginsResponse GetUserLogins(GetAuditUserLoginsRequest request);
        AuditUserLoginResponse GetUserLogin(GetAuditUserLoginRequest request);
        void CreateAuditUserRequest(CreateAuditUserRequest request);
    }
}
