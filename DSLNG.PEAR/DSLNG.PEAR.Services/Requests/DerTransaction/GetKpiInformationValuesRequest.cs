

using DSLNG.PEAR.Services.Responses.DerTransaction;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Requests.DerTransaction
{
    public class GetKpiInformationValuesRequest
    {
        public GetKpiInformationValuesRequest() {
            ActualKpiIds = new int[] { };
            TargetKpiIds = new int[] { };
        }
        public DateTime Date { get; set; }
        public IEnumerable<int> ActualKpiIds { get; set; }
        public IEnumerable<int> TargetKpiIds { get; set; }
        //public IList<GetDerLayoutItemsResponse.KpiInformation> KpiInformations { get; set; }
    }
}
