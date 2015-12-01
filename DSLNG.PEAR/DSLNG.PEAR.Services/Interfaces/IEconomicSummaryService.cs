using DSLNG.PEAR.Services.Requests.EconomicSummary;
using DSLNG.PEAR.Services.Responses.EconomicSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IEconomicSummaryService
    {
        GetEconomicSummariesResponse GetEconomicSummaries(GetEconomicSummariesRequest request);
        GetEconomicSummariesResponse GetEconomicSummariesForGrid(GetEconomicSummariesRequest request);
        SaveEconomicSummaryResponse SaveEconomicSummary(SaveEconomicSummaryRequest request);
        GetEconomicSummaryResponse GetEconomicSummary(GetEconomicSummaryRequest request);
        DeleteEconomicSummaryResponse DeleteEconomicSummary(DeleteEconomicSummaryRequest request);
        GetEconomicSummaryReportResponse GetEconomicSummaryReport();
        void UpdateEconomicSummary();
    }
}
