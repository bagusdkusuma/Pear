
namespace DSLNG.PEAR.Services.Requests.VesselSchedule
{
    public class GetVesselSchedulesRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
        public string Term { get; set; }
        public bool allActiveList { get; set; }
    }
}
