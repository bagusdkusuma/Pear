

namespace DSLNG.PEAR.Services.Requests.PlanningBlueprint
{
    public class GetPlanningBlueprintsRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool OnlyCount { get; set; }
    }
}
