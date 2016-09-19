using DSLNG.PEAR.Services.Requests.KpiTransformationSchedule;
using DSLNG.PEAR.Services.Responses.KpiTransformationSchedule;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IKpiTransformationScheduleService
    {
        SaveKpiTransformationScheduleResponse Save(SaveKpiTransformationScheduleRequest request);
    }
}
