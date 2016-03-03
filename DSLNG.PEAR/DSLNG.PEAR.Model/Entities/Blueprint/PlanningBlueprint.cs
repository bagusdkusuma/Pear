

using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class PlanningBlueprint
    {
        public PlanningBlueprint() {
            KeyOutput = new List<KeyOutputConfiguration>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public EnvironmentsScanning EnvironmentsScanning { get; set; }
        public BusinessPostureIdentification BusinessPostureIdentification { get; set; }
        public MidtermPhaseFormulation MidtermPhaseFormulation { get; set; }
        public MidtermStrategyPlanning MidtermStragetyPlanning { get; set; }
        public IList<KeyOutputConfiguration> KeyOutput { get; set; }
        public bool IsLocked { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
