
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.PlanningBlueprint
{
    public class GetPlanningBlueprintsResponse
    {
        public int Count { get; set; }
        public int TotalRecords { get; set; }
        public IList<PlanningBlueprint> PlanningBlueprints { get; set; }
        public class PlanningBlueprint {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public EnvironmentsScanning EnvironmentsScanning { get; set; }
            public BusinessPostureIdentification BusinessPostureIdentification { get; set; }
            public bool IsApproved { get; set; }
            public bool IsActive { get; set; }
            public bool IsLocked { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class EnvironmentsScanning {
            public int Id { get; set; }
            public bool IsLocked { get; set; }
            public bool IsApproved { get; set; }
        }
        public class BusinessPostureIdentification {
            public int Id { get; set; }
            public bool IsLocked { get; set; }
        }
    }
}
