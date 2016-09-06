

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.KpiTransformationEngine
{
    public class KpiTransformationSchedule
    {
        public KpiTransformationSchedule() {
            Periods = new List<KpiTransformationPeriod>();
        }
        [Key]
        //public int Id { get; set; }
        [ForeignKey("KpiTransformation")]
        public int KpiTransFormationId { get; set; }
        public KpiTransformation KpiTransformation { get; set; }
        public ICollection<Kpi> SelectedKpis { get; set; }
        public DateTime ProcessingDate { get; set; }
        public ProcessingType ProcessingType { get; set; }
        public ICollection<KpiTransformationPeriod> Periods { get; set; }
      
    }
}
