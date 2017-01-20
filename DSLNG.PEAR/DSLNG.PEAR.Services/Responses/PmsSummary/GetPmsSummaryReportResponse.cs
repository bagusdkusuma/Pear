using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Constants;

namespace DSLNG.PEAR.Services.Responses.PmsSummary
{
    public class GetPmsSummaryReportResponse : BaseResponse
    {
        public GetPmsSummaryReportResponse()
        {
            KpiDatas = new List<KpiData>();
        }

        public IList<KpiData> KpiDatas { get; set; }
        public string Title { get; set; }

        public class KpiData
        {
            //public int Id { get; set; }
            public int PmsConfigId { get; set; }
            public int PmsConfigDetailId { get; set; }
            public int PmsSummaryId { get; set; }
            public int PillarId { get; set; }
            public string Pillar { get; set; }
            public string Kpi { get; set; }
            public string Unit { get; set; }
            public double Weight { get; set; }

            public double? TargetYearly { get; set; }
            public double? TargetMonthly { get; set; }
            public double? TargetYtd { get; set; }

            public double? ActualYearly { get; set; }
            public double? ActualMonthly { get; set; }
            public double? ActualYtd { get; set; }
            public ScoringType ScoringType { get; set; }
            public YtdFormula YtdFormula { get; set; }
            public string KpiIcon { get; set; }
            public string PillarIcon { get; set; }

            public double? IndexYearly
            {
                get
                {
                    if (ActualYearly.HasValue && TargetYearly.HasValue)
                    {
                        if (ScoringType == ScoringType.Boolean)
                        {
                            return ActualYearly.Value > 0 ? 0 : 1;
                        }
                        else
                        {
                            if (ActualYearly.Value.Equals(TargetYearly.Value))
                            {
                                return 1;
                            }

                            return ActualYearly / TargetYearly;
                        }
                        
                    }

                    return null;
                }
            }

            public double? IndexMonthly
            {
                get
                {
                    if (ActualMonthly.HasValue && TargetMonthly.HasValue)
                    {
                        if (ScoringType == ScoringType.Boolean)
                        {
                            return ActualMonthly.Value > 0 ? 0 : 1;
                        }
                        else
                        {
                            if (ActualMonthly.Value.Equals(TargetMonthly.Value))
                            {
                                return 1;
                            }

                            return ActualMonthly / TargetMonthly;
                        }

                    }


                    return null;
                }
            }

            public double? IndexYtd
            {
                get
                {
                    if (ActualYtd.HasValue && TargetYtd.HasValue)
                    {
                        if (ScoringType == ScoringType.Boolean)
                        {
                            return ActualYtd.Value > 0 ? 0 : 1;
                        }

                        if (ScoringType == ScoringType.Negative)
                        {
                            double? indexYtd = /*ActualYtd.Value.Equals(TargetYtd.Value) ? 1 :*/ ActualYtd/TargetYtd;
                            return indexYtd < PmsSummaryConstant.NegativeValue ? PmsSummaryConstant.NegativeValue : indexYtd;
                        }

                        return /*ActualYtd.Value.Equals(TargetYtd.Value) ? 1 : */ActualYtd/TargetYtd;
                    }

                    return null;
                }
            }

            public double? Score { get; set; }
            public double? YtdScore { get; set; }
            public int PillarOrder { get; set; }

            public string PillarColor { get; set; }

            public double PillarWeight { get; set; }

            public string PmsConfigColor { get; set; }

            public int KpiOrder { get; set; }

            public string KpiColor { get; set; }

            public string TotalScoreColor { get; set; }

            public string ScoreIndicators { get; set; }

            //public string Color { get; set; }

            

            /*public string TargetYearly { get; set; }
            public string TargetMonthly { get; set; }
            public string TargetYtd { get; set; }

            public string ActualYearly { get; set; }
            public string ActualMonthly { get; set; }
            public string ActualYtd { get; set; }

            public string IndexYearly { get; set; }
            public string IndexMonthly { get; set; }
            public string IndexYtd { get; set; }*/
        }
    }
}
