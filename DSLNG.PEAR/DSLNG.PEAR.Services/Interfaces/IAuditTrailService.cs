
using DSLNG.PEAR.Services.Requests.AuditTrail;
using DSLNG.PEAR.Services.Responses.AuditTrail;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IAuditTrailService
    {
        AuditTrailsResponse GetAuditTrails(GetAuditTrailsRequest request);
        AuditTrailsResponse GetAuditTrail(GetAuditTrailRequest request);
    }
}
