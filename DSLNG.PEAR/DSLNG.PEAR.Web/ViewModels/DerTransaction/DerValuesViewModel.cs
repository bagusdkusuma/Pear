
using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Web.ViewModels.DerTransaction
{
    public class DerValuesViewModel
    {
        public IList<DerLayoutItemViewModel> DerLayoutItems { get; set; }
        public class DerLayoutItemViewModel
        {
            public int Id { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
            public string Type { get; set; }
            public DerHighlightValuesViewModel Highlight { get; set; }
            public IList<KpiInformationValuesViewModel> KpiInformations { get; set; }
        }

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