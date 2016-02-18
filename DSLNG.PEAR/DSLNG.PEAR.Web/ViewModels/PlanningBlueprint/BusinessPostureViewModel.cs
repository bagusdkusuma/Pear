

using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class BusinessPostureViewModel
    {
        public int Id { get; set; }
        public int PlanningBlueprintId { get; set; }
        public PostureViewModel ConstructionPosture { get; set; }
        public PostureViewModel OperationPosture { get; set; }
        public PostureViewModel DecommissioningPosture { get; set; }

        public class PostureViewModel {
            public int Id { get; set; }
            PostureViewModel() {
                DesiredStates = new List<DesiredStateViewModel>();
                PostureChallenges = new List<PostureChallangeViewModel>();
                PostureConstraints = new List<PostureConstraintViewModel>();
            }
            public IList<DesiredStateViewModel> DesiredStates { get; set; }
            public IList<PostureChallangeViewModel> PostureChallenges { get; set; }
            public IList<PostureConstraintViewModel> PostureConstraints { get; set; }
        }

        public class DesiredStateViewModel {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        public class PostureChallangeViewModel {
            public int Id { get; set; }
            public string Definition { get; set; }
            public bool HasRelation { get; set; }
        }

        public class PostureConstraintViewModel {
            public int Id { get; set; }
            public string Definition { get; set; }
            public bool HasRelation { get; set; }
        }
    }
}