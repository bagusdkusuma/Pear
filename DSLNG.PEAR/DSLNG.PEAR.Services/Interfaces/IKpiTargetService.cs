using System;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Requests.KpiTarget;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.KpiTarget;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IKpiTargetService
    {
        CreateKpiTargetResponse Create(CreateKpiTargetRequest request);
        CreateKpiTargetsResponse Creates(CreateKpiTargetsRequest request);
        GetPmsConfigsResponse GetPmsConfigs(GetPmsConfigsRequest request);
        GetKpiTargetsResponse GetKpiTargets(GetKpiTargetsRequest request);
        GetKpiTargetResponse GetKpiTarget(GetKpiTargetRequest request);
        GetKpiTargetItemResponse GetKpiTargetByValue(GetKpiTargetRequestByValue request);
        UpdateKpiTargetResponse UpdateKpiTarget(UpdateKpiTargetRequest request);
        UpdateKpiTargetItemResponse UpdateKpiTargetItem(UpdateKpiTargetItemRequest request);

        UpdateKpiTargetItemResponse SaveKpiTargetItem(SaveKpiTargetRequest request);
        GetKpiTargetsConfigurationResponse GetKpiTargetsConfiguration(GetKpiTargetsConfigurationRequest request);
        AllKpiTargetsResponse GetAllKpiTargets();
        AllKpiTargetsResponse GetAllKpiTargetByRole(GetKpiTargetsConfigurationRequest request);
        BaseResponse BatchUpdateKpiTargetss(BatchUpdateTargetRequest request);

        GetKpiTargetItemResponse GetKpiTarget(int kpiId, DateTime date, RangeFilter rangeFilter);
        GetKpiTargetItemResponse GetKpiTarget(int kpiId, DateTime date, PeriodeType periodeType);
        UpdateKpiTargetItemResponse UpdateOriginalData(SaveKpiTargetRequest request);
        GetKpiTargetsResponse GetKpiTargets(int[] kpiIds, DateTime? start, DateTime? end, string periodeType);
    }
}
