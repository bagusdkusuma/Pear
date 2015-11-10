

using DSLNG.PEAR.Services.Requests.Artifact;
using DSLNG.PEAR.Services.Responses.Artifact;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IArtifactService
    {
        //GetSeriesResponse GetSeries(GetSeriesRequest request);
        GetCartesianChartDataResponse GetChartData(GetCartesianChartDataRequest request);
        GetSpeedometerChartDataResponse GetSpeedometerChartData(GetSpeedometerChartDataRequest request);
        GetTrafficLightChartDataResponse GetTrafficLightChartData(GetTrafficLightChartDataRequest request);
        CreateArtifactResponse Create(CreateArtifactRequest request);
        UpdateArtifactResponse Update(UpdateArtifactRequest request);
        GetArtifactsResponse GetArtifacts(GetArtifactsRequest request);
        GetArtifactResponse GetArtifact(GetArtifactRequest request);
        GetArtifactsToSelectResponse GetArtifactsToSelect(GetArtifactsToSelectRequest request);
        GetTabularDataResponse GetTabularData(GetTabularDataRequest request);
        GetTankDataResponse GetTankData(GetTankDataRequest request);
        GetMultiaxisChartDataResponse GetMultiaxisChartData(GetMultiaxisChartDataRequest request);
        GetPieDataResponse GetPieData(GetPieDataRequest request);
        GetComboChartDataResponse GetComboChartData(GetComboChartDataRequest request);
        DeleteArtifactResponse Delete(DeleteArtifactRequest request);
    }
}
