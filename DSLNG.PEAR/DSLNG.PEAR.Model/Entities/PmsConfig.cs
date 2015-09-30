using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Data.Entities
{
    public class PmsConfig
    {
        public PmsConfig()
        {
            ScoreIndicators = new Collection<ScoreIndicator>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Pillar Pillar { get; set; }
        public double Weight { get; set; }
        public ScoringType ScoringType { get; set; }
        public ICollection<ScoreIndicator> ScoreIndicators { get; set; }
        public bool IsActive { get; set; }
        public ICollection<PmsConfigDetails> PmsConfigDetailsList { get; set; }
        public PmsSummary PmsSummary { get; set; }
    }
}
