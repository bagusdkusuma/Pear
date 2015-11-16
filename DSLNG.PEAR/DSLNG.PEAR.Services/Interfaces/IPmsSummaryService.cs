using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.PmsSummary;
using DSLNG.PEAR.Services.Responses.PmsSummary;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IPmsSummaryService
    {
        CreatePmsSummaryResponse CreatePmsSummary(CreatePmsSummaryRequest request);
        UpdatePmsSummaryResponse UpdatePmsSummary(UpdatePmsSummaryRequest request);
        GetPmsSummaryReportResponse GetPmsSummaryReport(GetPmsSummaryReportRequest request);
        GetPmsSummaryResponse GetPmsSummary(int id);
        GetPmsSummaryListResponse GetPmsSummaryList(GetPmsSummaryListRequest request);
        GetPmsSummaryConfigurationResponse GetPmsSummaryConfiguration(GetPmsSummaryConfigurationRequest request);
        GetScoreIndicatorsResponse GetScoreIndicators(GetScoreIndicatorRequest request);
        GetPmsDetailsResponse GetPmsDetails(GetPmsDetailsRequest request);
        GetPmsConfigDetailsResponse GetPmsConfigDetails(int id);
        GetKpisByPillarIdResponse GetKpis(int pillarId);
        DeletePmsResponse DeletePmsSummary(int id);
        int GetYearActive();
        bool UpdateStatus(int id, bool isActive);

        #region PMS Config

        CreatePmsConfigResponse CreatePmsConfig(CreatePmsConfigRequest request);
        UpdatePmsConfigResponse UpdatePmsConfig(UpdatePmsConfigRequest request);
        GetPmsConfigResponse GetPmsConfig(int id);
        DeletePmsResponse DeletePmsConfig(int id);

        #endregion

        #region PmsConfigDetails

        CreatePmsConfigDetailsResponse CreatePmsConfigDetails(CreatePmsConfigDetailsRequest request);
        UpdatePmsConfigDetailsResponse UpdatePmsConfigDetails(UpdatePmsConfigDetailsRequest request);
        DeletePmsResponse DeletePmsConfigDetails(int id);

        #endregion


    }
}
