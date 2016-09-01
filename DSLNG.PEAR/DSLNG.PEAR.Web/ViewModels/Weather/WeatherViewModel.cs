

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using EPeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;

namespace DSLNG.PEAR.Web.ViewModels.Weather
{
    public class WeatherViewModel
    {
        public WeatherViewModel()
        {
            PeriodeTypes = new List<SelectListItem>();
            Values = new List<SelectListItem>();
        }
        public int Id { get; set; }
        [Display(Name = "Periode Type")]
        [Required]
        public string PeriodeType { get; set; }
        public IList<SelectListItem> PeriodeTypes { get; set; }
        public DateTime? _date;
        public DateTime? Date
        {
            set
            {
                if (string.IsNullOrEmpty(this.PeriodeType)) {
                    this._date = value;
                }
                else if (!value.HasValue)
                {
                    this.DateInDisplay = "";
                }
                else if (this.PeriodeType == EPeriodeType.Monthly.ToString())
                {
                    this.DateInDisplay = value.Value.ToString("MM/yyyy");
                }
                else if (this.PeriodeType == EPeriodeType.Yearly.ToString())
                {
                    this.DateInDisplay = value.Value.ToString("yyyy");
                }
                else if (this.PeriodeType == EPeriodeType.Daily.ToString() || this.PeriodeType == EPeriodeType.Weekly.ToString())
                {
                    this.DateInDisplay = value.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    this.DateInDisplay = value.Value.ToString("MM/dd/yyyy hh:mm tt");
                }
            }
            get
            {
                if (this._date.HasValue)
                {
                    return this._date;
                }
                if (string.IsNullOrEmpty(this.DateInDisplay))
                {
                    return null;
                }
                if (this.PeriodeType == EPeriodeType.Monthly.ToString())
                {
                    return DateTime.ParseExact("01/" + this.DateInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == EPeriodeType.Yearly.ToString())
                {
                    return DateTime.ParseExact("01/01/" + this.DateInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == EPeriodeType.Daily.ToString() || this.PeriodeType == EPeriodeType.Weekly.ToString())
                {
                    return DateTime.ParseExact(this.DateInDisplay, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                return DateTime.ParseExact(this.DateInDisplay, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            }
        }
        [Required]
        [Display(Name = "Date")]
        public string DateInDisplay { get; set; }
        [Required]
        [Display(Name="Value")]
        public int ValueId { get; set; }
        public IList<SelectListItem> Values { get; set; }
        [Required]
        public string Temperature { get; set; }
        public string DerValueType { get; set; }
    }
}