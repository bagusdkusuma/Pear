using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.Der
{
    public class DerArtifact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public string HeaderTitle { get; set; }
        public Measurement Measurement { get; set; }
        [Required]
        public string GraphicType { get; set; }

        public ICollection<DerArtifactSerie> Series { get; set; }
        public ICollection<DerArtifactChart> Charts { get; set; } 
    }
}
