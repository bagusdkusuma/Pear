

using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using DSLNG.PEAR.Web.ViewModels.DerTransaction;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using DSLNG.PEAR.Web.ViewModels.Wave;
using DSLNG.PEAR.Web.ViewModels.Weather;
using DSLNG.PEAR.Web.ViewModels.Der.Display;
using System.Text;
using System.Linq.Expressions;
using System.Web;

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

        public static string DisplayDerValue(this HtmlHelper htmlHelper, string val, string defaultVal = "N/A", bool isRounded = true, int trailingDecimal = 0)
        {

            return !string.IsNullOrEmpty(val) ? RoundIt(isRounded, val, trailingDecimal) : defaultVal;
        }

        public static string DisplayCompleteDerValue(this HtmlHelper htmlHelper, string val, string measurement, string defaultMeasurement, string defaultVal = "N/A",
           bool isRounded = true, int trailingDecimal = 2, bool decimalAfterInt = true)
        {
            if (!string.IsNullOrEmpty(val) && val == "no invtgtn") {
                return val;
            }
            if (!string.IsNullOrEmpty(val) && (!string.IsNullOrEmpty(defaultMeasurement) && defaultMeasurement == "day"))
            {
                if (double.Parse(RoundIt(true, val, 2)) > 0)
                {
                    defaultMeasurement = string.Format("{0}s", defaultMeasurement);
                }
            }
            if (
                (!string.IsNullOrEmpty(measurement) && measurement.ToLowerInvariant() == "MMbtu") ||
                (!string.IsNullOrEmpty(measurement) && measurement.ToLowerInvariant() == "bbtu") ||
                (!string.IsNullOrEmpty(defaultMeasurement) && defaultMeasurement.ToLowerInvariant() == "MMbtu") ||
                (!string.IsNullOrEmpty(defaultMeasurement) && defaultMeasurement.ToLowerInvariant() == "bbtu")
                )
            {

                if (string.IsNullOrEmpty(val))
                {
                    return defaultVal;
                }
                else
                {
                    return string.Format("{0} {1}", RoundIt(isRounded, val, trailingDecimal, decimalAfterInt), string.IsNullOrEmpty(measurement) ? defaultMeasurement : measurement);
                }
            }

            return !string.IsNullOrEmpty(val) ?
                string.Format("{0} {1}", RoundIt(isRounded, val, trailingDecimal, decimalAfterInt), string.IsNullOrEmpty(measurement) ? defaultMeasurement : measurement) : defaultVal;
        }

        public static string DisplayTrafficLight(this HtmlHelper htmlHelper, DisplayKpiInformationViewModel.KpiInformationViewModel kpiInformation, 
            DisplayKpiInformationViewModel.KpiInformationViewModel kpiInformationTarget, string style, string type)
        {
            if (string.IsNullOrEmpty(kpiInformation.DerItemValue.Value)) {
                return string.Empty;
            }

           
            double target;
            double actual;
            ParseIt(kpiInformation.DerItemValue.Value, out actual);
            ParseIt(kpiInformationTarget.DerItemValue.Ytd, out target);

            switch (type)
            {
                case "ph":
                    if(actual >= 6 && actual <=9)
                    {
                        return "<img src='" + VirtualPathUtility.ToAbsolute("~/content/img/der-green-light.png") + "'" + style + "' />";
                    } 
                    break;
                default:
                    if (actual <= 10)
                    {
                        return "<img src='" + VirtualPathUtility.ToAbsolute("~/content/img/der-green-light.png") + "'" + style + "' />";
                    }
                    break;
            }
            
            return "<img src='" + VirtualPathUtility.ToAbsolute("~/content/img/der-red-light.png") + "'" + style + " />";
        }
        public static string DisplayTrafficLight(this HtmlHelper htmlHelper, List<string> list)
        {
            var numbers = new List<double>();
            var isValids = new List<bool>();
            double tempVal;
            foreach (var item in list)
            {
                isValids.Add(ParseIt(item, out tempVal));
                numbers.Add(tempVal);
            }

            for (int i = 0; i < list.Count / 2; i++)
            {
                if(isValids[i] && isValids[(list.Count / 2) + i])
                {
                    var actual = numbers[i];
                    var target = numbers[(list.Count / 2) + i];
                    if(actual > target)
                    {
                        return "<img src='" + VirtualPathUtility.ToAbsolute("~/content/img/der-red-light.png") + "' style='height:29px;display: block;margin: 0 auto;' />";
                    }
                }
                else
                {
                    return string.Empty;
                }
            }

            return "<img src='" + VirtualPathUtility.ToAbsolute("~/content/img/der-green-light.png") + "' style='height:29px;display: block;margin: 0 auto;' />";
        }


        public static string DisplayCompleteDerValueForMmbtu(this HtmlHelper htmlHelper, string val, string defaultVal = "N/A")
        {
            if (string.IsNullOrEmpty(val))
            {
                return defaultVal;
            }
            else
            {
                return string.Format("{0} {1}", RoundIt(true, val, 0), "MMbtu");
            }
        }

        public static string DisplayCompleteDerValueForPlantAvailability(this HtmlHelper htmlHelper, string val, string defaultVal = "N/A")
        {
            if (string.IsNullOrEmpty(val))
            {
                return defaultVal;
            }
            else
            {
                return string.Format("{0} {1}", RoundIt(true, val, 2), "%");
            }
        }

        public static string DisplayDerLabel(this HtmlHelper htmlHelper, string val, string defaultVal)
        {
            return string.IsNullOrEmpty(val) ? defaultVal : val;
        }

        public static string DisplayDerDeviation(this HtmlHelper htmlHelper, string deviation)
        {
            if (string.IsNullOrEmpty(deviation)) return string.Empty;
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

        public static string DisplayDerRemark(this HtmlHelper htmlHelper, string remark)
        {
            return RemarkToIcon(remark);
        }

        public static MvcHtmlString DisplayDerRemarkJson(this HtmlHelper htmlHelper, string remarkJson, string type)
        {
            if (string.IsNullOrEmpty(remarkJson)) return new MvcHtmlString(string.Empty);
            try
            {
                var jsonRemark = JsonConvert.DeserializeObject<JsonRemark>(remarkJson);

                switch (type.ToLowerInvariant())
                {
                    case "daily":
                    case "as of":
                        return RemarkToMvcHtmlString(jsonRemark.Daily);
                    case "mtd":
                        return RemarkToMvcHtmlString(jsonRemark.Mtd);
                    case "ytd":
                        return RemarkToMvcHtmlString(jsonRemark.Ytd);
                    default:
                        return new MvcHtmlString(string.Empty);
                }
            }
            catch (JsonSerializationException exception)
            {
                return new MvcHtmlString(string.Empty);
            }
            catch (Exception exception)
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        public static MvcHtmlString DisplayPlantAvailablityIndicator(this HtmlHelper htmlHelper, DisplayKpiInformationViewModel.KpiInformationViewModel kpiInformation, DisplayKpiInformationViewModel.KpiInformationViewModel kpiInformationTarget, string section)
        {
            try
            {
                int value;
                switch (section)
                {
                    case "ytd":
                        value = PlantAvailabilityIndicator(kpiInformation.DerItemValue.Ytd, kpiInformationTarget.DerItemValue.Ytd);
                        break;
                    case "mtd":
                        value = PlantAvailabilityIndicator(kpiInformation.DerItemValue.Mtd, kpiInformationTarget.DerItemValue.Ytd);
                        break;
                    default:
                        value = PlantAvailabilityIndicator(kpiInformation.DerItemValue.Value, kpiInformationTarget.DerItemValue.Ytd);
                        break;
                }
                //value = PlantAvailabilityIndicator(kpiInformation.DerItemValue.Ytd, kpiInformationTarget.DerItemValue.Ytd);
                return RemarkToMvcHtmlString(value.ToString());
            }
            catch (Exception e)
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        private static int PlantAvailabilityIndicator(string Value, string TargetVale)
        {
            int value;
            if (double.Parse(Value) >= double.Parse(TargetVale))
            {
                value = 1;
            }
            else if (double.Parse(Value) < (0.9 * double.Parse(TargetVale)))
            {
                value = -1;
            }
            else
            {
                value = 0;
            }
            return value;
        }

        public static MvcHtmlString DisplayPlantThermalIndicator(this HtmlHelper htmlHelper, DisplayKpiInformationViewModel.KpiInformationViewModel kpiInformation, DisplayKpiInformationViewModel.KpiInformationViewModel kpiInformationTarget, string section)
        {
            try
            {
                int value;
                switch (section)
                {
                    case "ytd":
                        value = PlantThermalIndicator(kpiInformation.DerItemValue.Ytd, kpiInformationTarget.DerItemValue.Ytd);
                        break;
                    case "mtd":
                        value = PlantThermalIndicator(kpiInformation.DerItemValue.Mtd, kpiInformationTarget.DerItemValue.Mtd);
                        break;
                    default:
                        value = PlantThermalIndicator(kpiInformation.DerItemValue.Value, kpiInformationTarget.DerItemValue.Value);
                        break;
                }
                return RemarkToMvcHtmlString(value.ToString());
            }
            catch (Exception e)
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        private static int PlantThermalIndicator(string Value, string TargetVale)
        {
            int value;
            if (double.Parse(Value) >= double.Parse(TargetVale))
            {
                value = 1;
            }
            else if (double.Parse(Value) < (0.95 * double.Parse(TargetVale)))
            {
                value = -1;
            }
            else
            {
                value = 0;
            }
            return value;
        }

        public static MvcHtmlString DisplayMaterialBalanceIndicator(this HtmlHelper htmlHelper, DisplayKpiInformationViewModel.KpiInformationViewModel kpiInformation, DisplayKpiInformationViewModel.KpiInformationViewModel kpiInformationTarget, string section)
        {
            try
            {
                int value;
                switch (section)
                {
                    case "ytd":
                        value = MaterialBalanceIndicator(kpiInformation.DerItemValue.Ytd, kpiInformationTarget.DerItemValue.Ytd);
                        break;
                    case "mtd":
                        value = MaterialBalanceIndicator(kpiInformation.DerItemValue.Mtd, kpiInformationTarget.DerItemValue.Mtd);
                        break;
                    default:
                        value = MaterialBalanceIndicator(kpiInformation.DerItemValue.Value, kpiInformationTarget.DerItemValue.Value);
                        break;
                }
                return RemarkToMvcHtmlString(value.ToString());
            }
            catch (Exception e)
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        private static int MaterialBalanceIndicator(string Value, string TargetVale)
        {
            int value;
            if (double.Parse(Value) < double.Parse(TargetVale))
            {
                value = 1;
            }
            else if (double.Parse(Value) > (1.05 * double.Parse(TargetVale)))
            {
                value = -1;
            }
            else
            {
                value = 0;
            }
            //else if ( >= double.Parse(TargetVale) && double.Parse(Value) < 2.2)
            //{
            //    value = 0;
            //}
            //else
            //{
            //    value = -1;
            //}
            return value;
        }

        public static MvcHtmlString DisplayDerRemarkJsonForLngAndCds(this HtmlHelper htmlHelper, string remarkJson, string type)
        {
            if (string.IsNullOrEmpty(remarkJson)) return new MvcHtmlString(string.Empty);
            try
            {
                //var jsonRemark = JsonConvert.DeserializeObject<JsonRemark>(remarkJson);

                switch (type.ToLowerInvariant())
                {
                    case "daily":
                    case "as of":
                        return RemarkToMvcHtmlStringForLngAndCds(remarkJson);
                    case "mtd":
                        return RemarkToMvcHtmlStringForLngAndCds(remarkJson);
                    case "ytd":
                        return RemarkToMvcHtmlStringForLngAndCds(remarkJson);
                    default:
                        return new MvcHtmlString(string.Empty);
                }
            }
            catch (JsonSerializationException exception)
            {
                return new MvcHtmlString(string.Empty);
            }
            catch (Exception exception)
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        public static string DisplayDerRemarkWithValue(this HtmlHelper htmlHelper, string deviation)
        {
            if (string.IsNullOrEmpty(deviation)) return string.Empty;
            switch (deviation.ToLowerInvariant())
            {
                case "4":
                case "fulfilled":
                    return "<span class='indicator left-side'><i class='fa fa-check' style='color:green'></i></span>Fulfilled";
                case "3":
                case "on-track":
                    return "<span class='indicator left-side'><i class='fa fa-circle' style='color:green'></i></span>On-track";
                case "2":
                case "loading":
                    return "<span class='indicator left-side'><i class='fa fa-arrow-right' style='color:grey'></i></span>Loading";
                case "1":
                case "need attention":
                    return "<span class='indicator left-side'><i class='fa fa-circle' style='color:orange'></i></span>Need attention";
                case "0":
                case "unfulfilled":
                    return "<span class='indicator left-side'><i class='fa fa-circle' style='color:red'></i></span>Unfulfilled";
                default:
                    return string.Empty;
            }
        }

        public static string DisplayDerRemarkForMarketTone(this HtmlHelper htmlHelper, string deviation)
        {
            if (string.IsNullOrEmpty(deviation)) return string.Empty;
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

        public static string DisplayDerValueWithLabelAtFront(this HtmlHelper htmlHelper, string measurement, string val, string defaultMeasurement, string defaultVal = "N/A", bool isRounded = true, int trailingDecimals = 2)
        {
            return !string.IsNullOrEmpty(val) ?
                string.Format("{1} {0}", RoundIt(isRounded, val, trailingDecimals), string.IsNullOrEmpty(measurement) ? defaultMeasurement : measurement) : defaultVal;
        }

        public static string Divide(this HtmlHelper htmlHelper, string val, int number)
        {
            if (string.IsNullOrEmpty(val)) return val;
            double x = double.Parse(val);
            return (x / number).ToString(CultureInfo.InvariantCulture);
        }

        public static string DisplayDerValueWithHours(this HtmlHelper htmlHelper, string val, string defaultVal = "N/A")
        {
            if (!string.IsNullOrEmpty(val))
            {
                var valInArray = val.Split('.');
                var hour = valInArray[0];
                var minutes = "00";
                if (valInArray.Count() == 2)
                {
                    minutes = valInArray[1].ToString().PadRight(2, '0');
                }
                return string.Format("{0}:{1}", hour, minutes);
            }

            return defaultVal;
        }

        public static string GetCssClassByDerValue(this HtmlHelper htmlHelper, string val, bool isCss = false)
        {
            if (!string.IsNullOrEmpty(val))
            {
                string x = val.Replace("<p>", "").Replace("</p>", "");
                return (isCss) ? x.Replace(' ', '-') : x;
            }

            return string.Empty;
        }
        private static string RoundIt(bool isRounded, string val, int number = 2, bool decimalAfterInt = true)
        {
            if (isRounded)
            {
                /*double v = double.Parse(val);
                val = Math.Round(v, 2).ToString(CultureInfo.InvariantCulture);*/
                int resultNumber;
                if (decimalAfterInt || !int.TryParse(val, out resultNumber))
                {
                    return ParseToNumber(val, number);
                }
                else {
                    return resultNumber.ToString();
                }
            }

            return val;
        }
        public static MvcHtmlString DisplaySafetyComplienceIndicator(this HtmlHelper htmlHelper, string valueYtd, string valueTarget)
        {
            if (!string.IsNullOrEmpty(valueYtd) && valueYtd == "no invtgtn")
            {
                return new MvcHtmlString("<span class='indicator absolute'><i class='fa fa-circle'></i></span>");
            }

            if(!string.IsNullOrEmpty(valueYtd) && !string.IsNullOrEmpty(valueTarget))
            {
                var ytd = double.Parse(valueYtd);
                var target = double.Parse(valueTarget);
                if (ytd == target)
                {
                    return new MvcHtmlString("<span class='indicator absolute'><i class='fa fa-circle'></i></span>");
                }
            }
           
           

            return new MvcHtmlString("<span class='indicator absolute'><i class='fa fa-circle' style='color:red'></i></span>");
        }

        public static MvcHtmlString DisplaySafetyIndicator(this HtmlHelper htmlHelper, string valueYtd, string valueTarget)
        {
            if (!string.IsNullOrEmpty(valueYtd) && valueYtd == "no invtgtn") {
                return new MvcHtmlString("<span class='indicator absolute'><i class='fa fa-circle'></i></span>");
            }

            if (string.IsNullOrEmpty(valueYtd) || string.IsNullOrEmpty(valueTarget))
            {
                return new MvcHtmlString(string.Empty);
            }
            var ytd = double.Parse(valueYtd);
            var target = double.Parse(valueTarget);
            if (ytd > target)
            {
                return new MvcHtmlString("<span class='indicator absolute'><i class='fa fa-circle' style='color:red'></i></span>");
            }
            else
            {
                return new MvcHtmlString("<span class='indicator absolute'><i class='fa fa-circle'></i></span>");
            }
        }

        public static MvcHtmlString DisplayKpiIndicatorType2(this HtmlHelper htmlHelper, string actualValue, string targetValue)
        {
            if (string.IsNullOrEmpty(actualValue) || string.IsNullOrEmpty(targetValue))
            {
                return new MvcHtmlString(string.Empty);
            }
            var actual = double.Parse(actualValue);
            var target = double.Parse(targetValue);
            if (actual < target)
            {
                //hijau
                return new MvcHtmlString("<span><i class='fa fa-circle' style='color: green'></i></span>");
            }
            else if (actual == target)
            {
                //kuning
                return new MvcHtmlString("<i class='fa fa-circle' style='color: orange'></i>");
            }
            else
            {
                //merah
                return new MvcHtmlString("<i class='fa fa-circle' style='color:red'></i>");
            }
        }

        public static MvcHtmlString DisplayKpiIndicatorType3(this HtmlHelper htmlHelper, string actualValue, string targetValue)
        {
            if (string.IsNullOrEmpty(actualValue) || string.IsNullOrEmpty(targetValue))
            {
                return new MvcHtmlString(string.Empty);
            }
            var actual = double.Parse(actualValue);
            var target = double.Parse(targetValue);
            if (actual > target)
            {
                //hijau
                return new MvcHtmlString("<i class='fa fa-circle'></i>");
            }
            else if (actual == target)
            {
                //kuning
                return new MvcHtmlString("<i class='fa fa-circle' style='color: orange'></i>");
            }
            else
            {
                //merah
                return new MvcHtmlString("<i class='fa fa-circle' style='color: red'></i>");
            }
        }

        public static MvcHtmlString DisplayKpiIndicatorType4(this HtmlHelper htmlHelper, string actualValue)
        {
            if (string.IsNullOrEmpty(actualValue))
            {
                return new MvcHtmlString(string.Empty);
            }
            var actual = double.Parse(actualValue);

            if (actual < 48) // 2 * 24
            {
                //hijau
                return new MvcHtmlString("<i class='fa fa-circle'></i>");
            }
            else if (actual > 52.8) // 2.2 * 24
            {
                //merah jika 
                return new MvcHtmlString("<i class='fa fa-circle' style='color:red'></i>");
            }
            else
            {
                //kuning
                return new MvcHtmlString("<i class='fa fa-circle' style='color: orange'></i>");
            }
        }

        public static MvcHtmlString DisplayKpiIndicatorType1(this HtmlHelper htmlHelper, string actualValue, string targetValue)
        {
            if (string.IsNullOrEmpty(actualValue) || string.IsNullOrEmpty(targetValue))
            {
                return new MvcHtmlString(string.Empty);
            }
            var actual = double.Parse(actualValue);
            var target = double.Parse(targetValue);
            if (actual < target)
            {
                //hijau
                return new MvcHtmlString("<i class='fa fa-circle'></i>");
            }
            else if (actual >= target)
            {
                //merah
                return new MvcHtmlString("<i class='fa fa-circle' style='color: red' ></i>");
            }
            else
            {
                return new MvcHtmlString(string.Empty);
            }
        }

        private static string ParseToNumber(string val, int number = 2)
        {
            double x;
            var styles = NumberStyles.AllowParentheses | NumberStyles.AllowTrailingSign | NumberStyles.Float | NumberStyles.AllowDecimalPoint;
            bool isValidDouble = Double.TryParse(val, styles, NumberFormatInfo.InvariantInfo, out x);


            //return isValidDouble ? Str x.ToString("0:0.###") : val;
            //return isValidDouble ? string.Format("{0:0,000.###}", x) : val;
            //if (number == 0)
            //{
            //    return isValidDouble ? Math.Round(x).ToString("N0", CultureInfo.InvariantCulture) : val;
            //}
            //else if (number == 1)
            //{
            //    return isValidDouble ? Math.Round(x, 1).ToString("N1", CultureInfo.InvariantCulture) : val;
            //}
            var format = string.Format("N{0}", number);
            return isValidDouble ? Math.Round(x, number).ToString(format, CultureInfo.InvariantCulture) : val; //string.Format("{0:#,##0.0##}", x) : val;
        }

        private static string RemarkToIcon(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            switch (s)
            {
                case "1":
                    return "fa-circle";
                case "-1":
                    return "fa-circle style='color:red'";
                case "0":
                    return "fa-circle style='color: orange'";
                default:
                    return string.Empty;
            }
        }

        private static MvcHtmlString RemarkToMvcHtmlString(string s)
        {
            if (string.IsNullOrEmpty(s)) return new MvcHtmlString(string.Empty);
            switch (s)
            {
                case "1":
                    return new MvcHtmlString("<span class='indicator absolute'><i class='fa fa-circle'></i></span>");
                case "-1":
                    return new MvcHtmlString("<span class='indicator absolute'><i class='fa fa-circle' style='color:red'></i></span>");
                case "0":
                    return new MvcHtmlString("<span class='indicator absolute'><i class='fa fa-circle' style='color:orange'></i></span>");
                default:
                    return new MvcHtmlString(s);
            }
        }

        private static MvcHtmlString RemarkToMvcHtmlStringForLngAndCds(string s)
        {
            if (string.IsNullOrEmpty(s)) return new MvcHtmlString(string.Empty);
            switch (s.ToLowerInvariant())
            {
                case "5":
                case "fulfilled":
                    return new MvcHtmlString("<span class='indicator left-side'><i class='fa fa-check' style='color:green'></i></span>Fulfilled");
                case "3":
                case "on track":
                    return new MvcHtmlString("<span class='indicator left-side'><i class='fa fa-circle' style='color:green'></i></span>On-track");
                case "4":
                case "loading":
                    return new MvcHtmlString("<span class='indicator left-side'><i class='fa fa-arrow-right' style='color:grey'></i></span>Loading");
                case "2":
                case "need attention":
                    return new MvcHtmlString("<span class='indicator left-side'><i class='fa fa-circle' style='color:orange'></i></span>Need attention");

                case "1":
                case "unfulfilled":
                    return new MvcHtmlString("<span class='indicator left-side'><i class='fa fa-circle' style='color:red'></i></span>Unfulfilled");

                default:
                    return new MvcHtmlString(string.Empty);
            }
        }

        public static MvcHtmlString DisplayKpiInformationInput(this HtmlHelper htmlHelper, IList<DerValuesViewModel.KpiInformationValuesViewModel> kpiInformations, int kpiId, int tabIndex, string placeholder = "rate", string defaultValueDefined = "empty", string type = "daily-actual", string valueType = "value", string additionalClass = "", string fieldLength = "", int maxlength = 0)
        {
            string value;
            string align = "right";
            if (valueType == "remark")
            {
                align = "left";
                if (string.IsNullOrEmpty(fieldLength))
                {
                    fieldLength = "300";
                }
            }
            if (string.IsNullOrEmpty(fieldLength))
            {
                fieldLength = "100";
            }
            if (!string.IsNullOrEmpty(additionalClass) && additionalClass.Equals("datepicker"))
            {
                fieldLength = "100";
            }
            string maxlengthProperty = string.Empty;
            if (maxlength > 0)
            {
                maxlengthProperty = string.Format("maxlength=\"{0}\"", maxlength.ToString());
            }
            switch (defaultValueDefined)
            {
                case "empty":
                case "prev":
                    value = "";
                    break;
                default:
                    value = defaultValueDefined;
                    break;
            }
            var kpiInformation = kpiInformations.First(x => x.KpiId == kpiId);
            var existValue = "empty";
            var id = 0;
            ValueObject valueObject;
            switch (type)
            {
                case "daily-actual-dafwc":
                case "daily-actual":
                    {
                        valueObject = GetValue(kpiInformation.DailyActual, value, defaultValueDefined, valueType, existValue);

                    }
                    break;
                case "monthly-actual-prev":
                case "monthly-actual-jcc":
                case "monthly-actual-bunker":
                case "monthly-actual":
                    {
                        valueObject = GetValue(kpiInformation.MonthlyActual, value, defaultValueDefined, valueType, existValue);
                    }
                    break;
                case "yearly-actual":
                    {
                        valueObject = GetValue(kpiInformation.YearlyActual, value, defaultValueDefined, valueType, existValue);
                    }
                    break;
                case "daily-target":
                    {
                        valueObject = GetValue(kpiInformation.DailyTarget, value, defaultValueDefined, valueType, existValue);
                    }
                    break;
                case "monthly-target":
                    {
                        valueObject = GetValue(kpiInformation.MonthlyTarget, value, defaultValueDefined, valueType, existValue);
                    }
                    break;
                default:
                    {
                        valueObject = GetValue(kpiInformation.YearlyTarget, value, defaultValueDefined, valueType, existValue);
                    }
                    break;

            }
            value = valueObject.Value;
            id = valueObject.ExistValue == "now" ? valueObject.Id : 0;
            existValue = valueObject.ExistValue;

            return new MvcHtmlString(string.Format("<input type=\"text\" value=\"{0}\" class=\"der-value-{1} form-control der-kpi {8}\"   placeholder=\"{2}\" tabindex=\"{3}\" data-type=\"{4}\" data-kpi-id=\"{5}\" data-id=\"{6}\" data-value-type=\"{7}\" style=\"width: {9}px; text-align: {10};\" {11}/>", value, existValue, placeholder, tabIndex, type, kpiId, id, valueType, additionalClass, fieldLength, align, maxlengthProperty));
        }

        public class ValueObject
        {
            public string Value { get; set; }
            public string ExistValue { get; set; }
            public int Id { get; set; }
        }

        private static ValueObject GetValue(DerValuesViewModel.KpiValueViewModel kpiValue, string value, string defaultValueDefined, string valueType, string existValue)
        {
            if (valueType == "value")
            {
                //var valtoString = string.Empty;
                //if (kpiValue != null) {valtoString= kpiValue.Value == 0 ? kpiValue.Value.ToString() : kpiValue.Value.ToString("#,#.#########"); }
                value = kpiValue == null ? value : (defaultValueDefined == "prev" ? (kpiValue.Value == 0 ? kpiValue.Value.ToString() : (kpiValue.Value.HasValue ? kpiValue.Value.Value.ToString("#,0.#########") : string.Empty)) : (kpiValue.Type == "now" ? (kpiValue.Value == 0 ? kpiValue.Value.ToString() : (kpiValue.Value.HasValue ? kpiValue.Value.Value.ToString("#,0.#########") : string.Empty)) : value));
                existValue = kpiValue == null ? existValue : kpiValue.Type;
            }
            else
            {
                value = kpiValue == null || kpiValue.Remark == null ? value : (defaultValueDefined == "prev" ? kpiValue.Remark.Replace("prev--","") : (kpiValue.Type == "now" ? kpiValue.Remark.Replace("prev--", "") : value));
                existValue = kpiValue == null ? existValue : (string.IsNullOrEmpty(kpiValue.Remark) || kpiValue.Remark.StartsWith("prev--") ? (!string.IsNullOrEmpty(kpiValue.Remark) && kpiValue.Remark.StartsWith("prev--")? "prev" : existValue) : kpiValue.Type);
            }
            return new ValueObject { Value = value, ExistValue = existValue, Id = kpiValue == null ? 0 : kpiValue.Id };
        }

        public static MvcHtmlString DisplayKpiInformationList(this HtmlHelper htmlHelper, IList<DerValuesViewModel.KpiInformationValuesViewModel> kpiInformations, int kpiId, int tabIndex, IList<SelectListItem> options, string defaultValueDefined = "empty", string type = "daily-actual")
        {
            string value;
            switch (defaultValueDefined)
            {
                case "empty":
                case "prev":
                    value = "";
                    break;
                default:
                    value = defaultValueDefined;
                    break;
            }
            var kpiInformation = kpiInformations.First(x => x.KpiId == kpiId);
            var existValue = "empty";
            var id = 0;
            switch (type)
            {
                case "daily-actual":
                    {
                        var valueObject = GetValue(kpiInformation.DailyActual, value, defaultValueDefined, "remark", existValue);
                        value = valueObject.Value;
                        id = valueObject.Id;
                        existValue = valueObject.ExistValue;
                    }
                    //value = kpiInformation.DailyActual == null ? value : (defaultValueDefined == "prev" ? kpiInformation.DailyActual.Remark : (kpiInformation.DailyActual.Type == "now" ? kpiInformation.DailyActual.Remark : value));
                    //existValue = kpiInformation.DailyActual == null ? existValue : kpiInformation.DailyActual.Type;
                    break;
                case "monthly-actual":
                    {
                        var valueObject = GetValue(kpiInformation.MonthlyActual, value, defaultValueDefined, "remark", existValue);
                        value = valueObject.Value;
                        id = valueObject.Id;
                        existValue = valueObject.ExistValue;
                    }
                    //value = kpiInformation.MonthlyActual == null ? value : (defaultValueDefined == "prev" ? kpiInformation.MonthlyActual.Remark : (kpiInformation.MonthlyActual.Type == "now" ? kpiInformation.MonthlyActual.Remark : value));
                    //existValue = kpiInformation.MonthlyActual == null ? existValue : kpiInformation.MonthlyActual.Type;
                    break;
                case "yearly-actual":
                    {
                        var valueObject = GetValue(kpiInformation.YearlyActual, value, defaultValueDefined, "remark", existValue);
                        value = valueObject.Value;
                        id = valueObject.Id;
                        existValue = valueObject.ExistValue;
                    }
                    //value = kpiInformation.YearlyActual == null ? value : (defaultValueDefined == "prev" ? kpiInformation.YearlyActual.Remark : (kpiInformation.YearlyActual.Type == "now" ? kpiInformation.YearlyActual.Remark : value));
                    //existValue = kpiInformation.YearlyActual == null ? existValue : kpiInformation.YearlyActual.Type;
                    break;
                case "daily-target":
                    {
                        var valueObject = GetValue(kpiInformation.DailyTarget, value, defaultValueDefined, "remark", existValue);
                        value = valueObject.Value;
                        id = valueObject.Id;
                        existValue = valueObject.ExistValue;
                    }
                    //value = kpiInformation.DailyTarget == null ? value : (defaultValueDefined == "prev" ? kpiInformation.DailyTarget.Remark : (kpiInformation.DailyTarget.Type == "now" ? kpiInformation.DailyTarget.Remark : value));
                    //existValue = kpiInformation.DailyTarget == null ? existValue : kpiInformation.DailyTarget.Type;
                    break;
                case "monthly-target":
                    {
                        var valueObject = GetValue(kpiInformation.MonthlyTarget, value, defaultValueDefined, "remark", existValue);
                        value = valueObject.Value;
                        id = valueObject.Id;
                        existValue = valueObject.ExistValue;
                    }
                    //value = kpiInformation.MonthlyTarget == null ? value : (defaultValueDefined == "prev" ? kpiInformation.MonthlyTarget.Remark : (kpiInformation.MonthlyTarget.Type == "now" ? kpiInformation.MonthlyTarget.Remark : value));
                    //existValue = kpiInformation.MonthlyTarget == null ? existValue : kpiInformation.MonthlyTarget.Type;
                    break;
                case "yearly-target":
                    {
                        var valueObject = GetValue(kpiInformation.YearlyTarget, value, defaultValueDefined, "remark", existValue);
                        value = valueObject.Value;
                        id = valueObject.Id;
                        existValue = valueObject.ExistValue;
                    }
                    //value = kpiInformation.YearlyTarget == null ? value : (defaultValueDefined == "prev" ? kpiInformation.YearlyTarget.Remark : (kpiInformation.YearlyTarget.Type == "now" ? kpiInformation.YearlyTarget.Remark : value));
                    //existValue = kpiInformation.YearlyTarget == null ? existValue : kpiInformation.YearlyTarget.Type;
                    break;

            }
            var selectInput = string.Format("<select class=\"der-value-{0} form-control der-kpi\" tabindex=\"{1}\" data-type=\"{2}\"  data-kpi-id=\"{3}\" data-id=\"{4}\" data-value-type=\"{5}\" >", existValue, tabIndex, type, kpiId, id, "remark");
            foreach (var option in options)
            {
                var selected = string.Equals(option.Value, value, StringComparison.InvariantCultureIgnoreCase) ? "selected=\"selected\"" : "";
                selectInput += string.Format("<option {2} value=\"{0}\">{1}</option>", option.Value, option.Text, selected);
            }
            selectInput += "</select>";
            return new MvcHtmlString(selectInput);

        }

        public static MvcHtmlString DisplayHighlightTextarea(this HtmlHelper htmlHelper, IList<DerValuesViewModel.DerHighlightValuesViewModel> highlights, int highlightTypeId, int tabIndex, string defaultValueDefined = "empty")
        {
            string value;
            switch (defaultValueDefined)
            {
                case "empty":
                case "prev":
                    value = "";
                    break;
                default:
                    value = defaultValueDefined;
                    break;
            }
            var existValue = "empty";
            var highlight = highlights.FirstOrDefault(x => x.HighlightTypeId == highlightTypeId);
            value = highlight == null ? value : (defaultValueDefined == "prev" ? highlight.HighlightMessage : (highlight.Type == "now" ? highlight.HighlightMessage : value));
            existValue = highlight == null ? existValue : highlight.Type;
            var highlightId = highlight == null ? 0 : highlight.Id;
            var title = highlight == null ? null : (string.IsNullOrEmpty(highlight.HighlightTitle) ? highlight.HighlightTypeValue : highlight.HighlightTitle);
            var textarea = string.Format("<textarea class=\"der-value-{0} form-control allow-html der-highlight\" data-highlight-type-id=\"{2}\" data-id=\"{3}\" id=\"highlight_{4}\" tabindex=\"{4}\" data-title=\"{5}\">{1}</textarea>", existValue, value, highlightTypeId, highlightId, tabIndex, title);
            return new MvcHtmlString(textarea);


        }
        public static MvcHtmlString DisplayHighlightDropdownList(this HtmlHelper htmlHelper, IList<DerValuesViewModel.DerHighlightValuesViewModel> highlights, int highlightTypeId, IList<SelectListItem> options, int tabIndex, string defaultValueDefined = "empty", int length = 100)
        {
            string value;
            switch (defaultValueDefined)
            {
                case "empty":
                case "prev":
                    value = "";
                    break;
                default:
                    value = defaultValueDefined;
                    break;
            }
            var len = string.Empty;
            if (length != 100)
            {
                len = string.Format("style=\"width: {0}px;\"", length.ToString());
            }
            var existValue = "empty";
            var highlight = highlights.FirstOrDefault(x => x.HighlightTypeId == highlightTypeId);
            value = highlight == null ? value : (defaultValueDefined == "prev" ? highlight.HighlightMessage : (highlight.Type == "now" ? highlight.HighlightMessage : value));
            existValue = highlight == null ? existValue : highlight.Type;
            var highlightId = highlight == null ? 0 : highlight.Id;
            var title = highlight == null ? null : (string.IsNullOrEmpty(highlight.HighlightTitle) ? highlight.HighlightTypeValue : highlight.HighlightTitle);
            var selectInput = string.Format("<select class=\"der-value-{0} form-control dropdown der-highlight\" tabindex=\"{1}\"  data-highlight-type-id=\"{2}\" data-id=\"{3}\" id=\"highlight_{4}\" data-title=\"{5}\" {6}>", existValue, tabIndex, highlightTypeId, highlightId, tabIndex, title, len);
            foreach (var option in options)
            {
                var selected = string.Equals(option.Value, value, StringComparison.InvariantCultureIgnoreCase) ? "selected=\"selected\"" : "";
                selectInput += string.Format("<option {2} value=\"{0}\">{1}</option>", option.Value, option.Text, selected);
            }
            selectInput += "</select>";
            return new MvcHtmlString(selectInput);
        }
        public static MvcHtmlString DisplayBrenfutHighlight(this HtmlHelper htmlHelper, IList<DerValuesViewModel.DerHighlightValuesViewModel> highlights, int highlightTypeId, int tabIndex, int position, string type = "value")
        {
            string value = "";
            var defaultValueDefined = "prev";
            string existValue = "empty";
            var highlight = highlights.FirstOrDefault(x => x.HighlightTypeId == highlightTypeId);
            value = highlight == null ? value : (defaultValueDefined == "prev" ? highlight.HighlightMessage : (highlight.Type == "now" ? highlight.HighlightMessage : value));
            existValue = highlight == null ? existValue : highlight.Type;
            var highlightId = highlight == null ? 0 : highlight.Id;
            var title = highlight == null ? null : (string.IsNullOrEmpty(highlight.HighlightTitle) ? highlight.HighlightTypeValue : highlight.HighlightTitle);
            JToken obj;
            var properties = new string[] { "a", "b", "c", "d" };
            if (IsValidJson(value, out obj))
            {
                value = obj[properties[position]][type].Value<string>();
            }
            else
            {
                value = "";
            }
            //if (value.Contains("<li>")) {
            //    //Regex regex = new Regex(@"<li.*?>(.*?)<\\/li>");
            //    MatchCollection matches = Regex.Matches(value, @"<li>(.*?)</li>");
            //    if (matches.Count > 0) {
            //        var splitResult = matches[position].Groups[1].Value.Split(':');
            //        if (type == "label")
            //        {
            //            value = splitResult[0];
            //        }
            //        else {
            //            var regex = new Regex("usd/bbl", RegexOptions.IgnoreCase);
            //            value = string.IsNullOrEmpty(splitResult[1]) ? splitResult[1] : regex.Replace(splitResult[1], "");
            //        }
            //    }

            //}
            if (type == "label")
            {
                return new MvcHtmlString(string.Format("<input type=\"text\" value=\"{0}\" class=\"der-value-{1} form-control der-highlight-brenfut\"   placeholder=\"{2}\" tabindex=\"{3}\" data-type=\"{4}\" data-property=\"{5}\" data-highlight-type-id=\"{6}\" data-id=\"{7}\" data-title=\"{8}\" />", value, existValue, "text period", tabIndex, type, properties[position], highlightTypeId, highlightId, title));
            }
            else
            {
                return new MvcHtmlString(string.Format("<input type=\"text\" value=\"{0}\" class=\"der-value-{1} form-control der-highlight-brenfut\"   placeholder=\"{2}\" tabindex=\"{3}\" data-type=\"{4}\" data-property=\"{5}\" data-highlight-type-id=\"{6}\" data-id=\"{7}\" data-title=\"{8}\" />", value, existValue, "USD/bbl", tabIndex, type, properties[position], highlightTypeId, highlightId, title));
            }

        }
        public static MvcHtmlString DisplayIctInfraOrGsfmHighlight(this HtmlHelper helper, IList<DerValuesViewModel.DerHighlightValuesViewModel> highlights, int highlightTypeId, IList<SelectListItem> options, int tabIndex, string property)
        {
            string value = "";
            var defaultValueDefined = "prev";
            string existValue = "empty";
            var highlight = highlights.FirstOrDefault(x => x.HighlightTypeId == highlightTypeId);
            value = highlight == null ? value : (defaultValueDefined == "prev" ? highlight.HighlightMessage : (highlight.Type == "now" ? highlight.HighlightMessage : value));
            existValue = highlight == null ? existValue : highlight.Type;
            var highlightId = highlight == null ? 0 : highlight.Id;
            var title = highlight == null ? null : (string.IsNullOrEmpty(highlight.HighlightTitle) ? highlight.HighlightTypeValue : highlight.HighlightTitle);
            JToken obj;
            if (!string.IsNullOrEmpty(value) && IsValidJson(value, out obj))
            {
                switch (property)
                {
                    case "a":
                        value = (string)obj.SelectToken("a");
                        break;
                    case "b":
                        value = (string)obj.SelectToken("b");
                        break;
                    default: //c
                        value = (string)obj.SelectToken("c");
                        break;
                        //default:
                        //    value = (string)obj.SelectToken("remark");
                        //    break;
                }
            }
            //if(property == "remark")
            //{
            //    return new MvcHtmlString(string.Format("<input type=\"text\" value=\"{0}\" class=\"der-value-{1} form-control\"   placeholder=\"{2}\" tabindex=\"{3}\" />", value, existValue, "text period", tabIndex));
            //}
            var selectInput = string.Format("<select class=\"der-value-{0} form-control der-highlight-infragsm\" tabindex=\"{1}\" data-property=\"{2}\" data-id=\"{3}\" data-highlight-type-id=\"{4}\" data-title=\"{5}\" >", existValue, tabIndex, property, highlightId, highlightTypeId, title);
            foreach (var option in options)
            {
                var selected = string.Equals(option.Value, value, StringComparison.InvariantCultureIgnoreCase) ? "selected=\"selected\"" : "";
                selectInput += string.Format("<option {2} value=\"{0}\">{1}</option>", option.Value, option.Text, selected);
            }
            selectInput += "</select>";
            return new MvcHtmlString(selectInput);
        }
        public static MvcHtmlString DisplayWeeklyAlarmInput(this HtmlHelper htmlHelper, IList<DerValuesViewModel.DerHighlightValuesViewModel> highlights, int highlightTypeId, int tabIndex, string label, string property)
        {
            string value = "";
            var defaultValueDefined = "prev";
            string existValue = "empty";
            var highlight = highlights.FirstOrDefault(x => x.HighlightTypeId == highlightTypeId);
            value = highlight == null ? value : (defaultValueDefined == "prev" ? highlight.HighlightMessage : (highlight.Type == "now" ? highlight.HighlightMessage : value));
            existValue = highlight == null ? existValue : highlight.Type;
            var highlightId = highlight == null ? 0 : highlight.Id;
            var title = highlight == null ? null : (string.IsNullOrEmpty(highlight.HighlightTitle) ? highlight.HighlightTypeValue : highlight.HighlightTitle);
            JToken obj;
            if (IsValidJson(value, out obj))
            {
                value = obj[property].Value<string>();
            }
            else
            {
                value = "";
            }
            //if (value.Contains("<li>")) {
            //    //Regex regex = new Regex(@"<li.*?>(.*?)<\\/li>");
            //    MatchCollection matches = Regex.Matches(value, @"<li>(.*?)</li>");
            //    if (matches.Count > 0) {
            //        var splitResult = matches[position].Groups[1].Value.Split(':');
            //        if (type == "label")
            //        {
            //            value = splitResult[0];
            //        }
            //        else {
            //            var regex = new Regex("usd/bbl", RegexOptions.IgnoreCase);
            //            value = string.IsNullOrEmpty(splitResult[1]) ? splitResult[1] : regex.Replace(splitResult[1], "");
            //        }
            //    }

            //}
            if (property == "remark")
            {
                return new MvcHtmlString(string.Format("<textarea type=\"text\" class=\"der-value-{1} form-control der-highlight-weekly-alarm\"   placeholder=\"{2}\" tabindex=\"{3}\" data-property=\"{4}\" data-highlight-type-id=\"{5}\" data-id=\"{6}\" data-title=\"{7}\">{0}</textarea>", value, existValue, label, tabIndex, property, highlightTypeId, highlightId, title));
            }
            return new MvcHtmlString(string.Format("<input type=\"text\" value=\"{0}\" class=\"der-value-{1} form-control der-highlight-weekly-alarm\"   placeholder=\"{2}\" tabindex=\"{3}\" data-property=\"{4}\" data-highlight-type-id=\"{5}\" data-id=\"{6}\" data-title=\"{7}\" />", value, existValue, label, tabIndex, property, highlightTypeId, highlightId, title));
        }
        public static MvcHtmlString DisplayHighlightInput(this HtmlHelper helper, IList<DerValuesViewModel.DerHighlightValuesViewModel> highlights, int highlightTypeId, int tabIndex, string placeHolder, string defaultValueDefined = "empty")
        {
            string value;
            switch (defaultValueDefined)
            {
                case "empty":
                case "prev":
                    value = "";
                    break;
                default:
                    value = defaultValueDefined;
                    break;
            }
            var existValue = "empty";
            var highlight = highlights.FirstOrDefault(x => x.HighlightTypeId == highlightTypeId);
            value = highlight == null ? value : (defaultValueDefined == "prev" ? highlight.HighlightMessage : (highlight.Type == "now" ? highlight.HighlightMessage : value));
            existValue = highlight == null ? existValue : highlight.Type;
            var id = highlight == null ? 0 : highlight.Id;
            var title = highlight == null ? "Der Highlight" : highlight.HighlightTitle;
            return new MvcHtmlString(string.Format("<input type=\"text\" value=\"{0}\" class=\"der-value-{1} form-control der-highlight-input\"   placeholder=\"{2}\" tabindex=\"{3}\" data-id=\"{4}\" data-highlight-type-id=\"{5}\" data-title=\"{6}\"  />", value, existValue, placeHolder, tabIndex, id, highlightTypeId, title));
        }
        public static MvcHtmlString DisplayWaveList(this HtmlHelper htmlHelper, WaveViewModel viewModel, IList<SelectListItem> options, string property, int tabIndex)
        {
            var value = "";
            var id = 0;
            var derValueType = "empty";
            if (viewModel != null)
            {
                if (property == "wind-direction")
                {
                    value = viewModel.ValueId.ToString();
                    derValueType = viewModel.WindDirectValueType;
                }
                else if (property == "tide")
                {
                    value = viewModel.Tide;
                    derValueType = viewModel.TideValueType;
                }
                else
                {
                    value = viewModel.Speed;
                    derValueType = viewModel.SpeedValueType;
                }
                id = viewModel.Id;
            }
            if (property == "speed")
            {
                return new MvcHtmlString(string.Format("<input value=\"{4}\" class=\"der-value-{0} form-control der-highlight-wave\" tabindex=\"{1}\" data-property=\"{2}\" data-id=\"{3}\" placeholder=\"Km/h\"  />", derValueType, tabIndex, property, id, value));
            }
            var selectInput = string.Format("<select class=\"der-value-{0} form-control der-highlight-wave\" tabindex=\"{1}\" data-property=\"{2}\" data-id=\"{3}\" >", derValueType, tabIndex, property, id);
            foreach (var option in options)
            {
                var selected = string.Equals(option.Value, value, StringComparison.InvariantCultureIgnoreCase) ? "selected=\"selected\"" : "";
                selectInput += string.Format("<option {2} value=\"{0}\">{1}</option>", option.Value, option.Text, selected);
            }
            selectInput += "</select>";
            return new MvcHtmlString(selectInput);
        }

        public static MvcHtmlString DisplayWeatherList(this HtmlHelper htmlHelper, WeatherViewModel viewModel, int tabIndex)
        {
            var value = "";
            var id = 0;
            var derValueType = "empty";
            if (viewModel != null)
            {
                value = viewModel.ValueId.ToString();
                id = viewModel.Id;
                derValueType = viewModel.DerValueType;
            }
            var selectInput = string.Format("<select class=\"der-value-{0} form-control der-highlight-weather\" tabindex=\"{1}\"  data-id=\"{2}\" >", derValueType, tabIndex, id);
            foreach (var option in viewModel.Values)
            {
                var selected = string.Equals(option.Value, value, StringComparison.InvariantCultureIgnoreCase) ? "selected=\"selected\"" : "";
                selectInput += string.Format("<option {2} value=\"{0}\">{1}</option>", option.Value, option.Text, selected);
            }
            selectInput += "</select>";
            return new MvcHtmlString(selectInput);
        }

        public static bool IsValidJson(string strInput, out JToken obj)
        {
            if (!string.IsNullOrEmpty(strInput))
            {
                strInput = strInput.Trim();
                if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                    (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
                {
                    try
                    {
                        obj = JObject.Parse(strInput);
                        return true;
                    }
                    catch (JsonReaderException jex)
                    {
                        obj = null;
                        return false;
                    }
                    catch (Exception ex) //some other exception
                    {
                        obj = null;
                        return false;
                    }
                }
                else
                {
                    obj = null;
                    return false;
                }
            }
            else
            {
                obj = null;
                return false;
            }
        }

        public static string GetPercentage(this HtmlHelper htmlHelper, string achievement, string target, int digit, bool isAskedForWidth = false)
        {
            if (string.IsNullOrEmpty(achievement) || string.IsNullOrEmpty(target))
            {
                return !isAskedForWidth ? "N/A" : "0";
            }
            else
            {
                var achievementToDouble = double.Parse(achievement);
                var targetToDouble = double.Parse(target);
                var percentage = Math.Round((achievementToDouble / targetToDouble) * 100, digit);
                return htmlHelper.DisplayCompleteDerValue(percentage.ToString(CultureInfo.InvariantCulture), "%", "%", "N/A", false, digit);
            }
        }

        public static string ToEngV(this HtmlHelper htmlHelper, string number, int digits, bool round)
        {
            var d = Double.Parse(number);
            var length = Math.Truncate(d).ToString().Length;
            int decimals = 0;

            if (length < digits)
            {
                decimals = digits - length;
                if (Math.Truncate(d) == d)
                {
                    decimals = 0;
                }
            }



            var format = string.Format("N{0}", decimals);

            if (round)
            {
                return Math.Round(d, decimals).ToString(format, CultureInfo.InvariantCulture);
            }
            else
            {
                int pow = (int)Math.Pow(10, decimals);
                return (Math.Truncate(d * pow) / pow).ToString(format, CultureInfo.InvariantCulture);
            }
        }

        private static bool ParseIt(string val, out double result)
        {
            double x;
            var styles = NumberStyles.AllowParentheses | NumberStyles.AllowTrailingSign | NumberStyles.Float | NumberStyles.AllowDecimalPoint;
            bool isValidDouble = Double.TryParse(val, styles, NumberFormatInfo.InvariantInfo, out x);
            result = (isValidDouble) ? Math.Round(x, 2) : 0;
            return isValidDouble;
        }
    }

    public class JsonRemark
    {
        public string Daily { get; set; }
        public string Mtd { get; set; }
        public string Ytd { get; set; }
    }
}