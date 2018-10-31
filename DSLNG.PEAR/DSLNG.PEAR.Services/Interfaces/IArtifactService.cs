

using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Requests.Artifact;
using DSLNG.PEAR.Services.Responses.Artifact;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IArtifactService
    {
        //GetSeriesResponse GetSeries(GetSeriesRequest request);
        GetCartesianChartDataResponse GetChartData(GetCartesianChartDataRequest request, bool multiaxisAsOrigin = false);
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
        string[] GetPeriodes(PeriodeType periodeType, RangeFilter rangeFilter, DateTime? Start, DateTime? End, out IList<DateTime> dateTimePeriodes, out string timeInformation);
        IList<ExportSettingData> GetExportExcelData(Dictionary<string, List<string>> labelDictionaries, DateTime? start, DateTime? end, string periodeType);
        IList<ExportSettingData> GetExportExcelPieData(Dictionary<string, List<string>> labelDictionaries, RangeFilter rangeFilter, DateTime? requestStart, DateTime? requestEnd, PeriodeType periodeType, ArtifactValueInformation valueInformation, out IList<DateTime> dateTimePeriodes, out string timeInformation);
        IList<ExportSettingData> GetExportExcelTankData(Dictionary<string, List<string>> labelDictionaries, RangeFilter rangeFilter, DateTime? requestStart, DateTime? requestEnd, PeriodeType periodeType, ArtifactValueInformation valueInformation, out IList<DateTime> dateTimePeriodes, out string timeInformation);
    }
}
