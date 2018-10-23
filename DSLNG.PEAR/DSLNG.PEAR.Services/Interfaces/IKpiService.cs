using System.Collections.Generic;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Services.Requests.Kpi;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.Kpi;


namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IKpiService
    {
        GetKpiResponse GetBy(GetKpiRequest request);
        GetKpiToSeriesResponse GetKpiToSeries(GetKpiToSeriesRequest request);

        GetKpiDetailResponse GetKpiDetail(GetKpiRequest request);
        GetKpiResponse GetKpi(GetKpiRequest request);
        GetKpisResponse GetKpis(GetKpisRequest request);
        CreateKpiResponse Create(CreateKpiRequest request);
        UpdateKpiResponse Update(UpdateKpiRequest request);
        DeleteKpiResponse Delete(int id);
        DeleteKpiResponse Delete(DeleteKpiRequest request);

        bool IsValidKpi(GetKpiByRole request);
        IList<Kpi> DownloadKpis();
        GetKpisResponse GetKpis(List<int> kpiIds);
    }
}
