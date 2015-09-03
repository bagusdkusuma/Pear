
using System.ComponentModel.DataAnnotations;
namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class TankViewModel
    {
        public int  Id { get; set; }
        [Display(Name = "Volume Inventory")]
        public int VolumeInventoryId { get; set; }
        public string VolumeInventory { get; set; }
        [Display(Name = "Days to Tank Top")]
        public int DaysToTankTopId { get; set; }
        public string DaysToTankTop { get; set; }
        [Display(Name = "Days to Tank Top Title")]
        public string DaysToTankTopTitle { get; set; }
        [Display(Name = "Min Capacity")]
        public double MinCapacity { get; set; }
        [Display(Name = "Max Capacity")]
        public double MaxCapacity { get; set; }
        public string Color { get; set; }
        [Display(Name = "Show Shipment Ready Line")]
        public bool ShowLine { get; set; }
    }
}