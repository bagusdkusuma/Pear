using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using EPeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;

namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class ArtifactDesignerViewModel
    {
        public ArtifactDesignerViewModel()
        {
            GraphicTypes = new List<SelectListItem>();
            Measurements = new List<SelectListItem>();
            PeriodeTypes = new List<SelectListItem>();
            ValueAxes = new List<SelectListItem>();
            RangeFilters = new List<SelectListItem>();
        }

        public int Id { get; set; }
        [Display(Name= "Graphic Type")]
        public string GraphicType { get; set; }
        public IList<SelectListItem> GraphicTypes { get; set; }
        [Display(Name="Graphic Name")]
        public string GraphicName { get; set; }
        [Display(Name="Header Title")]
        public string HeaderTitle { get; set; }

        [Display(Name="Measurement")]
        public int MeasurementId { get; set; }
        public IList<SelectListItem> Measurements { get; set; }

        [Display(Name = "Periode Type")]
        [Required]
        public string PeriodeType { get; set; }
        public IList<SelectListItem> PeriodeTypes { get; set; }
        [Display(Name = "Range Filter")]
        [Required]
        public string RangeFilter { get; set; }
        public IList<SelectListItem> RangeFilters { get; set; }

        public DateTime? StartAfterParsed
        {
            get
            {
                if (string.IsNullOrEmpty(this.StartInDisplay))
                {
                    return null;
                }
                if (this.PeriodeType == EPeriodeType.Monthly.ToString())
                {
                    return DateTime.ParseExact("01/" + this.StartInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == EPeriodeType.Yearly.ToString())
                {
                    return DateTime.ParseExact("01/01/" + this.StartInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == EPeriodeType.Daily.ToString() || this.PeriodeType == EPeriodeType.Weekly.ToString())
                {
                    return DateTime.ParseExact(this.StartInDisplay, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                return DateTime.ParseExact(this.StartInDisplay, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            }
        }
        public DateTime? EndAfterParsed
        {
            get
            {
                if (string.IsNullOrEmpty(this.EndInDisplay))
                {
                    return null;
                }
                if (this.PeriodeType == EPeriodeType.Monthly.ToString())
                {
                    return DateTime.ParseExact("01/" + this.EndInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == EPeriodeType.Yearly.ToString())
                {
                    return DateTime.ParseExact("01/01/" + this.EndInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == EPeriodeType.Daily.ToString() || this.PeriodeType == EPeriodeType.Weekly.ToString())
                {
                    return DateTime.ParseExact(this.EndInDisplay, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                return DateTime.ParseExact(this.EndInDisplay, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            }
        }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        
        [Display(Name = "Start")]
        public string StartInDisplay { get; set; }
        [Display(Name = "End")]
        public string EndInDisplay { get; set; }
        [Display(Name = "Value Axis")]
        [Required]
        public string ValueAxis { get; set; }
        
        public bool AsNetbackChart { get; set; }
        public IList<SelectListItem> ValueAxes { get; set; }
        [Display(Name = "Is 3D")]
        public bool Is3D { get; set; }
        [Display(Name = "Show Legend")]
        public bool ShowLegend { get; set; }

        public double? FractionScale { get; set; }
        public double? MaxFractionScale { get; set; }
        public int HighlightTypeId { get; set; }
        //chart
        public BarChartViewModel BarChart { get; set; }
        public AreaChartViewModel AreaChart { get; set; }   
        public LineChartViewModel LineChart { get; set; }
        public SpeedometerChartViewModel SpeedometerChart { get; set; }
        public SpiderwebChartViewModel SpiderwebChart { get; set; }
        public MultiaxisChartViewModel MultiaxisChart { get; set; }
        public ComboChartViewModel ComboChart { get; set; }
        public TrafficLightChartViewModel TrafficLightChart { get; set; }
        public TabularViewModel Tabular { get; set; }
        public TankViewModel Tank { get; set; }
        public PieViewModel Pie { get; set; }
    }
}