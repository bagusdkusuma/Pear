

using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Requests.OutputConfig
{
    public class SaveOutputConfigRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int MeasurementId { get; set; }
        public Formula Formula { get; set; }
        public IList<int> KpiIds { get; set; }
        public IList<int> KeyAssumptionIds { get; set; }
        public double? ExcludeValue { get; set; }
        public double? ConversionValue { get; set; }
        public ConversionType? ConversionType { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}
