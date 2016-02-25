using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Data.Entities.Blueprint;

namespace DSLNG.PEAR.Data.Entities.EconomicModel
{
    public class KeyOutputConfiguration
    {
        public KeyOutputConfiguration()
        {
            Kpis = new List<Kpi>();
            KeyAssumptions = new List<KeyAssumptionConfig>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public KeyOutputCategory Category { get; set; }
        public Measurement Measurement { get; set; }
        public Formula Formula { get; set; }
        public IList<Kpi> Kpis { get; set; }
        public string KpiIds { get; set; }
        public IList<KeyAssumptionConfig> KeyAssumptions { get; set; }
        public string KeyAssumptionIds { get; set; }
        public double? ExcludeValue { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public double? ConversionValue { get; set; }
        public ConversionType? ConversionType { get; set; }
        public IList<PlanningBlueprint> PlanningBlueprints { get; set; }
    }
}
