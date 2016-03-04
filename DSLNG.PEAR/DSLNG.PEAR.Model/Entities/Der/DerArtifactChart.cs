using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Data.Entities.Der
{
    public class DerArtifactChart
    {
        public DerArtifactChart()
        {
            Series = new List<DerArtifactSerie>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string GraphicType { get; set; }
        public ICollection<DerArtifactSerie> Series { get; set; }
        public ValueAxis ValueAxis { get; set; }
        public Measurement Measurement { get; set; }
        public string ValueAxisTitle { get; set; }
        public string ValueAxisColor { get; set; }
        public double? FractionScale { get; set; }
        public double? MaxFractionScale { get; set; }
        public bool IsOpposite { get; set; }
    }
}
