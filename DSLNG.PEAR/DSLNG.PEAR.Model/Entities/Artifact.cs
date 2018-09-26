using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities
{
    public class Artifact
    {
        public Artifact() {
            Series = new List<ArtifactSerie>();
            Plots = new List<ArtifactPlot>();
            Rows = new List<ArtifactRow>();
            Charts = new List<ArtifactChart>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string GraphicType { get; set; }
        [Required]
        public string GraphicName { get; set; }
        [Required]
        public string HeaderTitle { get; set; }

        public string SeriesType { get; set; }
        public ICollection<ArtifactSerie> Series { get; set; }
        public ICollection<ArtifactPlot> Plots { get; set; }
        public ICollection<ArtifactRow> Rows { get; set; }
        public ICollection<ArtifactChart> Charts { get; set; }
        public ArtifactTank Tank { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public ValueAxis ValueAxis { get; set; }
        public bool? AsNetbackChart { get; set; }
        public RangeFilter RangeFilter { get; set; }
        public Measurement Measurement { get; set; }
        public double? FractionScale { get; set; }
        public double? MaxFractionScale { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
       
        //tabular settings
        public bool Actual { get; set; }
        public bool Target { get; set; }
        public bool Economic { get; set; }
        public bool Fullfillment { get; set; }
        [DefaultValue("false")]
        public bool Is3D { get; set; }
        [DefaultValue("true")]
        public bool ShowLegend { get; set; }
        public bool Remark { get; set; }
        public bool IsActive { get; set; }
        public User CreatedBy { get; set; }
        public User UpdatedBy { get; set; }
        public DateTime CreatedDate
        {
            get
            {
                return this.createdDate.HasValue
                   ? this.createdDate.Value
                   : DateTime.Now;
            }

            set { this.createdDate = value; }
        }
        private DateTime? createdDate = null;
        private DateTime? updatedDate = null;
        public DateTime UpdatedDate
        {
            get
            {
                return this.updatedDate.HasValue
                   ? this.updatedDate.Value
                   : DateTime.Now;
            }

            set { this.updatedDate = value; }
        }
        public ICollection<LayoutColumn> LayoutColumns { get; set; }
    }
}
