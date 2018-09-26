using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class LineChartViewModel
    {
        public LineChartViewModel()
        {
            Series = new List<SeriesViewModel>();
            ValueAxes = new List<SelectListItem>();
            LineTypes = new List<SelectListItem> {
                new SelectListItem { Text = "Solid", Value="Solid"},
                new SelectListItem { Text = "Dot", Value="Dot"},
                new SelectListItem { Text = "Dash", Value="Dash"}
            };
        }
        public IList<SeriesViewModel> Series { get; set; }
        public IList<SelectListItem> ValueAxes { get; set; }
        public IList<SelectListItem> LineTypes { get; set; }
        public string SeriesType { get; set; }
        public class SeriesViewModel
        {
            [Display(Name = "Kpi")]
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string Label { get; set; }
            public string Color { get; set; }
            public string ValueAxis { get; set; }
            public string MarkerColor { get; set; }
            public string LineType { get; set; }
        }
    }
}