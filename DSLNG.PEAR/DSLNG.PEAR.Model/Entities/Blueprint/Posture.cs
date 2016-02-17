

using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class Posture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public IList<DesiredState> DesiredStates { get; set; }
        public IList<PostureConstraint> PostureConstraints { get; set; }
        public IList<PostureChallenge> PostureChallenges { get; set; }
        public PostureType Type { get; set; }

        public BusinessPostureIdentification BusinessPostureIdentification { get; set; }

        //public BusinessPostureIdentification ConstructionPostureHost { get; set; }
        //public BusinessPostureIdentification OperationPostureHost { get; set; }
        //public BusinessPostureIdentification DecommissioningPostureHost { get; set; }
    }
}
