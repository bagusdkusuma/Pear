

using System;
namespace DSLNG.PEAR.Services.Responses.NLS
{
    public class GetNLSResponse
    {
        public int Id { get; set; }
        public int VesselScheduleId { get; set; }
        public string VesselName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Remark { get; set; }
    }
}
