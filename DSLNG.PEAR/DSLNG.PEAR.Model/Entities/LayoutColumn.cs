using DSLNG.PEAR.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities
{
    public class LayoutColumn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Index { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Artifact Artifact { get; set; }
        public TemplateColumnType ColumnType { get; set; }

        public PeriodeType HighlightPeriodeType { get; set; }
        public SelectOption HighlightType { get; set; }

        public bool IsActive { get; set; }
        //public User CreatedBy { get; set; }
        //public User UpdatedBy { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public DateTime UpdatedDate { get; set; }
    }
}
