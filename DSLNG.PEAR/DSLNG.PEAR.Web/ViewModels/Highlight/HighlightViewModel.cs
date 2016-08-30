

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using EPeriodeType = DSLNG.PEAR.Data.Enums.PeriodeType;
namespace DSLNG.PEAR.Web.ViewModels.Highlight
{
    public class HighlightViewModel
    {
        public HighlightViewModel()
        {
            PeriodeTypes = new List<SelectListItem>();
            Types = new List<SelectListItem>();
            AlertConditions = new List<SelectListItem>();
            IsActive = true;
        }
        public bool AsNew { get; set; }
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
        [Display(Name = "Highglight Type")]
        [Required]
        public int TypeId { get; set; }
        public IList<SelectListItem> Types { get; set; }
        public IList<SelectListItem> AlertConditions { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public string Property { get; set; }
        public string ValueType { get; set; }
    }
}