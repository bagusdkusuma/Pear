

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
            ProcessingType = ProcessingType.Instant;
            Status = KpiTransformationStatus.WaitingSchedule;
            SelectedKpis = new List<Kpi>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public KpiTransformation KpiTransformation { get; set; }
        public ICollection<Kpi> SelectedKpis { get; set; }
        public DateTime ProcessingDate { get; set; }
        public ProcessingType ProcessingType { get; set; }
        public KpiTransformationStatus Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ICollection<KpiTransformationLog> Logs { get; set; }
      
    }
}
