

namespace DSLNG.PEAR.Services.Responses.Artifact
{
    public class GetTankDataResponse : BaseResponse
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string DaysToTankTopTitle { get; set; }
        public double VolumeInventory { get; set; }
        public string VolumeInventoryUnit { get; set; }
        public double DaysToTankTop { get; set; }
        public string DaysToTankTopUnit { get; set; }
        public double MinCapacity { get; set; }
        public double MaxCapacity { get; set; }
        public string Color { get; set; }
        public bool ShowLine { get; set; }
    }
}
