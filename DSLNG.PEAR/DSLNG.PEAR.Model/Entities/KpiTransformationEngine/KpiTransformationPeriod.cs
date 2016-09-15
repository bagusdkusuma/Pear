

using DSLNG.PEAR.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.KpiTransformationEngine
{
    public class KpiTransformationPeriod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public KpiTransformationSchedule Schedule { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
