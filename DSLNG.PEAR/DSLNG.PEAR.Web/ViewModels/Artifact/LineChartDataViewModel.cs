
using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class LineChartDataViewModel
    {
        public LineChartDataViewModel()
        {
            Series = new List<SeriesViewModel>();
        }
        public string SeriesType { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string[] Periodes { get; set; }
        public string ValueAxisTitle { get; set; }
        public IList<SeriesViewModel> Series { get; set; }
        public class SeriesViewModel
        {
            public string name { get; set; }
            public IList<double?> data { get; set; }
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