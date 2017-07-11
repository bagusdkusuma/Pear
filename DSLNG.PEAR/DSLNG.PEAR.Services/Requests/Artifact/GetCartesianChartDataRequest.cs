
using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Requests.Artifact
{
    public class GetCartesianChartDataRequest
    {
        public string GraphicType { get; set; }
        public string GraphicName { get; set; }
        public string HeaderTitle { get; set; }
        public int MeasurementId { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public RangeFilter RangeFilter { get; set; }
        public ValueAxis ValueAxis { get; set; }
        public bool AsNetbackChart { get; set; }
        //public double FractionScale { get; set; }
        //public double MaxValue { get; set; }
        public IList<SeriesRequest> Series { get; set; }

        public class SeriesRequest
        {
            public SeriesRequest()
            {
                Stacks = new List<StackRequest>();
            }
            public int Order { get; set; }
            public int KpiId { get; set; }
            public string Label { get; set; }
            public ValueAxis ValueAxis { get; set; }
            public IList<StackRequest> Stacks { get; set; }
            public string Color { get; set; }
            public string PreviousColor { get; set; }
            public string LineType { get; set; }
            public string MarkerColor { get; set; }
        }

        public class StackRequest
        {
            public int KpiId { get; set; }
            public string Label { get; set; }
            public string Color { get; set; }
        }
    }
}
