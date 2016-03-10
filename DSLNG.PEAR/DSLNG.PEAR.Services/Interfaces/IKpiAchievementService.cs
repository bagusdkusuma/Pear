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
        GetKpiAchievementResponse GetKpiAchievementByValue(GetKpiAchievementRequestByValue request);
        BaseResponse BatchUpdateKpiAchievements(BatchUpdateKpiAchievementRequest request);
        GetKpiAchievementResponse GetKpiAchievement(int kpiId, DateTime date, RangeFilter rangeFilter);
        GetKpiAchievementResponse GetKpiAchievement(int kpiId, DateTime date, RangeFilter rangeFilter, YtdFormula ytdFormula);
    }

}
