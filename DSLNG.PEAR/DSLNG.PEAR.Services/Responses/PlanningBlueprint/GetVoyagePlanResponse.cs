
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.PlanningBlueprint
{
    public class GetVoyagePlanResponse : BaseResponse
    {
        public Posture ConstructionPosture { get; set; }
        public Posture OperationPosture { get; set; }
        public Posture DecommissioningPosture { get; set; }
        public IList<UltimateObjectivePoint> ConstructionPhase { get; set; }
        public IList<UltimateObjectivePoint> OperationPhase { get; set; }
        public IList<UltimateObjectivePoint> ReinventPhase { get; set; }
        public IList<Challenge> InternalChallenge { get; set; }
        public IList<Challenge>  ExternalChallenge { get; set; }
        public IList<Constraint> Constraints { get; set; } 

        public class Challenge{
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class Constraint{
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class Posture {
            public IList<DesiredState> DesiredStates { get; set; }
            public IList<PostureChallenge> PostureChallenges { get; set; }
            public IList<PostureConstraint> PostureConstraints { get; set; }
        }

        public class DesiredState {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        public class PostureChallenge {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class PostureConstraint {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class UltimateObjectivePoint {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}
