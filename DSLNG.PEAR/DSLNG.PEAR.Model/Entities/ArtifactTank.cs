
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class ArtifactTank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Kpi VolumeInventory { get; set; }
        public Kpi DaysToTankTop { get; set; }
        public string DaysToTankTopTitle {get;set;}
        public double MinCapacity { get; set; }
        public double MaxCapacity { get; set; }
        [DefaultValue("#3949AB")]
        public string Color { get; set; }
        [DefaultValue(true)]
        public bool ShowLine { get; set; }
    }
}
