

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.KpiTransformationEngine
{
    public class KpiTransformation
    {
        public KpiTransformation() {
            RoleGroups = new List<RoleGroup>();
            Kpis = new List<Kpi>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public ICollection<RoleGroup> RoleGroups { get; set; }
        public ICollection<Kpi> Kpis { get; set; }
        public DateTime? LastProcessing { get; set; }
        public ICollection<KpiTransformationSchedule> Schedules {get;set;}
    }
}
