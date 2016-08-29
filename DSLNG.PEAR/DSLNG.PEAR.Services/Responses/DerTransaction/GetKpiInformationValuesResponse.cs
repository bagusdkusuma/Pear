

using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.DerTransaction
{
    public class GetKpiInformationValuesResponse
    {
        public GetKpiInformationValuesResponse() {
            KpiInformations = new List<KpiInformation>();
        }
        public IList<KpiInformation> KpiInformations { get; set; }
        public class KpiInformation
        {
            public int KpiId { get; set; }
            public KpiValue DailyTarget { get; set; }
            public KpiValue MonthlyTarget { get; set; }
            public KpiValue YearlyTarget { get; set; }
          
            public KpiValue DailyActual { get; set; }
            public KpiValue MonthlyActual { get; set; }
            public KpiValue YearlyActual { get; set; }
        }

        public class KpiValue {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public double? Value { get; set; }
            public string Remark { get; set; }
            public string Type { get; set; }
        }
    }
}
