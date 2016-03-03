

namespace DSLNG.PEAR.Services.Requests.PlanningBlueprint
{
    public  class SavePlanningBlueprintRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int[] KeyOutputIds { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
    }
}
