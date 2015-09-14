

namespace DSLNG.PEAR.Services.Responses.Vessel
{
    public class GetVesselResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Capacity { get; set; }
        public string Type { get; set; }
        public string Measurement { get; set; }
        public int MeasurementId { get; set; }
    }
}
