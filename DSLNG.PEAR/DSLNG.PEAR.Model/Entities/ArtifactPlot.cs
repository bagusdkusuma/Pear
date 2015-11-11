

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class ArtifactPlot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double From { get; set; }
        public double To { get; set; }
        public string Color { get; set; }
        public string Label { get; set; }
        public Artifact Artifact { get; set; }
        public ArtifactChart ArtifactChart { get; set; }
    }
}
