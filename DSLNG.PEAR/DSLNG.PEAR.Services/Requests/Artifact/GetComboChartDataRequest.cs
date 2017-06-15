

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Requests.Artifact
{
    public class GetComboChartDataRequest
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public RangeFilter RangeFilter { get; set; }
        public int MeasurementId { get; set; }

        public IList<ChartRequest> Charts { get; set; }

        public class ChartRequest
        {
            
            public ValueAxis ValueAxis { get; set; }
            public string GraphicType { get; set; }
            public IList<SeriesRequest> Series { get; set; }
            public class SeriesRequest
            {
                public SeriesRequest()
                {
                    Stacks = new List<StackRequest>();
                }
                public int KpiId { get; set; }
                public string Label { get; set; }
                public ValueAxis ValueAxis { get; set; }
                public IList<StackRequest> Stacks { get; set; }
                public string Color { get; set; }
                public string PreviousColor { get; set; }
                public string MarkerColor { get; set; }
                public string LineType { get; set; }
            }

            public class StackRequest
            {
                public int KpiId { get; set; }
                public string Label { get; set; }
                public string Color { get; set; }
            }
        }
    }
}
