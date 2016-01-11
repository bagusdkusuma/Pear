
using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;
using System.Linq;

namespace DSLNG.PEAR.Services.Responses.OutputConfig
{
    public class GetOutputConfigResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int MeasurementId { get; set; }
        public Formula Formula { get; set; }
        public IList<Kpi> Kpis { get; set; }
        public IList<int> KpiIds { get;set;}
        public IList<KeyAssumptionConfig> KeyAssumptions { get; set; }
        public IList<int> KeyAssumptionIds { get; set; }
        public double? ExcludeValue { get; set; }
        public double? ConversionValue { get; set; }
        public ConversionType ConversionType { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public class Kpi {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class KeyAssumptionConfig {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
