using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class MidtermStrategicPlanning
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IList<MidtermStrategicPlanningObjective> Objectives { get; set; }
        public IList<MidtermPlanningKpi> Kpis { get; set; }
        public MidtermPhaseFormulationStage Stage { get; set; }
    }
}
