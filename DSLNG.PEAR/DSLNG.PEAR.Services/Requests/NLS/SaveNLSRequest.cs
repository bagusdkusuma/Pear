

using System;
namespace DSLNG.PEAR.Services.Requests.NLS
{
    public class SaveNLSRequest
    {
        public int Id { get; set; }
        public int VesselScheduleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Remark { get; set; }
        public DateTime? DerTransactionDate { get; set; }
    }
}
