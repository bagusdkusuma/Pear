

using DSLNG.PEAR.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class Highlight
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public SelectOption HighlightType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
    }
}
