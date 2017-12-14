using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Requests.KpiTransformationSchedule;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.KpiTransformationSchedule;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IKpiTransformationScheduleService
    {
        SaveKpiTransformationScheduleResponse Save(SaveKpiTransformationScheduleRequest request);
        GetKpiTransformationSchedulesResponse Get(GetKpiTransformationSchedulesRequest request);
        GetKpiTransformationSchedulesResponse.KpiTransformationScheduleResponse Get(int Id);
        void UpdateStatus(int id, KpiTransformationStatus status);
        BaseResponse Delete(int id);
        BaseResponse Delete(DeleteKPITransformationScheduleRequest request);
        BaseResponse BatchDelete(int[] ids);
    }
}
