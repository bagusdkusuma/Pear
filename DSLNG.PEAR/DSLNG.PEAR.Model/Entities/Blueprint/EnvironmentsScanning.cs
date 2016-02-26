

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class EnvironmentsScanning
    {
        [Key]
        public int Id { get; set; }
        public IList<UltimateObjectivePoint> ConstructionPhase { get; set; }
        public IList<UltimateObjectivePoint> OperationPhase { get; set; }
        public IList<UltimateObjectivePoint> ReinventPhase { get; set; }
        public PlanningBlueprint PlanningBlueprint { get; set; }
        public bool IsLocked { get; set; }
        public IList<EnvironmentalScanning> Threat { get; set; }
        public IList<EnvironmentalScanning> Opportunity { get; set; }
        public IList<EnvironmentalScanning> Weakness { get; set; }
        public IList<EnvironmentalScanning> Strength { get; set; }
        public IList<Constraint> Constraints { get; set; }
        public IList<Challenge> Challenges { get; set; }
    }
}
