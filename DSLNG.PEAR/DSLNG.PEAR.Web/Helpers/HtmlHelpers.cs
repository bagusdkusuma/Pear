

using System;
using System.Globalization;
using System.Web.Mvc;
namespace DSLNG.PEAR.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString LimitString(this HtmlHelper htmlHelper, string source, int length)
        {
            if (!string.IsNullOrEmpty(source) && source.Length > length)
            {
                var result = source.Substring(0, length);
                result += "... <a class=\"see-more\" href=\"#\" data-toggle=\"modal\" data-target=\"#modalDialog\">See More</a>";
                result += "<div style=\"display:none;color:#fff\" class=\"full-string\">";
                result += source;
                result += "</div>";
                return MvcHtmlString.Create(result);
            }
            return MvcHtmlString.Create(source);
        }

        public static string ParseToDateOrNumber(this HtmlHelper htmlHelper, string val)
        {
            var resultDate = new DateTime();
            bool isValid = false;
            if (string.IsNullOrEmpty(val))
            {
                return val;
            }

            if (val.Length == 4)
            {
                return val;
            }
            if (val.Length == 6 || val.Length == 7)
            {
                if (val.Length == 6)
                {
                    isValid = DateTime.TryParseExact("01-" + val, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                                                     DateTimeStyles.AllowWhiteSpaces, out resultDate);
                }
                else if (val.Length == 7)
                {
                    isValid = DateTime.TryParseExact("01-0" + val, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                                                     DateTimeStyles.AllowWhiteSpaces, out resultDate);
                }
            }
            else if (val.Length >= 20 && val.Length <= 22)
            {
                isValid = DateTime.TryParseExact(val, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out resultDate);
            }
            else
            {
                isValid = DateTime.TryParseExact(val, "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out resultDate);
            }

            return isValid ? resultDate.ToString("dd MMM yyyy") : ParseToNumber(val);
        }

        public static string ParseToNumber(this HtmlHelper htmlHelper, string val)
        {
            return ParseToNumber(val);
        }

        public static string DisplayDerValue(this HtmlHelper htmlHelper, string val, string defaultVal = "N/A", bool isRounded=true)
        {
           
            return !string.IsNullOrEmpty(val) ? RoundIt(isRounded, val) : defaultVal;
        }

        public static string DisplayCompleteDerValue(this HtmlHelper htmlHelper, string val, string measurement, string defaultMeasurement, string defaultVal = "N/A",
            bool isRounded = true)
        {
            return !string.IsNullOrEmpty(val) ?
                $"{RoundIt(isRounded, val)} {(string.IsNullOrEmpty(measurement) ? defaultMeasurement : measurement)}"
                : defaultVal;
        }

        public static string DisplayDerLabel(this HtmlHelper htmlHelper, string val, string defaultVal)
        {
            return string.IsNullOrEmpty(val) ? defaultVal : val;
        }

        public static string DisplayDerDeviation(this HtmlHelper htmlHelper, string deviation)
        {
            switch (deviation)
            {
                case "1":
                    return "fa-arrow-up";
                case "-1":
                    return "fa-arrow-down";
                default:
                    return "fa-minus";
            }
        }

        public static string DisplayDerValueWithLabelFront(this HtmlHelper htmlHelper, string label, string val)
        {
            
        }

        public static string Divide(this HtmlHelper htmlHelper, string val, int number)
        {
            if (string.IsNullOrEmpty(val)) return val;
            double x = double.Parse(val);
            return (x/number).ToString(CultureInfo.InvariantCulture);
        }

        private static string RoundIt(bool isRounded, string val)
        {
            if (isRounded)
            {
                double v = double.Parse(val);
                val = Math.Round(v, 2).ToString(CultureInfo.InvariantCulture);
            }

            return val;
        }

        private static string ParseToNumber(string val)
        {
            double x;
            var styles = NumberStyles.AllowParentheses | NumberStyles.AllowTrailingSign | NumberStyles.Float | NumberStyles.AllowDecimalPoint;
            bool isValidDouble = Double.TryParse(val, styles, NumberFormatInfo.InvariantInfo, out x);
            //return isValidDouble ? Str x.ToString("0:0.###") : val;
            //return isValidDouble ? string.Format("{0:0,000.###}", x) : val;
            return isValidDouble ? $"{x:#,##0.##}" : val;
        }
    }
}