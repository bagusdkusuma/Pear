using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Data.Entities
{
    public class PmsConfigDetails : BaseEntity
    {
        public PmsConfigDetails()
        {
            ScoreIndicators = new Collection<ScoreIndicator>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public PmsConfig PmsConfig { get; set; }
        public Kpi Kpi { get; set; }
        public double Weight { get; set; }
        public ScoringType ScoringType { get; set; }
        public bool AsGraphic { get; set; }
        public ICollection<ScoreIndicator> ScoreIndicators { get; set; } 
        //public ScoreIndicator ScoreIndicator { get; set; }
        //public ICollection<KpiTarget> KpiTargets { get; set; }
        //public ICollection<KpiAchievement> KpiAchievements { get; set; }
        //test
        public TargetType TargetType { get; set; }
        public string Target { get; set; }
        public bool IsActive { get; set; }
    }
}
