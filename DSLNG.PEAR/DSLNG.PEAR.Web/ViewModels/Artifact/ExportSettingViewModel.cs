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
        [Display(Name = "KPI")]
        public string KpiId { get; set; }
        public string[] KpiIds { get; set; }
        public string PeriodeType { get; set; }
        public string RangeFilter { get; set; }
        public string GraphicType { get; set; }
        public int ArtifactId { get; set; }

        [Required]
        [Display(Name = "From")]
        public string StartInDisplay { get; set; }
        [Required]
        [Display(Name = "To")]
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

        [Required]
        [StringLength(300, ErrorMessage = "Minimum File Name is 7 Characters", MinimumLength = 7)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        public string Name { get; set; }

        public bool AsNetBackChart { get; set; }
    }
}