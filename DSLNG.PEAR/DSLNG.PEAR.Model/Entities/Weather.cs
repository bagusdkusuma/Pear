
using DSLNG.PEAR.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities
{
    public class Weather
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public DateTime Date { get; set; }
        public SelectOption Value { get; set; }
        public string Temperature { get; set; }
    }
}
