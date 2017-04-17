using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime Parse(string periodeType, string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return DateTime.Now.Date;
            }

            if (periodeType == PeriodeType.Monthly.ToString())
            {
                return DateTime.ParseExact("01/" + date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else if (periodeType == PeriodeType.Yearly.ToString())
            {
                return DateTime.ParseExact("01/01/" + date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                return DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
        }
    }
}