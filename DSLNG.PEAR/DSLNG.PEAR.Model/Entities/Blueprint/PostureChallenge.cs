
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class PostureChallenge
    {
        public PostureChallenge() {
            DesiredStates = new List<DesiredState>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Definition { get; set; }
        public IList<DesiredState> DesiredStates { get; set; }
        public Posture Posture { get; set; }
    }
}
