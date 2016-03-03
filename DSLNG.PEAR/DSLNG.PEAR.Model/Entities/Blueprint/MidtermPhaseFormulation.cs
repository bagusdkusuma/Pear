

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class MidtermPhaseFormulation
    {
        public MidtermPhaseFormulation() {
            MidtermPhaseFormulationStages = new List<MidtermPhaseFormulationStage>();
        }
        [Key]
        public int Id { get; set; }
        public PlanningBlueprint PlanningBlueprint { get; set; }
        public bool IsLocked { get; set; }
        public IList<MidtermPhaseFormulationStage> MidtermPhaseFormulationStages { get; set; }
    }
}
