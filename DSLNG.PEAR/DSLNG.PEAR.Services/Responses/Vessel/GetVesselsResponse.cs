

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.Vessel
{
    public class GetVesselsResponse
    {
        public IList<VesselResponse> Vessels { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class VesselResponse
        {
            public int id { get; set; }
            public string Name { get; set; }
            public double Capacity { get; set; }
            public string Type { get; set; }
            public string Measurement { get; set; }
            public int MeasurementId { get; set; }
        }
    }
}
