

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class UltimateObjectivePoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Description { get; set; }
        public EnvironmentsScanning ConstructionPhaseHost { get; set; }
        public EnvironmentsScanning OperationPhaseHost { get; set; }
        public EnvironmentsScanning ReinventPhaseHost { get; set; }
    }
}
