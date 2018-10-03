using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;

namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class ExportSettingViewModel
    {
        public ExportSettingViewModel()
        {
            Kpis = new List<SelectListItem>();
        }

        public IList<SelectListItem> Kpis { get; set; }
        public string KpiId { get; set; }
        public string[] KpiIds { get; set; }
        public string PeriodeType { get; set; }

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