
using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class MultiaxisChartDataViewModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string[] Periodes { get; set; }
        public IList<ChartViewModel> Charts { get; set; }
        
        public class ChartViewModel {
            public string ValueAxisTitle { get; set; }
            public string Measurement { get; set; }
            public string GraphicType { get; set; }
            public string ValueAxisColor { get; set; }
            public bool IsOpposite { get; set; }
            public string SeriesType { get; set; }
            public IList<SeriesViewModel> Series { get; set; }
            public double? FractionScale { get; set; }
            public double? MaxFractionScale { get; set; }
            public class SeriesViewModel
            {
                public string name { get; set; }
                public IList<double?> data { get; set; }
                public string stack { get; set; }
                public string color { get; set; }
                public string dashStyle { get; set; }
                public MarkerViewModel marker { get; set; }

                public class MarkerViewModel
                {
                    public string fillColor { get; set; }
                    public string lineColor { get; set; }
                }
            }
        }
    }
}