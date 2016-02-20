

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using EPeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;
namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class TabularViewModel
    {
        public TabularViewModel() {
            Rows = new List<RowViewModel>();
            PeriodeTypes = new List<SelectListItem>();
            RangeFilters = new List<SelectListItem>();
        }
        public bool Actual { get; set; }
        public bool Target { get; set; }
        public bool Economic { get; set; }
        public bool Fullfillment { get; set; }
        public bool Remark { get; set; }
        public IList<RowViewModel> Rows { get; set; }
        public IList<SelectListItem> PeriodeTypes { get; set; }
        public IList<SelectListItem> RangeFilters { get; set; }
        public class RowViewModel {
            public RowViewModel() {
                 
            }
            [Display(Name="Kpi")]
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            [Display(Name = "Periode Type")]
            [Required]
            public string PeriodeType { get; set; }
           
            [Display(Name = "Range Filter")]
            [Required]
            public string RangeFilter { get; set; }
           
            public DateTime? Start
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
            public DateTime? End
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
            [Display(Name = "Start")]
            public string StartInDisplay { get; set; }
            [Display(Name = "End")]
            public string EndInDisplay { get; set; }

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

        }
    }
}