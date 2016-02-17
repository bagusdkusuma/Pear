

using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class VoyagePlanViewModel
    {
        public PostureViewModel ConstructionPosture { get; set; }
        public PostureViewModel OperationPosture { get; set; }
        public PostureViewModel DecommissioningPosture { get; set; }
        public IList<UltimateObjectivePointViewModel> ConstructionPhase { get; set; }
        public IList<UltimateObjectivePointViewModel> OperationPhase { get; set; }
        public IList<UltimateObjectivePointViewModel> ReinventPhase { get; set; }
        public IList<ChallengeViewModel> InternalChallenge { get; set; }
        public IList<ChallengeViewModel> ExternalChallenge { get; set; }
        public IList<ConstraintViewModel> Constraints { get; set; }

        public class ChallengeViewModel
        {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class ConstraintViewModel
        {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class PostureViewModel
        {
            public IList<DesiredStateViewModel> DesiredStates { get; set; }
            public IList<PostureChallengeViewModel> PostureChallenges { get; set; }
            public IList<PostureConstraintViewModel> PostureConstraints { get; set; }
        }

        public class DesiredStateViewModel
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        public class PostureChallengeViewModel
        {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class PostureConstraintViewModel
        {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class UltimateObjectivePointViewModel
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}