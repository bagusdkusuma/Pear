using System;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.KpiAchievement;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IKpiAchievementService
    {
        GetKpiAchievementsResponse GetKpiAchievements(GetKpiAchievementsRequest request);
        UpdateKpiAchievementsResponse UpdateKpiAchievements(UpdateKpiAchievementsRequest request);
        AllKpiAchievementsResponse GetAllKpiAchievements();
        AllKpiAchievementsResponse GetKpiAchievementsByRole(GetKpiAchievementsConfigurationRequest request);

        GetKpiAchievementsConfigurationResponse GetKpiAchievementsConfiguration(
            GetKpiAchievementsConfigurationRequest request);

        GetAchievementsResponse GetAchievements(GetKpiAchievementsConfigurationRequest request);
        UpdateKpiAchievementItemResponse UpdateKpiAchievementItem(UpdateKpiAchievementItemRequest request);
        UpdateKpiAchievementItemResponse UpdateKpiAchievementItem(int kpiId, PeriodeType periodeType, DateTime periode, double? value, int userId);
        GetKpiAchievementResponse GetKpiAchievementByValue(GetKpiAchievementRequestByValue request);
        BaseResponse BatchUpdateKpiAchievements(BatchUpdateKpiAchievementRequest request);
        GetKpiAchievementResponse GetKpiAchievement(int kpiId, DateTime date, RangeFilter rangeFilter);
        GetKpiAchievementResponse GetKpiAchievement(int kpiId, DateTime date, RangeFilter rangeFilter, YtdFormula ytdFormula);
        GetKpiAchievementResponse GetKpiAchievement(int kpiId, DateTime date, PeriodeType periodeType);
        GetKpiAchievementLessThanOrEqualResponse GetKpiAchievementLessThanOrEqual(int kpiId, DateTime date, PeriodeType periodeType);
        BaseResponse DeleteKpiAchievement(int kpiId, DateTime periode, PeriodeType periodeType);
        BaseResponse DeleteKpiAchievement(DeleteKpiAchievementRequest request);
        UpdateKpiAchievementItemResponse UpdateOriginalData(UpdateKpiAchievementItemRequest request, bool checkManualInput = false);
        UpdateKpiAchievementItemResponse UpdateCustomJccFormula(UpdateKpiAchievementItemRequest request);
        UpdateKpiAchievementItemResponse UpdateCustomBunkerPriceFormula(UpdateKpiAchievementItemRequest request);
        GetAchievementsResponse GetKpiAchievements(int[] kpiIds, DateTime? start, DateTime? end, string periodeType);
        GetAchievementsResponse GetKpiEconomics(int[] kpiIds, DateTime? start, DateTime? end, string periodeType);
    }

}
