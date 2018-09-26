using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Data.Enums;
using EPeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;
using System.Globalization;
namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class BarChartViewModel
    {
        public BarChartViewModel()
        {
            ValueAxes = new List<SelectListItem>();
            Series = new List<SeriesViewModel>();
            SeriesTypes = new List<SelectListItem>();
        }

        public IList<SelectListItem> ValueAxes { get; set; }
        public IList<SeriesViewModel> Series { get; set; }
        public IList<SelectListItem> SeriesTypes { get; set; }
        public string SeriesType { get; set; }
        public class SeriesViewModel
        {
            public SeriesViewModel()
            {
                Stacks = new List<StackViewModel>();
            }
            [Display(Name = "Kpi")]
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string Label { get; set; }
            public string Color { get; set; }
            public string PreviousColor { get; set; }
            public string ValueAxis { get; set; }
            public IList<StackViewModel> Stacks { get; set; }
        }

        public class StackViewModel
        {
            [Display(Name = "Kpi")]
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string Label { get; set; }
            public string Color { get; set; }
        }
    }
}