

using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Requests.Artifact
{
    public class GetSpeedometerChartDataRequest
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public RangeFilter RangeFilter { get; set; }
        public ValueAxis ValueAxis { get; set; }
        public SeriesRequest Series { get; set; }
        public SeriesRequest LabelSeries { get; set; }
        public IList<PlotBandRequest> PlotBands { get; set; }
        public class SeriesRequest
        {
           
            public int KpiId { get; set; }
            public string Label { get; set; }
        }

        public class PlotBandRequest
        {
            public double From { get; set; }
            public double To { get; set; }
            public string Color { get; set; }
        }
    }
}
