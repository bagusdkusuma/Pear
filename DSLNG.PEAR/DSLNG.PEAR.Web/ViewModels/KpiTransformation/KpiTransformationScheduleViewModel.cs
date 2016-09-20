using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.KpiTransformation
{
    public class KpiTransformationScheduleViewModel
    {
        public KpiTransformationScheduleViewModel()
        {
            ProcessingType = ProcessingType.Instant;
        }
        public int KpiTransformationId { get; set; }        
        public PeriodeType PeriodeType { get; set; }
        public ProcessingType ProcessingType { get; set; }
        public string StartInDisplay { get; set; }
        public string EndInDisplay { get; set; }
        public DateTime? Start
        {
            get
            {
                if (string.IsNullOrEmpty(this.StartInDisplay))
                {
                    return null;
                }
                if (this.PeriodeType == PeriodeType.Monthly)
                {
                    return DateTime.ParseExact("01/" + this.StartInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == PeriodeType.Yearly)
                {
                    return DateTime.ParseExact("01/01/" + this.StartInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == PeriodeType.Daily || this.PeriodeType == PeriodeType.Weekly)
                {
                    return DateTime.ParseExact(this.StartInDisplay, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                return DateTime.ParseExact(this.StartInDisplay, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            }
        }
        public DateTime? End {
            get
            {
                if (string.IsNullOrEmpty(this.EndInDisplay))
                {
                    return null;
                }
                if (this.PeriodeType == PeriodeType.Monthly)
                {
                    return DateTime.ParseExact("01/" + this.EndInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == PeriodeType.Yearly)
                {
                    return DateTime.ParseExact("01/01/" + this.EndInDisplay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (this.PeriodeType == PeriodeType.Daily || this.PeriodeType == PeriodeType.Weekly)
                {
                    return DateTime.ParseExact(this.EndInDisplay, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                return DateTime.ParseExact(this.EndInDisplay, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            }
        }
        public IList<int> KpiIds { get; set; }
    }
}