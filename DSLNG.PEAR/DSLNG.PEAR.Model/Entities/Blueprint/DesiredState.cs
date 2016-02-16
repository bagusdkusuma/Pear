

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class DesiredState
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Description { get; set; }
        public Posture Posture { get; set; }
        public IList<PostureChallenge> PostureChallenges { get; set; }
        public IList<PostureConstraint> PostureConstraints { get; set; }
    }
}
