using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSLNG.PEAR.Common.Contants;

namespace DSLNG.PEAR.Web.Extensions
{
    public static class NumberExtension
    {
        public static string ToStringFromNullableDouble(this double? input)
        {
            return ToStringFromNullableDouble(input, "-");
        }

        public static string ToStringFromNullableDouble(this double? input, string defaultValue)
        {
            return (input.HasValue) ? input.Value.ToString(FormatNumber.DecimalFormat) : defaultValue;
        }
    }
}