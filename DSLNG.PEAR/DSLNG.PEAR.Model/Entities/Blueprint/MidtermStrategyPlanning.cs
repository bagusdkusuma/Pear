

using System.ComponentModel.DataAnnotations;
namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class MidtermStrategyPlanning
    {
        [Key]
        public int Id { get; set; }
        public PlanningBlueprint PlanningBlueprint { get; set; }
        public bool IsLocked { get; set; }
        public bool IsBeingReviewed { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string Notes { get; set; }
    }
}
