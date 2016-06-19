using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Data.Entities.Der
{
    public class DerKpiInformation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string KpiLabel { get; set; }   
        public Kpi Kpi { get; set; }
        public string KpiMeasurement { get; set; }
        public int Position { get; set; }
        [DefaultValue(ConfigType.KpiAchievement)]
        public ConfigType ConfigType { get; set; }
        public SelectOption SelectOption { get; set; }
    }
}
