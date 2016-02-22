
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class MidtermPhaseFormulationStage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IList<MidtermPhaseDescription> Descriptions { get; set; }
        public IList<MidtermPhaseKeyDriver> KeyDrivers { get; set; }
        public IList<MidtermStrategicPlanning> MidtermStrategicPlannings { get; set; }
        public MidtermPhaseFormulation MidtermPhaseFormulation { get; set; }
    }
}
