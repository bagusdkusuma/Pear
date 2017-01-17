using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DSLNG.PEAR.Common.Contants;

namespace DSLNG.PEAR.Web.ViewModels.PmsSummary
{
    public class PmsSummaryViewModel
    {
        public int PmsConfigId { get; set; }
        public int PmsConfigDetailId { get; set; }
        public int PmsSummaryId { get; set; }
        public string Pillar { get; set; }
        public string Kpi { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; }

        public double? TargetYearly { get; set; }
        public double? TargetMonthly { get; set; }
        public double? TargetYtd { get; set; }

        public double? ActualYearly { get; set; }
        public double? ActualMonthly { get; set; }
        public double? ActualYtd { get; set; }

        public double? IndexYearly { get; set; }
        public double? IndexMonthly { get; set; }
        public double? IndexYtd { get; set; }

        public double? Score { get; set; }
        public double? YtdScore { get; set; }
        public string TargetActualYearly
        {
            get
            {
                return string.Format(@"{1} / {0}", TargetYearly.HasValue ? TargetYearly.Value.ToString(FormatNumber.DecimalFormat) : "-", ActualYearly.HasValue ? ActualYearly.Value.ToString(FormatNumber.DecimalFormat) : "-");
            }
        }

        public string TargetActualMonthly
        {
            get
            {
                return string.Format(@"{1} / {0}", TargetMonthly.HasValue ? TargetMonthly.Value.ToString(FormatNumber.DecimalFormat) : "-", ActualMonthly.HasValue ? ActualMonthly.Value.ToString(FormatNumber.DecimalFormat) : "-");
            }
        }

        public string TargetActualYtd
        {
            get
            {
                return string.Format(@"{1} / {0}", TargetYtd.HasValue ? TargetYtd.Value.ToString(FormatNumber.DecimalFormat) : "-", ActualYtd.HasValue ? ActualYtd.Value.ToString(FormatNumber.DecimalFormat) : "-");
            }
        }

        public string IndexYearlyStr
        {
            get
            {
                return string.Format(@"{0}", IndexYearly.HasValue ? IndexYearly.Value.ToString(FormatNumber.DecimalFormat) : "-");
            }
        }

        public string IndexMonthlyStr
        {
            get
            {
                return string.Format(@"{0}", IndexMonthly.HasValue ? IndexMonthly.Value.ToString(FormatNumber.DecimalFormat) : "-");
            }
        }

        public string IndexYtdStr
        {
            get
            {
                return string.Format(@"{0}", IndexYtd.HasValue ? IndexYtd.Value.ToString(FormatNumber.DecimalFormat) : "-");
            }
        }

        public string ScoreStr
        {
            get
            {
                return string.Format(@"{0} / {1}", Score.HasValue ? Score.Value.ToString(FormatNumber.DecimalFormat) : "-", Weight);
            }
        }

        public int PillarOrder { get; set; }

        public int KpiOrder { get; set; }

        public string KpiColor { get; set; }

        public string PillarColor { get; set; }

        public double PillarWeight { get; set; }

        public string KpiName
        {
            get { return string.Format(@"{0} ({1})", Kpi, Unit); }
        }

        public string KpiNameWithColor
        {
            get
            {
                UrlHelper u = new UrlHelper(HttpContext.Current.Request.RequestContext);
                string url = u.Action("ScoreIndicator", "PmsSummary", new {id = this.PmsConfigDetailId});
                return string.Format(@"<span class='trafficlight popover-kpi' data-poload={2} style='background-color:{0}'></span>{1}", KpiColor, KpiName, url);
            }
        }

        public string PillarNameWithColor
        {
            get { return string.Format(@"<span class='trafficlight popover-pillar' style='background-color:{0}'></span>{1}", PillarColor, string.Format(@"{0} ({1})", Pillar, PillarWeight.ToString("0"))); }
        }

        public string TotalScoreColor { get; set; }

        public string ScoreIndicators { get; set; }

        public string KpiIcon { get; set; }
        public string PillarIcon { get; set; }
        public string PmsConfigColor { get; set; }
    }
}