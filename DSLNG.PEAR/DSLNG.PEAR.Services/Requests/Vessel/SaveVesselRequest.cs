

namespace DSLNG.PEAR.Services.Requests.Vessel
{
    public class SaveVesselRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Capacity { get; set; }
        public string Type { get; set; }
        public int MeasurementId { get; set; }
    }
}
