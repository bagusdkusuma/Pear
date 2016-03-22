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
        public Kpi Kpi { get; set; }
        public int Position { get; set; }
        [DefaultValue("false")]
        public bool IsOriginalData { get; set; }
        [DefaultValue(ConfigType.KpiAchievement)]
        public ConfigType ConfigType { get; set; }
        public SelectOption SelectOption { get; set; }
    }
}
