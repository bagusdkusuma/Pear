using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.VesselSchedule
{
    public class GetVesselSchedulesResponse
    {
        public int Count { get; set; }
        public IList<VesselScheduleResponse> VesselSchedules { get; set; }
        public class VesselScheduleResponse {
            public VesselScheduleResponse() {
                //NLSResponse = new NLSResponse();
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
            public string Remark { get; set; }
            public DateTime? RemarkDate { get; set; }
            //public NLSResponse NLSResponse { get; set; }
        }

        //public class NLSResponse{
        //    public DateTime CreatedAt { get; set; }
        //    public string Remark { get; set; }
        //}
    }
}
