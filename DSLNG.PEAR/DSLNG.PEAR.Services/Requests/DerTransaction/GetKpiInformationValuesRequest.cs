

using DSLNG.PEAR.Services.Responses.DerTransaction;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Requests.DerTransaction
{
    public class GetKpiInformationValuesRequest
    {
        public DateTime Date { get; set; }
        public IList<GetDerLayoutItemsResponse.KpiInformation> KpiInformations { get; set; }
    }
}
