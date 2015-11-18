using DSLNG.PEAR.Services.Requests.VesselSchedule;
using DSLNG.PEAR.Services.Responses.VesselSchedule;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IVesselScheduleService
    {
        GetVesselScheduleResponse GetVesselSchedule(GetVesselScheduleRequest request);
        GetVesselSchedulesResponse GetVesselSchedules(GetVesselSchedulesRequest request);
        SaveVesselScheduleResponse SaveVesselSchedule(SaveVesselScheduleRequest request);
        DeleteVesselScheduleResponse Delete(DeleteVesselScheduleRequest request);
        GetVesselSchedulesResponse GetVesselSchedulesForGrid(GetVesselSchedulesRequest request);
    }
}
