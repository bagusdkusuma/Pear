

using System;
namespace DSLNG.PEAR.Services.Requests.VesselSchedule
{
    public class SaveVesselScheduleRequest
    {
        public SaveVesselScheduleRequest() {
            IsActive = true;
        }
        public int Id { get; set; }
        public int VesselId { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public bool IsActive { get; set; }
        public int BuyerId { get; set; }
        public string Location { get; set; }
        public string SalesType { get; set; }
        public string Type { get; set; }
        public string Cargo { get; set; }
        public DateTime? DerTransactionDate { get; set; }
    }
}
