

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
                string.Format("{0} {1}", RoundIt(isRounded, val), string.IsNullOrEmpty(measurement) ? defaultMeasurement : measurement) : defaultVal;
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
                case "0":
                    return "fa-minus";
                default:
                    return string.Empty;
            }
        }

        public static string DisplayDerRemark(this HtmlHelper htmlHelper, string deviation)
        {
            switch (deviation)
            {
                case "1":
                    return "fa-circle";
                case "-1":
                    return "fa-times-circle";
                case "0":
                    return "fa-exclamation-circle";
                default:
                    return string.Empty;
            }
        }

        public static string DisplayDerRemarkWithValue(this HtmlHelper htmlHelper, string deviation)
        {
            switch (deviation)
            {
                case "4":
                    return "<span class='indicator left-side'><i class='fa fa-check' style='color:green'></i></span>Fulfilled";
                case "3":
                    return "<span class='indicator left-side'><i class='fa fa-circle' style='color:green'></i></span>On-track";
                case "2":
                    return "<span class='indicator left-side'><i class='fa fa-arrow-right' style='color:grey'></i></span>Loading";
                case "1":
                    return "<span class='indicator left-side'><i class='fa fa-circle' style='color:orange'></i></span>Need attention";
                case "0":
                    return "<span class='indicator left-side'><i class='fa fa-circle' style='color:red'></i></span>Unfulfilled";
                default:
                    return string.Empty;
            }
        }

        public static string DisplayDerRemarkForMarketTone(this HtmlHelper htmlHelper, string deviation)
        {
            switch (deviation)
            {
                case "1":
                    return "<span class='comparison'><i class='fa fa-arrow-up' style='color:green'></i></span>";
                case "0":
                    return "<span class='comparison'><i class='fa fa-minus' style='color:orange'></i></span>";
                case "-1":
                    return "<span class='comparison'><i class='fa fa-arrow-down' style='color:red'></i></span>";
                default:
                    return string.Empty;
            }
        }

        public static string DisplayDerValueWithLabelAtFront(this HtmlHelper htmlHelper, string measurement, string val, string defaultMeasurement, string defaultVal = "N/A", bool isRounded = true)
        {
            return !string.IsNullOrEmpty(val) ?
                string.Format("{1} {0}", RoundIt(isRounded, val), string.IsNullOrEmpty(measurement) ? defaultMeasurement : measurement) : defaultVal;
        }

        public static string Divide(this HtmlHelper htmlHelper, string val, int number)
        {
            if (string.IsNullOrEmpty(val)) return val;
            double x = double.Parse(val);
            return (x/number).ToString(CultureInfo.InvariantCulture);
        }

        public static string DisplayDerValueWithHours(this HtmlHelper htmlHelper, string val, string defaultVal = "N/A")
        {
            if (!string.IsNullOrEmpty(val))
            {
                double v = double.Parse(val);
                TimeSpan span = TimeSpan.FromMinutes(v);
                return span.ToString(@"hh\:mm");
            }

            return defaultVal;
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
            return isValidDouble ? string.Format("{#,##0.##}",x) : val;
        }
    }
}