
using System;

namespace DSLNG.PEAR.Services.Requests.DerLoadingSchedule
{
    public class GetDerLoadingSchedulesRequest
    {
        public DateTime? Periode { get; set; }
        public bool StrictDate { get; set; }
    }
}
