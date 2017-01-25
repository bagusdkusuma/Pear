using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Data.Entities
{
    public class KpiAchievement : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Kpi Kpi { get; set; }
        public double? Value { get; set; }
        public double? Mtd { get; set; }
        public double? Ytd { get; set; }
        public double? Itd { get; set; }
        public string Deviation { get; set; }
        public string MtdDeviation { get; set; }
        public string YtdDeviation { get; set; }
        public string ItdDeviation { get; set; }
        public DateTime Periode { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public string Remark { get; set; }

        public bool IsActive { get; set; }
        public string UpdateFrom { get; set; }
    }
}
