using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;

namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class ArtifactPreviewViewModel
    {
        public int Id{ get; set; }
        public string GraphicType { get; set; }
        public double? FractionScale { get; set; }
        public double? MaxFractionScale { get; set; }
        public string PeriodeType { get; set; }
        public IList<DateTime> TimePeriodes { get; set; }
        public IList<HighlightViewModel> Highlights { get; set; }
        public AreaChartDataViewModel AreaChart { get; set; }
        public BarChartDataViewModel BarChart { get; set; }
        public LineChartDataViewModel LineChart { get; set; }
        public SpeedometerChartDataViewModel SpeedometerChart { get; set; }
        public TrafficLightChartDataViewModel TrafficLightChart { get; set; }
        public TabularDataViewModel Tabular { get; set; }
        public TankDataViewModel Tank { get; set; }
        public MultiaxisChartDataViewModel MultiaxisChart { get; set; }
        public PieDataViewModel Pie { get; set; }
        public ComboChartDataViewModel ComboChart { get; set; }
        public bool AsNetbackChart { get; set; }
        public class HighlightViewModel{
            public string Title {get;set;}
            public string Message {get;set;}
        }
    }
}