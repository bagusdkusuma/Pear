using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities
{
    public class ArtifactSerie
    {
        public ArtifactSerie() {
            Stacks = new List<ArtifactStack>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Label { get; set; }
        public ICollection<ArtifactStack> Stacks { get; set; }
        public Artifact Artifact { get; set; }
        //public string Aggregation { get; set; }
        public string Color { get; set; }
        public string PreviousColor { get; set; }
        public Kpi Kpi { get; set; }
        public ValueAxis ValueAxis { get; set; }
        public string MarkerColor { get; set; }
        public string LineType { get; set; }

        public bool IsActive { get; set; }
    }
}
