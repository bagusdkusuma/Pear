

using DSLNG.PEAR.Services.Requests.DerLoadingSchedule;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.DerLoadingSchedule;
using System;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IDerLoadingScheduleService
    {
        BaseResponse SaveSchedules(int[] ids, DateTime date);
        GetDerLoadingSchedulesResponse Get(GetDerLoadingSchedulesRequest request);
    }
}
