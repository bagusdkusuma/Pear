using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using DSLNG.PEAR.Services.Responses.KpiAchievement;
using DSLNG.PEAR.Services.Responses;

namespace DSLNG.PEAR.Services
{
    public class KpiAchievementService : BaseService, IKpiAchievementService
    {
        public KpiAchievementService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public GetKpiAchievementsResponse GetKpiAchievements(GetKpiAchievementsRequest request)
        {
            var response = new GetKpiAchievementsResponse();
            try
            {
                var pmsSummary = DataContext.PmsSummaries.Single(x => x.Id == request.PmsSummaryId);
                response.Year = pmsSummary.Year;
                var pillarsAndKpis = DataContext.PmsConfigDetails
                        .Include(x => x.Kpi)
                        .Include(x => x.Kpi.KpiAchievements)
                        .Include(x => x.Kpi.Measurement)
                        .Include(x => x.PmsConfig)
                        .Include(x => x.PmsConfig.PmsSummary)
                        .Include(x => x.PmsConfig.Pillar)
                        .Where(x => x.PmsConfig.PmsSummary.Id == request.PmsSummaryId)
                        .ToList()
                        .GroupBy(x => x.PmsConfig.Pillar)
                        .ToDictionary(x => x.Key);


                foreach (var item in pillarsAndKpis)
                {
                    var pillar = new GetKpiAchievementsResponse.Pillar();
                    pillar.Id = item.Key.Id;
                    pillar.Name = item.Key.Name;

                    foreach (var val in item.Value)
                    {
                        var achievements = new List<GetKpiAchievementsResponse.KpiAchievement>();
                        switch (request.PeriodeType)
                        {
                            case PeriodeType.Monthly:
                                for (int i = 1; i <= 12; i++)
                                {
                                    var kpiAchievementsMonthly = val.Kpi.KpiAchievements.FirstOrDefault(x => x.PeriodeType == PeriodeType.Monthly
                                                    && x.Periode.Month == i && x.Periode.Year == pmsSummary.Year);
                                    var kpiAchievementMonthly = new GetKpiAchievementsResponse.KpiAchievement();
                                    if (kpiAchievementsMonthly == null)
                                    {
                                        kpiAchievementMonthly.Id = 0;
                                        kpiAchievementMonthly.Periode = new DateTime(pmsSummary.Year, i, 1);
                                        kpiAchievementMonthly.Value = null;
                                        kpiAchievementMonthly.Remark = null;
                                    }
                                    else
                                    {
                                        kpiAchievementMonthly.Id = kpiAchievementsMonthly.Id;
                                        kpiAchievementMonthly.Periode = kpiAchievementsMonthly.Periode;
                                        kpiAchievementMonthly.Value = kpiAchievementsMonthly.Value;
                                        kpiAchievementMonthly.Remark = kpiAchievementsMonthly.Remark;
                                    }

                                    achievements.Add(kpiAchievementMonthly);
                                }
                                break;
                            case PeriodeType.Yearly:
                                var kpiAchievementsYearly =
                                    val.Kpi.KpiAchievements.FirstOrDefault(x => x.PeriodeType == PeriodeType.Yearly
                                                                           && x.Periode.Year == pmsSummary.Year);
                                var kpiAchievementYearly = new GetKpiAchievementsResponse.KpiAchievement();
                                if (kpiAchievementsYearly == null)
                                {
                                    kpiAchievementYearly.Id = 0;
                                    kpiAchievementYearly.Periode = new DateTime(pmsSummary.Year, 1, 1);
                                    kpiAchievementYearly.Value = null;
                                    kpiAchievementYearly.Remark = null;
                                }
                                else
                                {
                                    kpiAchievementYearly.Id = kpiAchievementsYearly.Id;
                                    kpiAchievementYearly.Periode = kpiAchievementsYearly.Periode;
                                    kpiAchievementYearly.Value = kpiAchievementsYearly.Value;
                                    kpiAchievementYearly.Remark = kpiAchievementsYearly.Remark;
                                }
                                achievements.Add(kpiAchievementYearly);

                                break;
                        }

                        var kpi = new GetKpiAchievementsResponse.Kpi
                        {
                            Id = val.Kpi.Id,
                            Measurement = val.Kpi.Measurement.Name,
                            Name = val.Kpi.Name,
                            KpiAchievements = achievements
                        };

                        pillar.Kpis.Add(kpi);
                    }

                    response.Pillars.Add(pillar);
                }
                response.IsSuccess = true;
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.Message = argumentNullException.Message;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
            }

            return response;
        }

        public UpdateKpiAchievementsResponse UpdateKpiAchievements(UpdateKpiAchievementsRequest request)
        {
            PeriodeType periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), request.PeriodeType);
            var response = new UpdateKpiAchievementsResponse();
            response.PeriodeType = periodeType;

            try
            {
                foreach (var pillar in request.Pillars)
                {
                    foreach (var kpi in pillar.Kpis)
                    {
                        foreach (var kpiAchievement in kpi.KpiAchievements)
                        {
                            if (kpiAchievement.Id == 0)
                            {
                                var kpiAchievementNew = new KpiAchievement() { Value = kpiAchievement.Value, Kpi = DataContext.Kpis.Single(x => x.Id == kpi.Id), PeriodeType = periodeType, Periode = kpiAchievement.Periode, IsActive = true, Remark = kpiAchievement.Remark, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
                                DataContext.KpiAchievements.Add(kpiAchievementNew);
                            }
                            else
                            {
                                var kpiAchievementNew = new KpiAchievement() { Id = kpiAchievement.Id, Value = kpiAchievement.Value, Kpi = DataContext.Kpis.Single(x => x.Id == kpi.Id), PeriodeType = periodeType, Periode = kpiAchievement.Periode, IsActive = true, Remark = kpiAchievement.Remark, UpdatedDate = DateTime.Now };
                                DataContext.KpiAchievements.Attach(kpiAchievementNew);
                                DataContext.Entry(kpiAchievementNew).State = EntityState.Modified;
                            }
                        }
                    }
                }
                response.IsSuccess = true;
                response.Message = "KPI Achievements has been updated successfully";
                DataContext.SaveChanges();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
            }

            return response;
        }


        public AllKpiAchievementsResponse GetAllKpiAchievements()
        {
            var response = new AllKpiAchievementsResponse();
            try
            {
                var kpiAchievements = DataContext.Kpis
                    .Include(x => x.Measurement)
                    .Include(x => x.Type)
                    .Include(x => x.RoleGroup)
                    .AsEnumerable()
                    .OrderBy(x => x.Order)
                    .GroupBy(x => x.RoleGroup).ToDictionary(x => x.Key);

                foreach (var item in kpiAchievements)
                {
                    var kpis = new List<AllKpiAchievementsResponse.Kpi>();
                    foreach (var val in item.Value)
                    {
                        kpis.Add(val.MapTo<AllKpiAchievementsResponse.Kpi>());
                    }

                    response.RoleGroups.Add(new AllKpiAchievementsResponse.RoleGroup
                    {
                        Id = item.Key.Id,
                        Name = item.Key.Name,
                        Kpis = kpis
                    });
                }

                response.IsSuccess = true;
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.Message = argumentNullException.Message;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
            }

            return response;
        }

        public GetKpiAchievementsConfigurationResponse GetKpiAchievementsConfiguration(GetKpiAchievementsConfigurationRequest request)
        {
            var response = new GetKpiAchievementsConfigurationResponse();
            try
            {
                var periodeType = string.IsNullOrEmpty(request.PeriodeType)
                                      ? PeriodeType.Yearly
                                      : (PeriodeType)Enum.Parse(typeof(PeriodeType), request.PeriodeType);

                var kpis = DataContext.Kpis
                                      .Include(x => x.RoleGroup)
                                      .Include(x => x.Measurement).ToList();

                if (request.RoleGroupId > 0)
                {
                    kpis = kpis.Where(x => x.RoleGroup.Id == request.RoleGroupId).ToList();
                    var roleGroup = DataContext.RoleGroups.Single(x => x.Id == request.RoleGroupId);
                    response.RoleGroupName = roleGroup.Name;
                    response.RoleGroupId = roleGroup.Id;
                    response.IsSuccess = true;
                }
                //var kpis = DataContext.Kpis
                //                      .Include(x => x.RoleGroup)
                //                      .Include(x => x.Measurement)
                //                      .Where(x => x.RoleGroup.Id == request.RoleGroupId).ToList();



                switch (periodeType)
                {
                    case PeriodeType.Yearly:
                        var kpiAchievementsYearly = DataContext.KpiAchievements
                                        .Include(x => x.Kpi)
                                        .OrderBy(x => x.Kpi.Order)
                                        .Where(x => x.PeriodeType == periodeType).ToList();
                        foreach (var kpi in kpis)
                        {
                            var kpiDto = kpi.MapTo<GetKpiAchievementsConfigurationResponse.Kpi>();
                            foreach (var number in YearlyNumbers)
                            {
                                var achievement = kpiAchievementsYearly.FirstOrDefault(x => x.Kpi.Id == kpi.Id && x.Periode.Year == number);
                                if (achievement != null)
                                {
                                    var achievementDto =
                                        achievement.MapTo<GetKpiAchievementsConfigurationResponse.KpiAchievement>();
                                    kpiDto.KpiAchievements.Add(achievementDto);
                                }
                                else
                                {
                                    var achievementDto = new GetKpiAchievementsConfigurationResponse.KpiAchievement();
                                    achievementDto.Periode = new DateTime(number, 1, 1);
                                    kpiDto.KpiAchievements.Add(achievementDto);
                                }
                            }


                            response.Kpis.Add(kpiDto);
                        }
                        break;

                    case PeriodeType.Monthly:
                        var kpiAchievementsMonthly = DataContext.KpiAchievements
                                        .Include(x => x.Kpi)
                                        .Where(x => x.PeriodeType == periodeType && x.Periode.Year == request.Year).ToList();
                        foreach (var kpi in kpis)
                        {
                            var kpiDto = kpi.MapTo<GetKpiAchievementsConfigurationResponse.Kpi>();
                            var achievements = kpiAchievementsMonthly.Where(x => x.Kpi.Id == kpi.Id).ToList();

                            for (int i = 1; i <= 12; i++)
                            {
                                var achievement = achievements.FirstOrDefault(x => x.Periode.Month == i);
                                if (achievement != null)
                                {
                                    var achievementDto =
                                        achievement.MapTo<GetKpiAchievementsConfigurationResponse.KpiAchievement>();
                                    kpiDto.KpiAchievements.Add(achievementDto);
                                }
                                else
                                {
                                    var achievementDto = new GetKpiAchievementsConfigurationResponse.KpiAchievement();
                                    achievementDto.Periode = new DateTime(request.Year, i, 1);
                                    kpiDto.KpiAchievements.Add(achievementDto);
                                }
                            }
                            response.Kpis.Add(kpiDto);
                        }
                        break;

                    case PeriodeType.Daily:
                        var kpiAchievementsDaily = DataContext.KpiAchievements
                                        .Include(x => x.Kpi)
                                        .Where(x => x.PeriodeType == periodeType && x.Periode.Year == request.Year
                                        && x.Periode.Month == request.Month).ToList();
                        foreach (var kpi in kpis)
                        {
                            var kpiDto = kpi.MapTo<GetKpiAchievementsConfigurationResponse.Kpi>();
                            var achievements = kpiAchievementsDaily.Where(x => x.Kpi.Id == kpi.Id).ToList();
                            for (int i = 1; i <= DateTime.DaysInMonth(request.Year, request.Month); i++)
                            {
                                var achievement = achievements.FirstOrDefault(x => x.Periode.Day == i);
                                if (achievement != null)
                                {
                                    var achievementDto =
                                        achievement.MapTo<GetKpiAchievementsConfigurationResponse.KpiAchievement>();
                                    kpiDto.KpiAchievements.Add(achievementDto);
                                }
                                else
                                {
                                    var achievementDto = new GetKpiAchievementsConfigurationResponse.KpiAchievement();
                                    achievementDto.Periode = new DateTime(request.Year, request.Month, i);
                                    kpiDto.KpiAchievements.Add(achievementDto);
                                }
                            }
                            response.Kpis.Add(kpiDto);
                        }
                        break;
                }


            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.Message = argumentNullException.Message;
            }

            return response;
        }

        public UpdateKpiAchievementItemResponse UpdateKpiAchievementItem(UpdateKpiAchievementItemRequest request)
        {
            var response = new UpdateKpiAchievementItemResponse();
            try
            {
                var user = DataContext.Users.First(x => x.Id == request.UserId);
                var kpiAchievement = request.MapTo<KpiAchievement>();

                if (request.Id > 0)
                {
                    if (string.IsNullOrEmpty(request.Value) || request.Value == "-" || request.Value.ToLowerInvariant() == "null")
                    {
                        kpiAchievement = DataContext.KpiAchievements.Single(x => x.Id == request.Id);
                        DataContext.KpiAchievements.Remove(kpiAchievement);
                    }
                    else
                    {
                        kpiAchievement = DataContext.KpiAchievements
                                                .Include(x => x.Kpi)
                                                .Include(x => x.UpdatedBy)
                                                .Single(x => x.Id == request.Id);
                        request.MapPropertiesToInstance<KpiAchievement>(kpiAchievement);
                        kpiAchievement.UpdatedBy = user;
                        kpiAchievement.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                    }
                }
                else if (request.Id == 0)
                {
                    if ((string.IsNullOrEmpty(request.Value) || request.Value == "-" ||
                         request.Value.ToLowerInvariant() == "null") && request.Id == 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "You can not update this item because it is not existed";
                        return response;
                    }
                    else
                    {
                        kpiAchievement.CreatedBy = user;
                        kpiAchievement.UpdatedBy = user;
                        kpiAchievement.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                        DataContext.KpiAchievements.Add(kpiAchievement);
                    }
                }


                DataContext.SaveChanges();
                response.Id = request.Id > 0 ? request.Id : kpiAchievement.Id;
                response.IsSuccess = true;
                response.Message = "KPI Achievement item has been updated successfully";
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.Message = argumentNullException.Message;
            }

            return response;
        }

        public GetKpiAchievementResponse GetKpiAchievementByValue(GetKpiAchievementRequestByValue request)
        {
            PeriodeType periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), request.PeriodeType);
            var response = new GetKpiAchievementResponse();
            response.PeriodeType = periodeType;
            try
            {
                var kpiAchievement = DataContext.KpiAchievements.Include(x => x.Kpi).Single(x => x.Kpi.Id == request.KpiId && x.PeriodeType == periodeType && x.Periode == request.Periode);
                response = kpiAchievement.MapTo<GetKpiAchievementResponse>();
                response.IsSuccess = true;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.IsSuccess = false;
                response.Message = invalidOperationException.Message;
                response.ExceptionType = typeof(InvalidOperationException);
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.IsSuccess = false;
                response.Message = argumentNullException.Message;
            }
            return response;
        }


        public GetAchievementsResponse GetAchievements(GetKpiAchievementsConfigurationRequest request)
        {
            PeriodeType periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), request.PeriodeType);
            var response = new GetAchievementsResponse();
            try
            {
                var kpiAchievement = DataContext.KpiAchievements.Include(x => x.Kpi).OrderBy(x => x.Kpi.Order).Where(x => x.PeriodeType == periodeType && x.Periode.Year == request.Year && x.Periode.Month == request.Month).ToList();
                if (kpiAchievement.Count > 0)
                {
                    foreach (var item in kpiAchievement)
                    {
                        response.KpiAchievements.Add(item.MapTo<GetKpiAchievementResponse>());
                    }
                }
                response.IsSuccess = true;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.IsSuccess = false;
                response.Message = invalidOperationException.Message;
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.IsSuccess = false;
                response.Message = argumentNullException.Message;
            }
            return response;
        }


        //public BaseResponse BatchUpdateKpiAchievements(BatchUpdateKpiAchievementRequest request)
        //{
        //    var response = new BaseResponse();
        //    try
        //    {
        //        int i = 0;
        //        foreach (var item in request.BatchUpdateKpiAchievementItemRequest)
        //        {
        //            var kpiAchievement = item.MapTo<KpiAchievement>();
        //            var exist = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == item.KpiId && x.PeriodeType == item.PeriodeType && x.Periode == item.Periode && x.Value == item.RealValue && x.Remark == item.Remark);
        //            //skip no change value
        //            if (exist != null)
        //            {
        //                continue;
        //            }
        //            var attachedEntity = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == item.KpiId && x.PeriodeType == item.PeriodeType && x.Periode == item.Periode);
        //            if (attachedEntity != null)
        //            {
        //                kpiAchievement.Id = attachedEntity.Id;
        //            }
        //            //jika tidak ada perubahan di skip aja
        //            //if (existing.Value.Equals(item.Value) && existing.Periode.Equals(item.Periode) && existing.Kpi.Id.Equals(item.KpiId) && existing.PeriodeType.Equals(item.PeriodeType)) {
        //            //    break;
        //            //}
        //            if (kpiAchievement.Id != 0)
        //            {
        //                //var attachedEntity = DataContext.KpiAchievements.Find(item.Id);
        //                if (attachedEntity != null && DataContext.Entry(attachedEntity).State != EntityState.Detached)
        //                {
        //                    DataContext.Entry(attachedEntity).State = EntityState.Detached;
        //                }
        //                DataContext.KpiAchievements.Attach(kpiAchievement);
        //                DataContext.Entry(kpiAchievement).State = EntityState.Modified;
        //            }
        //            else
        //            {
        //                kpiAchievement.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == item.KpiId);
        //                DataContext.KpiAchievements.Add(kpiAchievement);
        //            }
        //            i++;
        //        }
        //        DataContext.SaveChanges();
        //        response.IsSuccess = true;
        //        if (i > 0)
        //        {
        //            response.Message = string.Format("{0}  KPI Achievement items has been updated successfully", i.ToString());
        //        }
        //        else
        //        {
        //            response.Message = "File Successfully Parsed, but no data changed!";
        //        }


        //    }
        //    catch (InvalidOperationException invalidOperationException)
        //    {
        //        response.Message = invalidOperationException.Message;
        //    }
        //    catch (ArgumentNullException argumentNullException)
        //    {
        //        response.Message = argumentNullException.Message;
        //    }
        //    return response;
        //}

        public BaseResponse BatchUpdateKpiAchievements(BatchUpdateKpiAchievementRequest request)
        {
            var response = new BaseResponse();
            try
            {
                int deletedCounter = 0;
                int updatedCounter = 0;
                int addedCounter = 0;
                int skippedCounter = 0;
                foreach (var item in request.BatchUpdateKpiAchievementItemRequest)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        var existedKpiAchievement =
                            DataContext.KpiAchievements.FirstOrDefault(
                                x =>
                                x.Kpi.Id == item.KpiId && x.PeriodeType == item.PeriodeType && x.Periode == item.Periode);


                        if (existedKpiAchievement != null)
                        {
                            if (item.Value.Equals("-") || item.Value.ToLowerInvariant().Equals("null"))
                            {
                                DataContext.KpiAchievements.Remove(existedKpiAchievement);
                                deletedCounter++;
                            }
                            else
                            {
                                if (existedKpiAchievement.Value.Equals(item.RealValue))
                                {
                                    skippedCounter++;
                                }
                                else
                                {
                                    existedKpiAchievement.Value = item.RealValue;
                                    DataContext.Entry(existedKpiAchievement).State = EntityState.Modified;
                                    updatedCounter++;
                                }
                            }
                        }
                        else
                        {
                            var kpiAchievement = item.MapTo<KpiAchievement>();
                            if (kpiAchievement.Value.HasValue)
                            {
                                kpiAchievement.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == item.KpiId);
                                DataContext.KpiAchievements.Add(kpiAchievement);
                                addedCounter++;
                            }
                        }
                    }
                }
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = string.Format("{0} data has been added, {1} data has been updated, {2} data has been removed, {3} data didn't change", addedCounter.ToString()
                   , updatedCounter.ToString(), deletedCounter.ToString(), skippedCounter.ToString());
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.Message = argumentNullException.Message;
            }
            return response;
        }

        public GetKpiAchievementResponse GetKpiAchievement(int kpiId, DateTime date, RangeFilter rangeFilter)
        {
            var response = new GetKpiAchievementResponse();
            try
            {
                switch (rangeFilter)
                {
                    case RangeFilter.CurrentDay:
                    case RangeFilter.MTD:
                    case RangeFilter.YTD:
                    case RangeFilter.AllExistingYears:
                    case RangeFilter.CurrentWeek:
                        {
                            var kpi = DataContext.Kpis
                                .Include(x => x.Measurement)
                                .Single(x => x.Id == kpiId);

                            return GetKpiAchievement(kpi.Id, date, rangeFilter, kpi.YtdFormula);
                        }
                }

            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }


            return response;
        }

        public GetKpiAchievementResponse GetKpiAchievement(int kpiId, DateTime date, RangeFilter rangeFilter, YtdFormula ytdFormula)
        {
            var response = new GetKpiAchievementResponse();
            try
            {
                switch (rangeFilter)
                {
                    case RangeFilter.CurrentDay:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiAchievements.Include(x => x.Kpi).FirstOrDefault(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Daily && x.Periode == date);
                            var kpiResponse = new GetKpiAchievementResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiAchievementResponse
                            {
                                Value = (data != null) ? data.Value : null,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }

                    case RangeFilter.MTD:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiAchievements.Include(x => x.Kpi)
                                .Where(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Daily &&
                                    (x.Periode.Day >= 1 && x.Periode.Day <= date.Day && x.Periode.Month == date.Month && x.Periode.Year == date.Year)).AsQueryable();
                            double? kpiAchievement = ytdFormula == YtdFormula.Average ? data.Average(x => x.Value) : data.Sum(x => x.Value);
                            var kpiResponse = new GetKpiAchievementResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiAchievementResponse
                            {
                                Value = kpiAchievement,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }

                    case RangeFilter.YTD:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiAchievements.Include(x => x.Kpi)
                                    .Where(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Monthly && x.Value.HasValue &&
                                    (x.Periode.Month >= 1 && x.Periode.Month <= date.Month && x.Periode.Year == date.Year)).AsQueryable();
                            double? kpiAchievement = ytdFormula == YtdFormula.Average ? data.Average(x => x.Value) : data.Sum(x => x.Value);
                            var kpiResponse = new GetKpiAchievementResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiAchievementResponse
                            {
                                Value = kpiAchievement,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }

                    case RangeFilter.AllExistingYears:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiAchievements.Include(x => x.Kpi)
                                    .Where(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Yearly && x.Value.HasValue &&
                                    (x.Periode.Year >= 2011 && x.Periode.Year <= date.Year)).AsQueryable();
                            double? kpiAchievement = ytdFormula == YtdFormula.Average ? data.Average(x => x.Value) : data.Sum(x => x.Value);
                            var kpiResponse = new GetKpiAchievementResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiAchievementResponse
                            {
                                Value = kpiAchievement,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }
                    case RangeFilter.CurrentWeek:
                        {
                            DateTime lastWednesday = date;
                            while (lastWednesday.DayOfWeek != DayOfWeek.Wednesday)
                                lastWednesday = lastWednesday.AddDays(-1);
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiAchievements.Include(x => x.Kpi)
                                    .Where(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Daily && x.Value.HasValue &&
                                        ((x.Periode.Year == lastWednesday.Year && x.Periode.Month == lastWednesday.Month && x.Periode.Day >= lastWednesday.Day) &&
                                        (x.Periode.Year == date.Year && x.Periode.Month == date.Month && x.Periode.Day <= date.Day))).AsQueryable();

                            double? kpiAchievement = ytdFormula == YtdFormula.Average ? data.Average(x => x.Value) : data.Sum(x => x.Value);
                            var kpiResponse = new GetKpiAchievementResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiAchievementResponse
                            {
                                Value = kpiAchievement,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }
                }

            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }


            return response;
        }

        public GetKpiAchievementResponse GetKpiAchievement(int kpiId, DateTime date, PeriodeType periodeType)
        {
            var response = new GetKpiAchievementResponse();
            try
            {
                var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                var data = DataContext.KpiAchievements.Include(x => x.Kpi).Where(x => x.Kpi.Id == kpiId && x.Periode == date).AsQueryable();
                var result = new KpiAchievement();
                switch (periodeType)
                {
                    case PeriodeType.Daily:
                    {
                        result = data.FirstOrDefault(x => x.PeriodeType == periodeType);
                        break;
                    }
                }

                var kpiResponse = new GetKpiAchievementResponse.KpiResponse
                {
                    Id = kpi.Id,
                    Measurement = kpi.Measurement.Name,
                    Name = kpi.Name,
                    Remark = kpi.Remark,
                };

                return new GetKpiAchievementResponse
                {
                    Value = (result != null) ? result.Value : null,
                    Mtd = (result != null) ? result.Mtd : null,
                    Ytd = (result != null) ? result.Ytd : null,
                    Itd = (result != null) ? result.Itd : null,
                    Remark = (result != null) ? result.Remark : null,
                    Kpi = kpiResponse,
                    Deviation = (result != null) ? result.Deviation : null,
                    IsSuccess = true
                };
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }


            return response;
        }

        public BaseResponse DeleteKpiAchievement(int kpiId, DateTime periode, PeriodeType periodeType)
        {
            var response = new BaseResponse();
            try
            {
                var achievements = DataContext.KpiAchievements.Where(
                x => x.Kpi.Id == kpiId && x.Periode == periode && x.PeriodeType == periodeType).ToList();
                foreach (var achievement in achievements)
                {
                    DataContext.KpiAchievements.Remove(achievement);
                }
                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
                response.ExceptionType = typeof(InvalidOperationException);
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.Message = argumentNullException.Message;
                response.ExceptionType = typeof(ArgumentNullException);
            }

            return response;
        }


        public AllKpiAchievementsResponse GetKpiAchievementsByRole(GetKpiAchievementsConfigurationRequest request)
        {
            var response = new AllKpiAchievementsResponse();
            try
            {
                var kpiAchievements = DataContext.Kpis
                    .Include(x => x.Measurement)
                    .Include(x => x.Type)
                    .Include(x => x.RoleGroup)
                    .Where(x => x.RoleGroup.Id == request.RoleGroupId)
                    .AsEnumerable()
                    .OrderBy(x => x.Order)
                    .GroupBy(x => x.RoleGroup).ToDictionary(x => x.Key);

                foreach (var item in kpiAchievements)
                {
                    var kpis = new List<AllKpiAchievementsResponse.Kpi>();
                    foreach (var val in item.Value)
                    {
                        kpis.Add(val.MapTo<AllKpiAchievementsResponse.Kpi>());
                    }

                    response.RoleGroups.Add(new AllKpiAchievementsResponse.RoleGroup
                    {
                        Id = item.Key.Id,
                        Name = item.Key.Name,
                        Kpis = kpis
                    });
                }

                response.IsSuccess = true;
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.Message = argumentNullException.Message;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
            }

            return response;
        }

        public UpdateKpiAchievementItemResponse UpdateOriginalData(UpdateKpiAchievementItemRequest request)
        {
            var response = new UpdateKpiAchievementItemResponse();
            try
            {
                var user = DataContext.Users.First(x => x.Id == request.UserId);
                var kpiAchievement = request.MapTo<KpiAchievement>();

                if (request.Id > 0)
                {
                    if ((string.IsNullOrEmpty(request.Value) && request.Remark == null) || request.Value == "-" || request.Value.ToLowerInvariant() == "null")
                    {
                        kpiAchievement = DataContext.KpiAchievements.Single(x => x.Id == request.Id);
                        DataContext.KpiAchievements.Remove(kpiAchievement);
                    }
                    else
                    {
                        kpiAchievement = DataContext.KpiAchievements
                                                .Include(x => x.Kpi)
                                                .Include(x => x.UpdatedBy)
                                                .Single(x => x.Id == request.Id);
                        if (request.Value != null) {
                            kpiAchievement.Value = request.RealValue;
                        }
                        if (request.Remark != null) {
                            kpiAchievement.Remark = request.Remark;
                        }
                        //request.MapPropertiesToInstance<KpiAchievement>(kpiAchievement);
                        kpiAchievement.UpdatedBy = user;
                        kpiAchievement.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                    }
                }
                else if (request.Id == 0)
                {
                    if (((string.IsNullOrEmpty(request.Value) && request.Remark == null) || request.Value == "-" ||
                         request.Value.ToLowerInvariant() == "null") && request.Id == 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "You can not update this item because it is not existed";
                        return response;
                    }
                    else
                    {
                        kpiAchievement.CreatedBy = user;
                        kpiAchievement.UpdatedBy = user;
                        kpiAchievement.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                        DataContext.KpiAchievements.Add(kpiAchievement);
                    }
                }


                DataContext.SaveChanges();

                if (request.Remark == null) {
                    switch (request.PeriodeType) {
                        case PeriodeType.Yearly:
                            if (kpiAchievement.Kpi.YtdFormula == YtdFormula.Sum)
                            {
                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Yearly
                                  && x.Periode.Year <= request.Periode.Year
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);
                                kpiAchievement.Itd = itdValue;

                                DataContext.SaveChanges();
                            }
                            else
                            {
                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Yearly
                                  && x.Periode.Year <= request.Periode.Year
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);

                                kpiAchievement.Itd = itdValue;

                                DataContext.SaveChanges();
                            }

                            break;
                        case PeriodeType.Monthly:
                            if (kpiAchievement.Kpi.YtdFormula == YtdFormula.Sum)
                            {
                                var ytdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Monthly
                            && x.Periode.Year == request.Periode.Year
                            && x.Periode.Month <= request.Periode.Month
                            && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Month == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (yearly != null)
                                {
                                    yearly.Value = ytdValue;
                                }
                                else
                                {
                                    yearly = new KpiAchievement
                                    {
                                        Value = ytdValue,
                                        Periode = new DateTime(request.Periode.Year, 1, 1),
                                        PeriodeType = PeriodeType.Yearly,
                                        Kpi = kpiAchievement.Kpi,
                                        CreatedBy = user,
                                        UpdatedBy = user
                                    };
                                    DataContext.KpiAchievements.Add(yearly);
                                }
                                DataContext.SaveChanges();

                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Yearly
                                  && x.Periode.Year <= request.Periode.Year
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);

                                yearly.Itd = itdValue;

                                kpiAchievement.Ytd = ytdValue;
                                kpiAchievement.Itd = itdValue;

                                DataContext.SaveChanges();
                            }
                            else {
                                var ytdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Monthly
                            && x.Periode.Year == request.Periode.Year
                            && x.Periode.Month <= request.Periode.Month
                            && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Month == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (yearly != null)
                                {
                                    yearly.Value = ytdValue;
                                }
                                else
                                {
                                    yearly = new KpiAchievement
                                    {
                                        Value = ytdValue,
                                        Periode = new DateTime(request.Periode.Year, 1, 1),
                                        PeriodeType = PeriodeType.Yearly,
                                        Kpi = kpiAchievement.Kpi,
                                        CreatedBy = user,
                                        UpdatedBy = user
                                    };
                                    DataContext.KpiAchievements.Add(yearly);
                                }
                                DataContext.SaveChanges();

                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Yearly
                                  && x.Periode.Year <= request.Periode.Year
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);

                                yearly.Itd = itdValue;

                                kpiAchievement.Ytd = ytdValue;
                                kpiAchievement.Itd = itdValue;

                                DataContext.SaveChanges();
                            }
                                break;
                        default:
                            if (kpiAchievement.Kpi.YtdFormula == YtdFormula.Sum) {
                                var mtdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Daily
                                && x.Periode.Year == request.Periode.Year && x.Periode.Month == request.Periode.Month
                                && x.Periode <= request.Periode
                                && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);
                                kpiAchievement.Mtd = mtdValue;
                                var monthly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Month == request.Periode.Month && x.PeriodeType == PeriodeType.Monthly);
                                if (monthly != null)
                                {
                                    monthly.Value = mtdValue;
                                }
                                else {
                                    monthly = new KpiAchievement
                                    {
                                        Value = mtdValue,
                                        Periode = new DateTime(request.Periode.Year, request.Periode.Month, 1),
                                        PeriodeType = PeriodeType.Monthly,
                                        Kpi = kpiAchievement.Kpi,
                                        CreatedBy = user,
                                        UpdatedBy = user
                                    };
                                    DataContext.KpiAchievements.Add(monthly);
                                }
                                DataContext.SaveChanges();

                                var ytdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Monthly
                                && x.Periode.Year == request.Periode.Year
                                && x.Periode.Month <= request.Periode.Month
                                && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Month == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (yearly != null)
                                {
                                    yearly.Value = ytdValue;
                                }
                                else
                                {
                                    yearly = new KpiAchievement
                                    {
                                        Value = ytdValue,
                                        Periode = new DateTime(request.Periode.Year, 1, 1),
                                        PeriodeType = PeriodeType.Yearly,
                                        Kpi = kpiAchievement.Kpi,
                                        CreatedBy = user,
                                        UpdatedBy = user
                                    };
                                    DataContext.KpiAchievements.Add(yearly);
                                }
                                DataContext.SaveChanges();

                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Yearly
                                  && x.Periode.Year <= request.Periode.Year
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);

                                yearly.Itd = itdValue;

                                monthly.Ytd = ytdValue;
                                monthly.Itd = itdValue;

                                kpiAchievement.Mtd = mtdValue;
                                kpiAchievement.Ytd = ytdValue;
                                kpiAchievement.Itd = itdValue;

                                DataContext.SaveChanges();

                            } else {
                                var mtdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Daily
                               && x.Periode.Year == request.Periode.Year && x.Periode.Month == request.Periode.Month
                               && x.Periode <= request.Periode
                               && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);
                                kpiAchievement.Mtd = mtdValue;
                                var monthly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Month == request.Periode.Month && x.PeriodeType == PeriodeType.Monthly);
                                if (monthly != null)
                                {
                                    monthly.Value = mtdValue;
                                }
                                else
                                {
                                    monthly = new KpiAchievement
                                    {
                                        Value = mtdValue,
                                        Periode = new DateTime(request.Periode.Year, request.Periode.Month, 1),
                                        PeriodeType = PeriodeType.Monthly,
                                        Kpi = kpiAchievement.Kpi,
                                        CreatedBy = user,
                                        UpdatedBy = user
                                    };
                                    DataContext.KpiAchievements.Add(monthly);
                                }
                                DataContext.SaveChanges();

                                var ytdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Monthly
                                && x.Periode.Year == request.Periode.Year
                                && x.Periode.Month <= request.Periode.Month
                                && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Month == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (yearly != null)
                                {
                                    yearly.Value = ytdValue;
                                }
                                else
                                {
                                    yearly = new KpiAchievement
                                    {
                                        Value = ytdValue,
                                        Periode = new DateTime(request.Periode.Year, 1, 1),
                                        PeriodeType = PeriodeType.Yearly,
                                        Kpi = kpiAchievement.Kpi,
                                        CreatedBy = user,
                                        UpdatedBy = user
                                    };
                                    DataContext.KpiAchievements.Add(yearly);
                                }
                                DataContext.SaveChanges();

                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Yearly
                                  && x.Periode.Year <= request.Periode.Year
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);

                                yearly.Itd = itdValue;

                                monthly.Ytd = ytdValue;
                                monthly.Itd = itdValue;

                                kpiAchievement.Mtd = mtdValue;
                                kpiAchievement.Ytd = ytdValue;
                                kpiAchievement.Itd = itdValue;

                                DataContext.SaveChanges();
                            }
                             
                            break;
                    }
                }
              
                response.Id = request.Id > 0 ? request.Id : kpiAchievement.Id;
                response.IsSuccess = true;
                response.Message = "KPI Achievement item has been updated successfully";
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
            }
            catch (ArgumentNullException argumentNullException)
            {
                response.Message = argumentNullException.Message;
            }

            return response;
        }
    }
}
