﻿
using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class BarChartDataViewModel
    {
        public BarChartDataViewModel() {
            Series = new List<SeriesViewModel>();
        }
        public string BarType { get; set; }
        public string Title { get; set; }
        public string[] Periodes { get; set; }
        public string ValueAxisTitle { get; set; }
        public IList<SeriesViewModel> Series { get; set; }
        public class SeriesViewModel
        {
            public string name { get; set; }
            public IList<double> data { get; set; }
            public string stack { get; set; }
        }
    }

    
}