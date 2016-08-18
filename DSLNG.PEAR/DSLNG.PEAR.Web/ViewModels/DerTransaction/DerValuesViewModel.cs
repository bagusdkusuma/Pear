
using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Web.ViewModels.DerTransaction
{
    public class DerValuesViewModel
    {
        public DerValuesViewModel() {
            Highlights = new List<DerHighlightValuesViewModel>();
            KpiInformations = new List<KpiInformationValuesViewModel>();
        }
        public IList<DerHighlightValuesViewModel> Highlights { get; set; }
        public IList<KpiInformationValuesViewModel> KpiInformations { get; set; }
        public class DerHighlightValuesViewModel
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public string Text { get; set; }
            public int HighlightTypeId { get; set; }
            public DateTime Date { get; set; }
            public string HighlightMessage { get; set; }
            public string HighlightTitle { get; set; }
        }

        public class KpiInformationValuesViewModel
        {
            public int Id { get; set; }
            public int KpiId { get; set; }
            public int Position { get; set; }
            public ConfigType ConfigType { get; set; }
            public KpiValueViewModel DailyTarget { get; set; }
            public KpiValueViewModel MonthlyTarget { get; set; }
            public KpiValueViewModel YearlyTarget { get; set; }

            public KpiValueViewModel DailyActual { get; set; }
            public KpiValueViewModel MonthlyActual { get; set; }
            public KpiValueViewModel YearlyActual { get; set; }
        }
        public class KpiValueViewModel
        {
            public DateTime Date { get; set; }
            public double Value { get; set; }
            public string Remark { get; set; }
        }
    }
}