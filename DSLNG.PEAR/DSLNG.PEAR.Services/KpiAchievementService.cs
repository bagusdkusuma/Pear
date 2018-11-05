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
using DSLNG.PEAR.Services.Requests.CustomFormula;
using DSLNG.PEAR.Services.Requests;

namespace DSLNG.PEAR.Services
{
    public class KpiAchievementService : BaseService, IKpiAchievementService
    {
        private readonly ICustomFormulaService _customService;

        public KpiAchievementService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public KpiAchievementService(IDataContext dataContext, ICustomFormulaService customService) : base(dataContext)
        {
            _customService = customService;
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

        public UpdateKpiAchievementItemResponse UpdateKpiAchievementItem(int kpiId, PeriodeType periodeType, DateTime periode, double? value, int userId)
        {
            var user = DataContext.Users.First(x => x.Id == userId);
            var kpiAchievement = DataContext.KpiAchievements.FirstOrDefault(x => x.PeriodeType == periodeType && x.Periode == periode && x.Kpi.Id == kpiId);
            if (kpiAchievement == null)
            {
                var kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiId);
                if (kpi == null)
                {
                    DataContext.Kpis.Attach(kpi);
                }
                kpiAchievement = new KpiAchievement { Periode = periode, PeriodeType = periodeType, Value = value, CreatedBy = user, UpdatedBy = user };
                kpiAchievement.Kpi = kpi;
                DataContext.KpiAchievements.Add(kpiAchievement);
            }
            else
            {
                kpiAchievement.Value = value;
            }
            DataContext.SaveChanges();
            var response = new UpdateKpiAchievementItemResponse() { Id = kpiAchievement.Id, IsSuccess = true, Message = "KPI Achievement item has been updated successfully" };
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
                var action = request.MapTo<BaseAction>();
                var user = DataContext.Users.First(x => x.Id == request.UserId);
                var kpiAchievement = request.MapTo<KpiAchievement>();
                DateTime prevDate;
                KpiAchievement prevAchievement = null;
                if (request.UpdateDeviation)
                {
                    switch (request.PeriodeType)
                    {
                        case PeriodeType.Yearly:
                            prevDate = request.Periode.AddYears(-1);
                            break;
                        case PeriodeType.Monthly:
                            prevDate = request.Periode.AddMonths(-1);
                            break;
                        default:
                            prevDate = request.Periode.AddDays(-1);
                            break;
                    }
                    prevAchievement = DataContext.KpiAchievements.FirstOrDefault(x => x.Periode == prevDate && x.PeriodeType == request.PeriodeType && x.Kpi.Id == request.KpiId);

                }

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
                        //request.MapPropertiesToInstance<KpiAchievement>(kpiAchievement);
                        if (request.UpdateFrom != null && request.UpdateFrom.Equals("KPIAchievementForm"))
                        {
                            kpiAchievement.Value = request.RealValue;
                            kpiAchievement.Remark = request.Remark;
                        }
                        else
                        {
                            request.MapPropertiesToInstance<KpiAchievement>(kpiAchievement);
                        }
                        kpiAchievement.UpdatedBy = user;
                        kpiAchievement.UpdatedDate = DateTime.Now;
                        kpiAchievement.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                        if (request.UpdateDeviation)
                        {
                            if (prevAchievement != null)
                            {
                                kpiAchievement.Deviation = CompareKpiValue(prevAchievement.Value, kpiAchievement.Value);
                                kpiAchievement.MtdDeviation = CompareKpiValue(prevAchievement.Mtd, kpiAchievement.Mtd);
                                kpiAchievement.YtdDeviation = CompareKpiValue(prevAchievement.Ytd, kpiAchievement.Ytd);
                                kpiAchievement.ItdDeviation = CompareKpiValue(prevAchievement.Itd, kpiAchievement.Itd);
                            }
                            else
                            {
                                kpiAchievement.Deviation = "1";
                                kpiAchievement.MtdDeviation = "1";
                                kpiAchievement.YtdDeviation = "1";
                                kpiAchievement.ItdDeviation = "1";
                            }
                        }
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
                        if (request.UpdateDeviation)
                        {
                            if (prevAchievement != null)
                            {
                                kpiAchievement.Deviation = CompareKpiValue(prevAchievement.Value, kpiAchievement.Value);
                                kpiAchievement.MtdDeviation = CompareKpiValue(prevAchievement.Mtd, kpiAchievement.Mtd);
                                kpiAchievement.YtdDeviation = CompareKpiValue(prevAchievement.Ytd, kpiAchievement.Ytd);
                                kpiAchievement.ItdDeviation = CompareKpiValue(prevAchievement.Itd, kpiAchievement.Itd);
                            }
                            else
                            {
                                kpiAchievement.Deviation = "1";
                                kpiAchievement.MtdDeviation = "1";
                                kpiAchievement.YtdDeviation = "1";
                                kpiAchievement.ItdDeviation = "1";
                            }
                        }
                        DataContext.KpiAchievements.Add(kpiAchievement);
                    }
                }


                DataContext.SaveChanges(action);
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
            catch (Exception exception)
            {
                response.Message = exception.Message;
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
                var action = ((BaseRequest)request).MapTo<BaseAction>();
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
                DataContext.SaveChanges(action);
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

        public GetKpiAchievementLessThanOrEqualResponse GetKpiAchievementLessThanOrEqual(int kpiId, DateTime date, PeriodeType periodeType)
        {

            var response = new GetKpiAchievementLessThanOrEqualResponse();
            try
            {
                var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                var data = DataContext.KpiAchievements.Include(x => x.Kpi).Where(x => x.Kpi.Id == kpiId && x.Periode <= date).AsQueryable();
                var result = new KpiAchievement();
                switch (periodeType)
                {
                    case PeriodeType.Daily:
                    case PeriodeType.Monthly:
                    case PeriodeType.Yearly:
                        {
                            result = data.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.PeriodeType == periodeType);
                            break;
                        }

                }

                var kpiResponse = new GetKpiAchievementLessThanOrEqualResponse.KpiResponse
                {
                    Id = kpi.Id,
                    Measurement = kpi.Measurement.Name,
                    Name = kpi.Name,
                    Remark = kpi.Remark,
                };
                if (result == null)
                {
                    return new GetKpiAchievementLessThanOrEqualResponse
                    {
                        Periode = date,
                        Value = "no invtgtn",
                        Mtd = "no invtgtn",
                        Ytd = "no invtgtn",
                        Itd = "no invtgtn",
                        Kpi = kpiResponse,
                        IsSuccess = true
                    };
                }
                else
                {
                    return new GetKpiAchievementLessThanOrEqualResponse
                    {
                        Id = result.Id,
                        Periode = result.Periode,
                        Value = (result != null) && result.Periode == date ? result.Value.ToString() : "no invtgtn",
                        Mtd = (result != null) && result.Periode.Month == date.Month && result.Periode.Year == date.Year ? result.Mtd.ToString() : "no invtgtn",
                        Ytd = (result != null) && result.Periode.Year == date.Year ? result.Ytd.ToString() : "no invtgtn",
                        Itd = (result != null) ? result.Itd.ToString() : "no invtgtn",
                        Remark = (result != null) ? result.Remark : null,
                        Kpi = kpiResponse,
                        Deviation = (result != null) ? result.Deviation : null,
                        MtdDeviation = (result != null) ? result.MtdDeviation : null,
                        YtdDeviation = (result != null) ? result.YtdDeviation : null,
                        ItdDeviation = (result != null) ? result.ItdDeviation : null,
                        IsSuccess = true
                    };
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
                    case PeriodeType.Monthly:
                    case PeriodeType.Yearly:
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
                if (result == null)
                {
                    return new GetKpiAchievementResponse
                    {
                        IsSuccess = false,
                        Message = "There is no actual value at this periode of time"
                    };
                }
                else
                {
                    return new GetKpiAchievementResponse
                    {
                        Id = result.Id,
                        Periode = result.Periode,
                        Value = (result != null) ? result.Value : null,
                        Mtd = (result != null) ? result.Mtd : null,
                        Ytd = (result != null) ? result.Ytd : null,
                        Itd = (result != null) ? result.Itd : null,
                        Remark = (result != null) ? result.Remark : null,
                        Kpi = kpiResponse,
                        Deviation = (result != null) ? result.Deviation : null,
                        MtdDeviation = (result != null) ? result.MtdDeviation : null,
                        YtdDeviation = (result != null) ? result.YtdDeviation : null,
                        ItdDeviation = (result != null) ? result.ItdDeviation : null,
                        IsSuccess = true
                    };
                }

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

        public UpdateKpiAchievementItemResponse UpdateOriginalData(UpdateKpiAchievementItemRequest request, bool checkManualInput = false)
        {
            var response = new UpdateKpiAchievementItemResponse();
            try
            {
                //check method value
                var involvedKpi = DataContext.Kpis.Include(x => x.Method).SingleOrDefault(x => x.Id == request.KpiId);
                if (checkManualInput && !string.Equals(involvedKpi.Method.Name, "Manual Input", StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(request.ValueType, "remark", StringComparison.InvariantCultureIgnoreCase))
                {
                    response.Id = request.Id;
                    response.IsSuccess = true;
                    response.Message = "KPI Achievement item has been updated successfully";
                    return response;
                }

                var action = request.MapTo<BaseAction>();
                var user = DataContext.Users.First(x => x.Id == request.UserId);
                var kpiAchievement = request.MapTo<KpiAchievement>();
                DateTime prevDate;
                switch (request.PeriodeType)
                {
                    case PeriodeType.Yearly:
                        prevDate = request.Periode.AddYears(-1);
                        break;
                    case PeriodeType.Monthly:
                        prevDate = request.Periode.AddMonths(-1);
                        break;
                    default:
                        prevDate = request.Periode.AddDays(-1);
                        break;
                }
                KpiAchievement prevAchievement = DataContext.KpiAchievements.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.Periode == prevDate && x.PeriodeType == request.PeriodeType && x.Kpi.Id == request.KpiId);
                if (!string.Equals(request.ValueType, "remark", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (prevAchievement != null && kpiAchievement.Value != null)
                    {
                        kpiAchievement.Deviation = CompareKpiValue(prevAchievement.Value, kpiAchievement.Value);
                    }
                    else
                    {
                        kpiAchievement.Deviation = "1";
                    }
                }

                #region existing data
                if (request.Id > 0)
                {
                    if ((string.IsNullOrEmpty(request.Value) && request.Remark == null) || request.Value == "-" || (!string.IsNullOrEmpty(request.Value) && request.Value.Equals("null", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        //delete kpi achievement row and should send back the Id to 0
                        kpiAchievement = DataContext.KpiAchievements.Single(x => x.Id == request.Id);
                        DataContext.KpiAchievements.Remove(kpiAchievement);
                        DataContext.SaveChanges();
                        response.Id = 0;
                        response.IsSuccess = true;
                        response.Message = "KPI with Id =" + request.KpiId.ToString() + " was deleted successfully.";
                        return response;
                    }
                    else
                    {
                        kpiAchievement = DataContext.KpiAchievements
                                                .Include(x => x.Kpi)
                                                .Include(x => x.UpdatedBy)
                                                .Single(x => x.Id == request.Id);
                        if (request.Value != null)
                        {
                            kpiAchievement.Value = request.RealValue;
                        }
                        if (request.Remark != null)
                        {
                            kpiAchievement.Remark = request.Remark;
                        }
                        //request.MapPropertiesToInstance<KpiAchievement>(kpiAchievement);
                        kpiAchievement.UpdatedBy = user;
                        kpiAchievement.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                        if (prevAchievement != null)
                        {
                            kpiAchievement.Deviation = CompareKpiValue(prevAchievement.Value, kpiAchievement.Value);
                        }
                        else
                        {
                            kpiAchievement.Deviation = "1";
                        }

                    }
                    //special case
                    //if (request.Id == 65) {

                    //}
                }
                #endregion
                #region insert
                else if (request.Id == 0)
                {
                    //try to search existing data first

                    var exist = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == request.KpiId && x.PeriodeType == request.PeriodeType && x.Periode == request.Periode);
                    if (exist != null)
                    {
                        if (request.Remark != null || !string.IsNullOrEmpty(request.Remark))
                        {
                            exist.Remark = request.Remark;
                        }
                        //if (!string.IsNullOrEmpty(request.Value) && request.Value.ToLowerInvariant() == "null" && request.Value != "-")
                        if (request.RealValue != null)
                        {
                            exist.Value = request.RealValue;
                        }
                        exist.UpdatedBy = user;
                        exist.UpdatedDate = DateTime.Now;
                        if (prevAchievement != null)
                        {
                            exist.Deviation = CompareKpiValue(prevAchievement.Value, kpiAchievement.Value);
                        }
                        else
                        {
                            exist.Deviation = "1";
                        }
                        kpiAchievement = exist;
                    }
                    else
                    {
                        if (kpiAchievement.Value != null || kpiAchievement.Remark != null)
                        {
                            kpiAchievement.CreatedBy = user;
                            kpiAchievement.UpdatedBy = user;
                            kpiAchievement.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.KpiId);
                            DataContext.KpiAchievements.Add(kpiAchievement);
                        }
                    }
                    //Masih bikin data double, sebab 2 field dengan arah yang sama dan kpi yang sama bisa jadi memiliki data-id yang beda, karena saat return data-id dari service
                    //data-id untuk field yang lainnya belum terupdate
                    //if (((string.IsNullOrEmpty(request.Value) && request.Remark == null) || request.Value == "-" ||
                    //      (!string.IsNullOrEmpty(request.Value) && request.Value.Equals("null", StringComparison.InvariantCultureIgnoreCase))) && request.Id == 0)
                    //{
                    //    response.IsSuccess = false;
                    //    response.Message = "You can not update this item because it is not existed";
                    //    return response;
                    //}
                    //else
                    //{
                    //    kpiAchievement.CreatedBy = user;
                    //    kpiAchievement.UpdatedBy = user;
                    //    kpiAchievement.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                    //    if (prevAchievement != null)
                    //    {
                    //        kpiAchievement.Deviation = CompareKpiValue(prevAchievement.Value, kpiAchievement.Value);
                    //    }
                    //    else
                    //    {
                    //        kpiAchievement.Deviation = "1";
                    //    }
                    //    DataContext.KpiAchievements.Add(kpiAchievement);
                    //}
                }
                #endregion

                DataContext.SaveChanges(action);
                request.Id = request.Id > 0 ? request.Id : kpiAchievement.Id;
                if (!string.IsNullOrEmpty(request.Value))
                {
                    switch (request.PeriodeType)
                    {
                        #region yearly
                        case PeriodeType.Yearly:
                            if (kpiAchievement.Kpi == null)
                            {
                                break;
                            }
                            if (kpiAchievement.Kpi.YtdFormula == YtdFormula.Sum)
                            {
                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Yearly
                                  && x.Periode <= request.Periode
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);
                                kpiAchievement.Itd = itdValue;
                            }
                            else if (kpiAchievement.Kpi.YtdFormula == YtdFormula.Average)
                            {
                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Yearly
                                  && x.Periode <= request.Periode
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);
                                kpiAchievement.Itd = itdValue;
                            }
                            else
                            {
                                kpiAchievement.Itd = kpiAchievement.Value;
                            }
                            if (prevAchievement != null)
                            {
                                kpiAchievement.ItdDeviation = CompareKpiValue(prevAchievement.Itd, kpiAchievement.Itd);
                            }
                            else
                            {
                                kpiAchievement.ItdDeviation = "1";
                            }
                            DataContext.SaveChanges(action);
                            break;
                        #endregion
                        #region monthly
                        case PeriodeType.Monthly:
                            if (kpiAchievement.Kpi == null)
                            {
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == request.KpiId && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (yearly != null)
                                {
                                    yearly.Value -= kpiAchievement.Value;
                                    DataContext.SaveChanges(action);
                                }
                                break;
                            }
                            if (kpiAchievement.Kpi.YtdFormula == YtdFormula.Sum)
                            {
                                var ytdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Monthly
                            && x.Periode.Year == request.Periode.Year
                            && x.Periode <= request.Periode
                            && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (yearly != null)
                                {
                                    yearly.Value = ytdValue;
                                    yearly.UpdatedBy = user;
                                    yearly.UpdatedDate = DateTime.Now;
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
                                DataContext.SaveChanges(action);

                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Monthly
                                 && x.Periode <= request.Periode
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);

                                yearly.Itd = itdValue;

                                kpiAchievement.Ytd = ytdValue;
                                kpiAchievement.Itd = itdValue;
                            }
                            else if (kpiAchievement.Kpi.YtdFormula == YtdFormula.Average)
                            {
                                var ytdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Monthly
                            && x.Periode.Year == request.Periode.Year
                            && x.Periode <= request.Periode
                            && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (yearly != null)
                                {
                                    yearly.Value = ytdValue;
                                    yearly.UpdatedBy = user;
                                    yearly.UpdatedDate = DateTime.Now;
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
                                DataContext.SaveChanges(action);

                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Monthly
                                 && x.Periode <= request.Periode
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);

                                yearly.Itd = itdValue;

                                kpiAchievement.Ytd = ytdValue;
                                kpiAchievement.Itd = itdValue;
                            }
                            else
                            {
                                kpiAchievement.Ytd = kpiAchievement.Value;
                                kpiAchievement.Itd = kpiAchievement.Value;
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (yearly != null)
                                {
                                    yearly.Value = kpiAchievement.Value;
                                    yearly.Itd = kpiAchievement.Value;
                                    yearly.UpdatedBy = user;
                                    yearly.UpdatedDate = DateTime.Now;
                                }
                                else
                                {
                                    yearly = new KpiAchievement
                                    {
                                        Value = kpiAchievement.Value,
                                        Periode = new DateTime(request.Periode.Year, 1, 1),
                                        PeriodeType = PeriodeType.Yearly,
                                        Kpi = kpiAchievement.Kpi,
                                        CreatedBy = user,
                                        UpdatedBy = user,
                                        Itd = kpiAchievement.Value
                                    };
                                    DataContext.KpiAchievements.Add(yearly);
                                }
                            }
                            if (prevAchievement != null)
                            {
                                kpiAchievement.YtdDeviation = CompareKpiValue(prevAchievement.Ytd, kpiAchievement.Ytd);
                                kpiAchievement.ItdDeviation = CompareKpiValue(prevAchievement.Itd, kpiAchievement.Itd);
                            }
                            else
                            {
                                kpiAchievement.YtdDeviation = "1";
                                kpiAchievement.ItdDeviation = "1";
                            }
                            DataContext.SaveChanges(action);
                            break;
                        #endregion
                        #region daily
                        default:
                            if (kpiAchievement.Kpi == null)
                            {
                                var monthly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == request.KpiId && x.Periode.Month == request.Periode.Month && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Monthly);
                                if (monthly != null)
                                {
                                    monthly.Value -= kpiAchievement.Value;
                                    DataContext.SaveChanges(action);
                                }
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == request.KpiId && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (yearly != null)
                                {
                                    yearly.Value -= kpiAchievement.Value;
                                    DataContext.SaveChanges(action);
                                }
                                break;
                            }
                            if (kpiAchievement.Kpi.YtdFormula == YtdFormula.Sum)
                            {
                                var mtdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Daily
                                && x.Periode.Year == request.Periode.Year && x.Periode.Month == request.Periode.Month
                                && x.Periode <= request.Periode
                                && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);
                                kpiAchievement.Mtd = mtdValue;
                                var monthly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Month == request.Periode.Month && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Monthly);
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
                                DataContext.SaveChanges(action);

                                var ytdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Daily
                                  && x.Periode <= request.Periode
                                && x.Periode.Year == request.Periode.Year
                                && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
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
                                DataContext.SaveChanges(action);

                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Daily
                                  && x.Periode <= request.Periode
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Sum(x => x.Value);

                                yearly.Itd = itdValue;

                                monthly.Ytd = ytdValue;
                                monthly.Itd = itdValue;

                                kpiAchievement.Mtd = mtdValue;
                                kpiAchievement.Ytd = ytdValue;
                                kpiAchievement.Itd = itdValue;

                            }
                            else if (kpiAchievement.Kpi.YtdFormula == YtdFormula.Average)
                            {
                                var mtdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Daily
                               && x.Periode.Year == request.Periode.Year && x.Periode.Month == request.Periode.Month
                               && x.Periode <= request.Periode
                               && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);
                                kpiAchievement.Mtd = mtdValue;
                                var monthly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Month == request.Periode.Month && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Monthly);
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
                                DataContext.SaveChanges(action);

                                var ytdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Daily
                                && x.Periode <= request.Periode
                                && x.Periode.Year == request.Periode.Year
                                && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
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
                                DataContext.SaveChanges(action);

                                var itdValue = DataContext.KpiAchievements.Where(x => x.PeriodeType == PeriodeType.Daily
                                 && x.Periode <= request.Periode
                                  && x.Kpi.Id == kpiAchievement.Kpi.Id).Average(x => x.Value);

                                yearly.Itd = itdValue;

                                monthly.Ytd = ytdValue;
                                monthly.Itd = itdValue;

                                kpiAchievement.Mtd = mtdValue;
                                kpiAchievement.Ytd = ytdValue;
                                kpiAchievement.Itd = itdValue;
                            }
                            else
                            {
                                var monthly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Month == request.Periode.Month && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Monthly);
                                if (monthly != null)
                                {
                                    monthly.Value = kpiAchievement.Value;
                                }
                                else
                                {
                                    monthly = new KpiAchievement
                                    {
                                        Value = kpiAchievement.Value,
                                        Periode = new DateTime(request.Periode.Year, request.Periode.Month, 1),
                                        PeriodeType = PeriodeType.Monthly,
                                        Kpi = kpiAchievement.Kpi,
                                        CreatedBy = user,
                                        UpdatedBy = user
                                    };
                                    DataContext.KpiAchievements.Add(monthly);
                                }
                                DataContext.SaveChanges(action);

                                var ytdValue = kpiAchievement.Value;
                                var yearly = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiAchievement.Kpi.Id && x.Periode.Year == request.Periode.Year && x.PeriodeType == PeriodeType.Yearly);
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
                                DataContext.SaveChanges(action);

                                var itdValue = kpiAchievement.Value;

                                yearly.Itd = itdValue;

                                monthly.Ytd = ytdValue;
                                monthly.Itd = itdValue;

                                kpiAchievement.Mtd = kpiAchievement.Value;
                                kpiAchievement.Ytd = kpiAchievement.Value;
                                kpiAchievement.Itd = kpiAchievement.Value;

                            }
                            if (prevAchievement != null)
                            {
                                kpiAchievement.MtdDeviation = !string.IsNullOrEmpty(CompareKpiValue(prevAchievement.Mtd, kpiAchievement.Mtd)) ? CompareKpiValue(prevAchievement.Mtd, kpiAchievement.Mtd) : "1";
                                kpiAchievement.YtdDeviation = !string.IsNullOrEmpty(CompareKpiValue(prevAchievement.Ytd, kpiAchievement.Ytd)) ? CompareKpiValue(prevAchievement.Ytd, kpiAchievement.Ytd) : "1";
                                kpiAchievement.ItdDeviation = !string.IsNullOrEmpty(CompareKpiValue(prevAchievement.Itd, kpiAchievement.Itd)) ? CompareKpiValue(prevAchievement.Ytd, kpiAchievement.Ytd) : "1";
                            }
                            else
                            {
                                kpiAchievement.MtdDeviation = "1";
                                kpiAchievement.YtdDeviation = "1";
                                kpiAchievement.ItdDeviation = "1";
                            }
                            //special case
                            if (request.KpiId == 65)
                            {
                                var kpiActual519 = DataContext.KpiAchievements.SingleOrDefault(x => x.Kpi.Id == 519 && x.Periode == request.Periode && x.PeriodeType == request.PeriodeType);
                                if (kpiActual519 != null)
                                {
                                    kpiActual519.Value = kpiAchievement.Mtd - 17;
                                }
                                else
                                {
                                    kpiActual519 = new KpiAchievement();
                                    kpiActual519.Periode = request.Periode;
                                    kpiActual519.PeriodeType = request.PeriodeType;
                                    kpiActual519.CreatedBy = user;
                                    kpiActual519.UpdatedBy = user;
                                    kpiActual519.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == 519);
                                    kpiActual519.Value = kpiAchievement.Mtd - 17;
                                    DataContext.KpiAchievements.Add(kpiActual519);
                                    //save actual daily for 519
                                }
                                DataContext.SaveChanges(action);
                            }
                            DataContext.SaveChanges(action);
                            break;
                            #endregion
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
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {
                response.Message = ex.Message;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private string CompareKpiValue(double? prevValue, double? currentValue)
        {
            if (!prevValue.HasValue || !currentValue.HasValue)
            {
                return string.Empty;
            }
            if (currentValue.Value > prevValue.Value)
            {
                return "1";
            }
            else if (currentValue.Value < prevValue.Value)
            {
                return "-1";
            }
            else
            {
                return "0";
            }
        }

        public UpdateKpiAchievementItemResponse UpdateCustomJccFormula(UpdateKpiAchievementItemRequest request)
        {
            var response = new UpdateKpiAchievementItemResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var user = DataContext.Users.First(x => x.Id == request.UserId);
                var origin = UpdateOriginalData(request, true);
                if (origin.IsSuccess)
                {
                    var jccPrice = double.Parse(request.Value);
                    /*
                     * Saving Current KPI Achievement for JCC Price
                     */

                    /*
                     * Get Feed Gas GSA JOB Value from Custom Formula  using JCC Price
                     */
                    var feedGasGSA_JOB = _customService.GetFeedGasGSA_JOB(new GetFeedGasGSARequest { JccPrice = jccPrice });
                    var FeedGasGSAJOB_Real = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == 381 && x.PeriodeType == PeriodeType.Monthly && x.Periode == request.Periode);
                    var lastFeedGasGSAJOB_Real = DataContext.KpiAchievements.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.Kpi.Id == 381 && x.PeriodeType == PeriodeType.Monthly && x.Periode < request.Periode);
                    var deviation = "1";
                    if (feedGasGSA_JOB.IsSuccess && lastFeedGasGSAJOB_Real != null)
                    {
                        deviation = CompareKpiValue(lastFeedGasGSAJOB_Real.Value.Value, feedGasGSA_JOB.Value);
                    }
                    if (FeedGasGSAJOB_Real != null && feedGasGSA_JOB.IsSuccess)
                    {
                        FeedGasGSAJOB_Real.Value = feedGasGSA_JOB.Value;
                        FeedGasGSAJOB_Real.UpdatedBy = user;
                        FeedGasGSAJOB_Real.UpdatedDate = DateTime.Now;
                        FeedGasGSAJOB_Real.Deviation = FeedGasGSAJOB_Real.YtdDeviation = FeedGasGSAJOB_Real.MtdDeviation = FeedGasGSAJOB_Real.ItdDeviation = deviation;

                    }
                    else if (FeedGasGSAJOB_Real == null && feedGasGSA_JOB.IsSuccess)
                    {
                        FeedGasGSAJOB_Real = new KpiAchievement
                        {
                            Value = feedGasGSA_JOB.Value,
                            Periode = request.Periode,
                            PeriodeType = PeriodeType.Monthly,
                            Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == 381),
                            CreatedBy = user,
                            UpdatedBy = user,
                            Deviation = deviation,
                            MtdDeviation = deviation,
                            YtdDeviation = deviation,
                            ItdDeviation = deviation

                        };
                        DataContext.KpiAchievements.Add(FeedGasGSAJOB_Real);
                    }
                    /*
                     * Get Feed Gas GSA MGDP Value from Custom Formula  using JCC Price
                     */
                    var feedGasGSA_MGDP = _customService.GetFeedGasGSA_MGDP(new GetFeedGasGSARequest { JccPrice = jccPrice });
                    var FeedGasGSAMGDP_Real = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == 382 && x.PeriodeType == PeriodeType.Monthly && x.Periode == request.Periode);
                    var lastFeedGasGSAMGDP_Real = DataContext.KpiAchievements.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.Kpi.Id == 382 && x.PeriodeType == PeriodeType.Monthly && x.Periode < request.Periode);
                    deviation = "1";
                    if (feedGasGSA_MGDP.IsSuccess && lastFeedGasGSAMGDP_Real != null)
                    {
                        deviation = CompareKpiValue(lastFeedGasGSAMGDP_Real.Value.Value, feedGasGSA_MGDP.Value);
                    }
                    if (FeedGasGSAMGDP_Real != null && feedGasGSA_MGDP.IsSuccess)
                    {
                        FeedGasGSAMGDP_Real.Value = feedGasGSA_MGDP.Value;
                        FeedGasGSAMGDP_Real.UpdatedBy = user;
                        FeedGasGSAMGDP_Real.UpdatedDate = DateTime.Now;
                        FeedGasGSAMGDP_Real.Deviation = FeedGasGSAMGDP_Real.YtdDeviation = FeedGasGSAMGDP_Real.MtdDeviation = FeedGasGSAMGDP_Real.ItdDeviation = deviation;
                    }
                    else if (FeedGasGSAMGDP_Real == null && feedGasGSA_MGDP.IsSuccess)
                    {
                        var kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == 382);
                        FeedGasGSAMGDP_Real = new KpiAchievement
                        {
                            Value = feedGasGSA_MGDP.Value,
                            Periode = request.Periode,
                            PeriodeType = PeriodeType.Monthly,
                            Kpi = kpi,
                            CreatedBy = user,
                            UpdatedBy = user,
                            Deviation = deviation,
                            MtdDeviation = deviation,
                            YtdDeviation = deviation,
                            ItdDeviation = deviation
                        };
                        DataContext.KpiAchievements.Add(FeedGasGSAMGDP_Real);
                    }
                    /*
                     * Get LNG Price SPA FOB Value from Custom Formula using JCC Price
                     */
                    var LNGPriceSPA_FOB = _customService.GetLNGPriceSPA_FOB(new GetFeedGasGSARequest { JccPrice = jccPrice });
                    var LNGPriceSPAFOB_Real = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == 186 && x.PeriodeType == PeriodeType.Monthly && x.Periode == request.Periode);
                    var lastLNGPriceSPAFOB_Real = DataContext.KpiAchievements.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.Kpi.Id == 186 && x.PeriodeType == PeriodeType.Monthly && (x.Periode.Month <= request.Periode.Month && x.Periode.Year <= request.Periode.Year));
                    deviation = "1";
                    if (LNGPriceSPA_FOB.IsSuccess && lastLNGPriceSPAFOB_Real != null)
                    {
                        deviation = CompareKpiValue(lastLNGPriceSPAFOB_Real.Value.Value, LNGPriceSPA_FOB.Value);
                    }
                    if (LNGPriceSPAFOB_Real != null && LNGPriceSPA_FOB.IsSuccess)
                    {
                        LNGPriceSPAFOB_Real.Value = LNGPriceSPA_FOB.Value;
                        LNGPriceSPAFOB_Real.UpdatedBy = user;
                        LNGPriceSPAFOB_Real.UpdatedDate = DateTime.Now;
                        LNGPriceSPAFOB_Real.Deviation = LNGPriceSPAFOB_Real.YtdDeviation = LNGPriceSPAFOB_Real.MtdDeviation = LNGPriceSPAFOB_Real.ItdDeviation = deviation;
                    }
                    else if (LNGPriceSPAFOB_Real == null && LNGPriceSPA_FOB.IsSuccess)
                    {
                        LNGPriceSPAFOB_Real = new KpiAchievement
                        {
                            Value = LNGPriceSPA_FOB.Value,
                            Periode = request.Periode,
                            PeriodeType = PeriodeType.Monthly,
                            Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == 186),
                            CreatedBy = user,
                            UpdatedBy = user,
                            Deviation = deviation,
                            MtdDeviation = deviation,
                            YtdDeviation = deviation,
                            ItdDeviation = deviation
                        };
                        DataContext.KpiAchievements.Add(LNGPriceSPAFOB_Real);
                    }

                    DataContext.SaveChanges(action);
                    response.IsSuccess = true;
                    response.Message = "JCC Price Calculation Succeded";
                }
            }
            catch (InvalidOperationException exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }
            catch (ArgumentNullException exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }
            catch (Exception exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }
            return response;
        }
        /// <summary>
        /// Calculate LNG Price(SPA) DES from JCC Price and Bunker Price
        /// </summary>
        /// <param name="request">UpdateKpiAchievementItemRequest</param>
        /// <returns>UpdateKpiAchievementItemResponse</returns>
        public UpdateKpiAchievementItemResponse UpdateCustomBunkerPriceFormula(UpdateKpiAchievementItemRequest request)
        {
            var response = new UpdateKpiAchievementItemResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var user = DataContext.Users.First(x => x.Id == request.UserId);
                var origin = UpdateOriginalData(request, true);

                /*
                 * Try to Update Dependend Object/KPI
                 */
                if (origin.IsSuccess)
                {
                    // Get Value of Bunker Price from request
                    var bunkerPrice = double.Parse(request.Value);
                    // Get JCC Price of Current Periode Request
                    var jccPrice = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == 62 && x.PeriodeType == PeriodeType.Monthly && x.Periode == request.Periode).Value;
                    if (!jccPrice.HasValue)
                    {
                        response.IsSuccess = false;
                        response.Message = "JCC Price not found, Please Insert JCC Price first";
                        return response;
                    }
                    // Get LNGPriceSPA_DES
                    var LNGPriceSPA_DES = _customService.GetLNGPriceSPA_DES(new GetLNGPriceSpaRequest { JccPrice = jccPrice.Value, BunkerPrice = bunkerPrice });

                    if (!LNGPriceSPA_DES.IsSuccess)
                    {
                        response.IsSuccess = false;
                        response.Message = "Could Not Calculate LNG Price SPA DESC";
                        return response;
                    }

                    // Get Existing LNG Price(SPA) DES value of selected periode
                    var existingLNG_Price_ADP = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == 187 && x.PeriodeType == PeriodeType.Monthly && x.Periode == request.Periode);
                    var lastLNG_Price_ADP = DataContext.KpiAchievements.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.Kpi.Id == 187 && x.PeriodeType == PeriodeType.Monthly && x.Periode < request.Periode);
                    var deviation = "1";
                    if (lastLNG_Price_ADP != null)
                    {
                        deviation = CompareKpiValue(lastLNG_Price_ADP.Value.Value, LNGPriceSPA_DES.Value);
                    }

                    // KPI Actual already exist
                    if (existingLNG_Price_ADP != null)
                    {
                        existingLNG_Price_ADP.Value = LNGPriceSPA_DES.Value;
                        existingLNG_Price_ADP.UpdatedBy = user;
                        existingLNG_Price_ADP.UpdatedDate = DateTime.Now;
                        existingLNG_Price_ADP.Deviation = existingLNG_Price_ADP.MtdDeviation = existingLNG_Price_ADP.YtdDeviation = existingLNG_Price_ADP.ItdDeviation = deviation;
                    }
                    else
                    {
                        existingLNG_Price_ADP = new KpiAchievement
                        {
                            Value = LNGPriceSPA_DES.Value,
                            Periode = request.Periode,
                            PeriodeType = PeriodeType.Monthly,
                            Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == 187),
                            CreatedBy = user,
                            UpdatedBy = user,
                            Deviation = deviation,
                            MtdDeviation = deviation,
                            YtdDeviation = deviation,
                            ItdDeviation = deviation
                        };
                        DataContext.KpiAchievements.Add(existingLNG_Price_ADP);
                    }

                    DataContext.SaveChanges(action);
                    response.IsSuccess = true;
                    response.Message = "Bunker Price Calculation Succeded";
                }

            }
            catch (InvalidOperationException o)
            {
                response.IsSuccess = false;
                response.Message = o.Message;
            }
            catch (ArgumentNullException a)
            {
                response.IsSuccess = false;
                response.Message = a.Message;
            }
            catch (Exception a)
            {
                response.IsSuccess = false;
                response.Message = a.Message;
            }
            return response;
        }

        public BaseResponse DeleteKpiAchievement(DeleteKpiAchievementRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var achievements = DataContext.KpiAchievements.Where(
                x => x.Kpi.Id == request.kpiId && x.Periode == request.periode && x.PeriodeType == request.periodeType).ToList();
                foreach (var achievement in achievements)
                {
                    DataContext.KpiAchievements.Remove(achievement);
                }
                DataContext.SaveChanges(action);
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

        public GetAchievementsResponse GetKpiAchievements(int[] kpiIds, DateTime? start, DateTime? end, string periodeType)
        {
            PeriodeType pType = (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);
            var response = new GetAchievementsResponse();
            try
            {
                var kpiAchievement = DataContext.KpiAchievements
                    .Include(x => x.Kpi.Measurement)
                    .OrderBy(x => x.Kpi.Order)
                    .Where(x => kpiIds.Contains(x.Kpi.Id) && x.PeriodeType == pType && x.Periode >= start.Value && x.Periode <= end.Value)
                    .ToList();
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

        public GetAchievementsResponse GetKpiEconomics(int[] kpiIds, DateTime? start, DateTime? end, string periodeType)
        {
            PeriodeType pType = (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);
            var response = new GetAchievementsResponse();
            try
            {
                var scenarioId = 0;
                var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                if (scenario != null)
                {
                    scenarioId = scenario.Id;
                }

                var kpiEconomics = DataContext.KeyOperationDatas
                    .Include(x => x.Kpi.Measurement)
                    .OrderBy(x => x.Kpi.Order)
                    .Where(x => kpiIds.Contains(x.Kpi.Id) && x.PeriodeType == pType && x.Periode >= start.Value && x.Periode <= end.Value && x.Scenario.Id == scenarioId)
                    .ToList();
                if (kpiEconomics.Count > 0)
                {
                    foreach (var item in kpiEconomics)
                    {
                        response.KpiAchievements.Add(new GetKpiAchievementResponse
                        {
                            Kpi = new GetKpiAchievementResponse.KpiResponse
                            {
                                Id = item.Kpi.Id,
                                Name = item.Kpi.Name,
                                Measurement = item.Kpi.Measurement.Name,
                                KpiMeasurement = item.Kpi.Measurement.Name,
                            },
                            Value = item.Value,
                            Periode = item.Periode,
                            PeriodeType = item.PeriodeType
                        });
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
    }
}