

using DSLNG.PEAR.Data.Enums;
using System;
namespace DSLNG.PEAR.Services.Requests.Artifact
{
    public class GetTankDataRequest
    {
        public string HeaderTitle { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public RangeFilter RangeFilter { get; set; }
        public TankRequest Tank { get; set; }
        public class TankRequest
        {
            public int VolumeInventoryId { get; set; }
            public string VolumeInventory { get; set; }
            public int DaysToTankTopId { get; set; }
            public string DaysToTankTop { get; set; }
            public string DaysToTankTopTitle { get; set; }
            public double MinCapacity { get; set; }
            public double MaxCapacity { get; set; }
            public string Color { get; set; }
            public bool ShowLine { get; set; }
        }
    }
}
