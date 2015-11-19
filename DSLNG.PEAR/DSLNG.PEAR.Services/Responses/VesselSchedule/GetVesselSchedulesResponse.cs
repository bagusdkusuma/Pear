using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.VesselSchedule
{
    public class GetVesselSchedulesResponse
    {
        public int Count { get; set; }
        public IList<VesselScheduleResponse> VesselSchedules { get; set; }
        public int TotalRecords { get; set; }
        public class VesselScheduleResponse {
            public VesselScheduleResponse() {
            }
            public int id { get; set; }
            public string Vessel { get; set; }
            public string Name { get; set; }
            public DateTime? ETA { get; set; }
            public DateTime? ETD { get; set; }
            public bool IsActive { get; set; }
            public string Buyer { get; set; }
            public string Location { get; set; }
            public string SalesType { get; set; }
            public string Type { get; set; }
            public string VesselType { get; set; }
            public string Cargo { get; set; }
            public string Remark { get; set; }
            public DateTime? RemarkDate { get; set; }
            public string Measurement { get; set; }
            public double Capacity { get; set; }
        }
    }
}
