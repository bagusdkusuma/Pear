

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.PlanningBlueprint
{
    public class GetPlanningBlueprintResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IList<KeyOutputResponse> KeyOutputs { get; set; }

        public class KeyOutputResponse {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
