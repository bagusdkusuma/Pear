
using System;

namespace DSLNG.PEAR.Services.Responses.VesselSchedule
{
    public class GetVesselScheduleResponse
    {
        public int Id { get; set; }
        public int VesselId { get; set; }
        public string VesselName { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public bool IsActive { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string Location { get; set; }
        public string SalesType { get; set; }
        public string Cargo { get; set; }
        public string Type { get; set; }
    }
}
