using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Requests.Artifact
{
    public class GetPieDataRequest
    {
        public GetPieDataRequest() {
            ValueInformation = ArtifactValueInformation.AsOf;
        }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public RangeFilter RangeFilter { get; set; }
        public ValueAxis ValueAxis { get; set; }
        public IList<SeriesRequest> Series { get; set; }
        public string HeaderTitle { get; set; }
        public bool Is3D { get; set; }
        public bool ShowLegend { get; set; }
        public ArtifactValueInformation ValueInformation { get; set; }
        public class SeriesRequest
        {

            public int KpiId { get; set; }
            public string Label { get; set; }
            public string Color { get; set; }
        }
    }
}
