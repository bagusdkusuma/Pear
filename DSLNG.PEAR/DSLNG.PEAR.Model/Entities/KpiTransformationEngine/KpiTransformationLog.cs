using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.KpiTransformationEngine
{
    public class KpiTransformationLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public KpiTransformationSchedule Schedule { get; set; }
        public Kpi Kpi { get; set; }
        public DateTime Periode { get; set; }
        public KpiTransformationStatus Status { get; set; }
        public string Notes { get; set; }
    }
}
