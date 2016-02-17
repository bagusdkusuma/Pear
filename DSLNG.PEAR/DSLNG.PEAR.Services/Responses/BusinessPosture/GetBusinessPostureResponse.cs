

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.BusinessPosture
{
    public class GetBusinessPostureResponse
    {
        public int Id { get; set; }
        public Posture ConstructionPosture { get; set; }
        public Posture OperationPosture { get; set; }
        public Posture DecommissioningPosture { get; set; }

        public class Posture
        {
            public int Id { get; set; }
            public IList<DesiredState> DesiredStates { get; set; }
            public IList<PostureChallenge> PostureChallenges { get; set; }
            public IList<PostureConstraint> PostureConstraints { get; set; }
        }

        public class DesiredState
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        public class PostureChallenge
        {
            public int Id { get; set; }
            public string Definition { get; set; }
            public bool HasRelation { get; set; }
        }

        public class PostureConstraint
        {
            public int Id { get; set; }
            public string Definition { get; set; }
            public bool HasRelation { get; set; }
        }
    }
}
