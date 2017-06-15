

using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.Artifact
{
    public class GetCartesianChartDataResponse
    {
        public GetCartesianChartDataResponse()
        {
            Series = new List<SeriesResponse>();
            TimePeriodes = new List<DateTime>();
        }
        public string Subtitle { get; set; }
        public string SeriesType { get; set; }
        public string[] Periodes { get; set; }
        public IList<DateTime> TimePeriodes { get; set; }
        public IList<SeriesResponse> Series { get; set; }
        public class SeriesResponse
        {
            public SeriesResponse()
            {
                Data = new List<double?>();
                BorderColor = "#ffffff";
                ShowInLegend = true;
            }
            public int Order { get; set; }
            public string Name { get; set; }
            public IList<double?> Data { get; set; }
            public string Stack { get; set; }
            public string Color { get; set; }
            public string BorderColor { get; set; }
            public bool ShowInLegend { get; set; }
            public string MarkerColor { get; set; }
            public string LineType { get; set; }
        }
    }
}
