using DSLNG.PEAR.Services.Requests.KpiTransformationLog;
using DSLNG.PEAR.Services.Responses.KpiTransformationLog;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IKpiTransformationLogService
    {
        SaveKpiTransformationLogResponse Save(SaveKpiTransformationLogRequest request);
        GetKpiTransformationLogsResponse Get(GetKpiTransformationLogsRequest request);
    }
}
