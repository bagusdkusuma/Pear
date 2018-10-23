using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Artifact
{
    public class ExportSettingData
    {
        public int KpiId { get; set; }
        public string KpiName { get; set; }
        public string MeasurementName { get; set; }
        public double? Value { get; set; }
        public DateTime Periode { get; set; }
        public string ValueAxes { get; set; }
        public int KpiReferenceId { get; set; }
        public string KpiGraphicType { get; set; }
    }

    public class KpiExport
    {
        public int  KpiId { get; set; }
        public string KpiName { get; set; }
        public string MeasurementName { get; set; }
        public string  KpiGraphicType { get; set; }
        public string ValueAxes { get; set; }
        public int KpiReferenceId { get; set; }
    }
}
