using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSLNG.PEAR.Common.Contants;
using DSLNG.PEAR.Common.Helpers;

namespace DSLNG.PEAR.Web.Helpers
{
    public class NumberHelper
    {
        public static string DoubleToDecimalFormat(double? input)
        {

            return (input.HasValue) ? KiloFormat(input.Value,2) : "-";
        }

        public static string DecimalFormat(decimal input)
        {

            return input.ToString(FormatNumber.DecimalFormat);
        }

        public static string KiloFormat(double input, int digit)
        {
            if (input < 1000)
            {
                //var format = string.Format("N{0}",digit.ToString());
                return DecimalFormat((decimal)input);
            }
            var number = SIPrefix.GetInfo((long)input, digit);
            return number.AmountWithPrefix;
        }
    }
}