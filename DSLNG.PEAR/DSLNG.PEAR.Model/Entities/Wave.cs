using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Data.Entities
{
    public class Wave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public DateTime Date { get; set; }
        public SelectOption Value { get; set; } //direction
        public string Tide { get; set; }
        public string Speed { get; set; }
    }
}
