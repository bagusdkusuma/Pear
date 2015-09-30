using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSLNG.PEAR.Common.Contants;

namespace DSLNG.PEAR.Web.Helpers
{
    public class NumberHelper
    {
        public static string DoubleToDecimalFormat(double? input)
        {

            return (input.HasValue) ? input.Value.ToString(FormatNumber.DecimalFormat) : "-";
        }

        public static string DecimalFormat(decimal input)
        {

            return input.ToString(FormatNumber.DecimalFormat);
        }
    }
}