using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.KpiTarget;
using DSLNG.PEAR.Services.Responses.KpiTarget;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Services.Responses;

namespace DSLNG.PEAR.Services
{
    public class KpiTargetService : BaseService, IKpiTargetService
    {
        public KpiTargetService(IDataContext dataContext)
            : base(dataContext)
        {
        }
        public CreateKpiTargetsResponse Creates(CreateKpiTargetsRequest request)
        {
            var response = new CreateKpiTargetsResponse();
            try
            {
                if (request.KpiTargets.Count > 0)
                {
                    foreach (var kpiTarget in request.KpiTargets)
                    {
                        var data = kpiTarget.MapTo<KpiTarget>();
                        data.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == kpiTarget.KpiId);
                        DataContext.KpiTargets.Add(data);
                        DataContext.SaveChanges();
                    }
                    response.IsSuccess = true;
                    response.Message = "KPI Target has been added successfully";
                }
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public GetPmsConfigsResponse GetPmsConfigs(GetPmsConfigsRequest request)
        {
            var pmsSummary = DataContext.PmsSummaries
                                            .Include("PmsConfigs.Pillar")
                                            .Include("PmsConfigs.PmsConfigDetailsList.Kpi.Measurement")
                                            .FirstOrDefault(x => x.Id == request.Id);
            var response = new GetPmsConfigsResponse();
            var pmsConfigsList = new List<PmsConfig>();
            if (pmsSummary != null)
            {
                var pmsConfigs = pmsSummary.PmsConfigs.ToList();
                if (pmsConfigs.Count > 0)
                {
                    response.PmsConfigs = pmsConfigs.MapTo<GetPmsConfigsResponse.PmsConfig>();
                }
            }

            return response;
        }

        public GetKpiTargetsResponse GetKpiTargets(GetKpiTargetsRequest request)
        {
            var kpis = new List<KpiTarget>();
            if (request.Take != 0)
            {
                kpis = DataContext.KpiTargets.Include(x => x.Kpi).OrderBy(x => x.Kpi.Order).Skip(request.Skip).Take(request.Take).ToList();
            }
            else
            {
                kpis = DataContext.KpiTargets.Include(x => x.Kpi).OrderBy(x => x.Kpi.Order).ToList();
            }
            var response = new GetKpiTargetsResponse();
            response.KpiTargets = kpis.MapTo<GetKpiTargetsResponse.KpiTarget>();

            return response;
        }

        public GetKpiTargetResponse GetKpiTarget(GetKpiTargetRequest request)
        {
            var response = new GetKpiTargetResponse();
            try
            {
                var pmsSummary = DataContext.PmsSummaries.Single(x => x.Id == request.PmsSummaryId);
                response.Year = pmsSummary.Year;
                var pillarsAndKpis = DataContext.PmsConfigDetails
                        .Include(x => x.Kpi)
                        .Include(x => x.Kpi.KpiTargets)
                        .Include(x => x.Kpi.Measurement)
                        .Include(x => x.PmsConfig)
                        .Include(x => x.PmsConfig.PmsSummary)
                        .Include(x => x.PmsConfig.Pillar)
                        .Where(x => x.PmsConfig.PmsSummary.Id == request.PmsSummaryId)
                        .OrderBy(x => x.Kpi.Order)
                        .ToList()
                        .GroupBy(x => x.PmsConfig.Pillar)
                        .ToDictionary(x => x.Key);


                foreach (var item in pillarsAndKpis)
                {
                    var pillar = new GetKpiTargetResponse.Pillar() { Id = item.Key.Id, Name = item.Key.Name };

                    foreach (var val in item.Value)
                    {
                        var targets = new List<GetKpiTargetResponse.KpiTarget>();
                        switch (request.PeriodeType)
                        {
                            case PeriodeType.Monthly:
                                for (int i = 1; i <= 12; i++)
                                {
                                    var kpiTargetsMonthly = val.Kpi.KpiTargets.FirstOrDefault(x => x.PeriodeType == PeriodeType.Monthly
                                                    && x.Periode.Month == i && x.Periode.Year == pmsSummary.Year);
                                    var kpiTargetMonthly = new GetKpiTargetResponse.KpiTarget();
                                    if (kpiTargetsMonthly == null)
                                    {
                                        kpiTargetMonthly.Id = 0;
                                        kpiTargetMonthly.Periode = new DateTime(pmsSummary.Year, i, 1);
                                        kpiTargetMonthly.Value = null;
                                        kpiTargetMonthly.Remark = null;
                                    }
                                    else
                                    {
                                        kpiTargetMonthly.Id = kpiTargetsMonthly.Id;
                                        kpiTargetMonthly.Periode = kpiTargetsMonthly.Periode;
                                        kpiTargetMonthly.Value = kpiTargetsMonthly.Value;
                                        kpiTargetMonthly.Remark = kpiTargetsMonthly.Remark;
                                    }

                                    targets.Add(kpiTargetMonthly);
                                }
                                break;
                            case PeriodeType.Yearly:
                                var kpiTargetsYearly =
                                    val.Kpi.KpiTargets.FirstOrDefault(x => x.PeriodeType == PeriodeType.Yearly
                                                                           && x.Periode.Year == pmsSummary.Year);
                                var kpiTargetYearly = new GetKpiTargetResponse.KpiTarget();
                                if (kpiTargetsYearly == null)
                                {
                                    kpiTargetYearly.Id = 0;
                                    kpiTargetYearly.Periode = new DateTime(pmsSummary.Year, 1, 1);
                                    kpiTargetYearly.Value = null;
                                    kpiTargetYearly.Remark = null;
                                }
                                else
                                {
                                    kpiTargetYearly.Id = kpiTargetsYearly.Id;
                                    kpiTargetYearly.Periode = kpiTargetsYearly.Periode;
                                    kpiTargetYearly.Value = kpiTargetsYearly.Value;
                                    kpiTargetYearly.Remark = kpiTargetsYearly.Remark;
                                }
                                targets.Add(kpiTargetYearly);

                                break;
                        }

                        var kpi = new GetKpiTargetResponse.Kpi
                        {
                            Id = val.Kpi.Id,
                            Measurement = val.Kpi.Measurement.Name,
                            Name = val.Kpi.Name,
                            KpiTargets = targets
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

        public UpdateKpiTargetResponse UpdateKpiTarget(UpdateKpiTargetRequest request)
        {
            PeriodeType periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), request.PeriodeType);
            var response = new UpdateKpiTargetResponse();
            response.PeriodeType = periodeType;
            try
            {
                foreach (var pillar in request.Pillars)
                {
                    foreach (var kpi in pillar.Kpis)
                    {
                        foreach (var kpiTarget in kpi.KpiTargets)
                        {
                            if (kpiTarget.Id == 0)
                            {
                                var kpiTargetNew = new KpiTarget();
                                kpiTargetNew.Value = kpiTarget.Value;
                                kpiTargetNew.Kpi = DataContext.Kpis.Single(x => x.Id == kpi.Id);
                                kpiTargetNew.PeriodeType = periodeType;
                                kpiTargetNew.Periode = kpiTarget.Periode;
                                kpiTargetNew.IsActive = true;
                                kpiTargetNew.Remark = kpiTarget.Remark;
                                kpiTargetNew.CreatedDate = DateTime.Now;
                                kpiTargetNew.UpdatedDate = DateTime.Now;
                                DataContext.KpiTargets.Add(kpiTargetNew);
                            }
                            else
                            {
                                var kpiTargetNew = new KpiTarget();
                                kpiTargetNew.Id = kpiTarget.Id;
                                kpiTargetNew.Value = kpiTarget.Value;
                                kpiTargetNew.Kpi = DataContext.Kpis.Single(x => x.Id == kpi.Id);
                                kpiTargetNew.PeriodeType = periodeType;
                                kpiTargetNew.Periode = kpiTarget.Periode;
                                kpiTargetNew.IsActive = true;
                                kpiTargetNew.Remark = kpiTarget.Remark;
                                kpiTargetNew.UpdatedDate = DateTime.Now;
                                DataContext.KpiTargets.Attach(kpiTargetNew);
                                DataContext.Entry(kpiTargetNew).State = EntityState.Modified;
                            }
                        }
                    }
                }
                response.IsSuccess = true;
                response.Message = "KPI Target has been updated successfully";
                DataContext.SaveChanges();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                response.Message = invalidOperationException.Message;
            }

            return response;
        }

        public CreateKpiTargetResponse Create(CreateKpiTargetRequest request)
        {
            var response = new CreateKpiTargetResponse();
            try
            {

                var data = request.MapTo<KpiTarget>();
                data.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.KpiId);
                DataContext.KpiTargets.Add(data);
                DataContext.SaveChanges();

                response.IsSuccess = true;
                response.Message = "KPI Target has been added successfully";
                response.Id = data.Id;
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public UpdateKpiTargetItemResponse UpdateKpiTargetItem(UpdateKpiTargetItemRequest request)
        {
            var response = new UpdateKpiTargetItemResponse();
            try
            {
                var user = DataContext.Users.First(x => x.Id == request.UserId);
                var kpiTarget = request.MapTo<KpiTarget>();

                if (request.Id > 0)
                {
                    if (string.IsNullOrEmpty(request.Value) || request.Value == "-" || request.Value.ToLowerInvariant() == "null")
                    {
                        kpiTarget = DataContext.KpiTargets.Single(x => x.Id == request.Id);
                        DataContext.KpiTargets.Remove(kpiTarget);
                    }
                    else
                    {
                        kpiTarget = DataContext.KpiTargets
                                                .Include(x => x.Kpi)
                                                .Include(x => x.UpdatedBy)
                                                .Single(x => x.Id == request.Id);
                        request.MapPropertiesToInstance<KpiTarget>(kpiTarget);
                        kpiTarget.UpdatedBy = user;
                        kpiTarget.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                    }
                }
                else if (request.Id == 0)
                {
                    if ((string.IsNullOrEmpty(request.Value) || request.Value == "-" ||
                         request.Value.ToLowerInvariant() == "null") && request.Id == 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Value is null but data is not existed before";
                        return response;
                    }
                    else
                    {
                        kpiTarget.CreatedBy = user;
                        kpiTarget.UpdatedBy = user;
                        kpiTarget.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                        DataContext.KpiTargets.Add(kpiTarget);
                    }
                }


                DataContext.SaveChanges();
                response.Id = request.Id > 0 ? request.Id : kpiTarget.Id;
                response.IsSuccess = true;
                response.Message = "KPI Target item has been updated successfully";
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
            /*var response = new UpdateKpiTargetItemResponse();
            try
            {
                var kpiTarget = request.MapTo<KpiTarget>();
                DataContext.KpiTargets.Attach(kpiTarget);
                DataContext.Entry(kpiTarget).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.Id = request.Id;
                response.IsSuccess = true;
                response.Message = "KPI Target item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;*/
        }

        public GetKpiTargetsConfigurationResponse GetKpiTargetsConfiguration(GetKpiTargetsConfigurationRequest request)
        {
            var response = new GetKpiTargetsConfigurationResponse();
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


                switch (periodeType)
                {
                    case PeriodeType.Yearly:
                        var kpiTargetsYearly = DataContext.KpiTargets
                                        .Include(x => x.Kpi)
                                        .Where(x => x.PeriodeType == periodeType).ToList();
                        foreach (var kpi in kpis)
                        {
                            var kpiDto = kpi.MapTo<GetKpiTargetsConfigurationResponse.Kpi>();
                            foreach (var number in YearlyNumbers)
                            {
                                var achievement = kpiTargetsYearly.SingleOrDefault(x => x.Kpi != null && x.Kpi.Id == kpi.Id && x.Periode.Year == number);
                                if (achievement != null)
                                {
                                    var targetDto =
                                        achievement.MapTo<GetKpiTargetsConfigurationResponse.KpiTarget>();
                                    kpiDto.KpiTargets.Add(targetDto);
                                }
                                else
                                {
                                    var targetDto = new GetKpiTargetsConfigurationResponse.KpiTarget();
                                    targetDto.Periode = new DateTime(number, 1, 1);
                                    kpiDto.KpiTargets.Add(targetDto);
                                }
                            }


                            response.Kpis.Add(kpiDto);
                        }
                        break;

                    case PeriodeType.Monthly:
                        var kpiTargetsMonthly = DataContext.KpiTargets
                                        .Include(x => x.Kpi)
                                        .Where(x => x.PeriodeType == periodeType && x.Periode.Year == request.Year).ToList();
                        foreach (var kpi in kpis)
                        {
                            var kpiDto = kpi.MapTo<GetKpiTargetsConfigurationResponse.Kpi>();
                            var targets = kpiTargetsMonthly.Where(x => x.Kpi.Id == kpi.Id).ToList();

                            for (int i = 1; i <= 12; i++)
                            {
                                var target = targets.FirstOrDefault(x => x.Periode.Month == i);
                                if (target != null)
                                {
                                    var achievementDto =
                                        target.MapTo<GetKpiTargetsConfigurationResponse.KpiTarget>();
                                    kpiDto.KpiTargets.Add(achievementDto);
                                }
                                else
                                {
                                    var achievementDto = new GetKpiTargetsConfigurationResponse.KpiTarget();
                                    achievementDto.Periode = new DateTime(request.Year, i, 1);
                                    kpiDto.KpiTargets.Add(achievementDto);
                                }
                            }
                            response.Kpis.Add(kpiDto);
                        }
                        break;

                    case PeriodeType.Daily:
                        var kpiTargetsDaily = DataContext.KpiTargets
                                        .Include(x => x.Kpi)
                                        .Where(x => x.PeriodeType == periodeType && x.Periode.Year == request.Year
                                        && x.Periode.Month == request.Month).ToList();
                        foreach (var kpi in kpis)
                        {
                            var kpiDto = kpi.MapTo<GetKpiTargetsConfigurationResponse.Kpi>();
                            var targets = kpiTargetsDaily.Where(x => x.Kpi.Id == kpi.Id).ToList();
                            for (int i = 1; i <= DateTime.DaysInMonth(request.Year, request.Month); i++)
                            {
                                var target = targets.FirstOrDefault(x => x.Periode.Day == i);
                                if (target != null)
                                {
                                    var targetDto =
                                        target.MapTo<GetKpiTargetsConfigurationResponse.KpiTarget>();
                                    kpiDto.KpiTargets.Add(targetDto);
                                }
                                else
                                {
                                    var targetDto = new GetKpiTargetsConfigurationResponse.KpiTarget();
                                    targetDto.Periode = new DateTime(request.Year, request.Month, i);
                                    kpiDto.KpiTargets.Add(targetDto);
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

        public AllKpiTargetsResponse GetAllKpiTargets()
        {
            var response = new AllKpiTargetsResponse();
            try
            {
                var kpiTargets = DataContext.Kpis
                                            .Include(x => x.Measurement)
                                            .Include(x => x.Type)
                                            .Include(x => x.RoleGroup)
                                            .AsEnumerable()
                                            .OrderBy(x => x.Order)
                                            .GroupBy(x => x.RoleGroup).ToDictionary(x => x.Key);



                foreach (var item in kpiTargets)
                {
                    var kpis = new List<AllKpiTargetsResponse.Kpi>();
                    foreach (var val in item.Value)
                    {
                        kpis.Add(val.MapTo<AllKpiTargetsResponse.Kpi>());
                    }

                    response.RoleGroups.Add(new AllKpiTargetsResponse.RoleGroup
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

        public GetKpiTargetItemResponse GetKpiTargetByValue(GetKpiTargetRequestByValue request)
        {
            var response = new GetKpiTargetItemResponse();
            PeriodeType periodeType = (PeriodeType)Enum.Parse(typeof(PeriodeType), request.PeriodeType);
            response.PeriodeType = periodeType;
            try
            {
                var kpiTarget = DataContext.KpiTargets.Include(x => x.Kpi).Single(x => x.Kpi.Id == request.Kpi_Id && x.PeriodeType == periodeType && x.Periode == request.periode);
                response = kpiTarget.MapTo<GetKpiTargetItemResponse>();
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

        public UpdateKpiTargetItemResponse SaveKpiTargetItem(SaveKpiTargetRequest request)
        {
            var response = new UpdateKpiTargetItemResponse();
            try
            {
                var kpiTarget = request.MapTo<KpiTarget>();

                if (request.Id != 0)
                {
                    var attachedEntity = DataContext.KpiTargets.Find(request.Id);
                    if (attachedEntity != null && DataContext.Entry(attachedEntity).State != EntityState.Detached)
                    {
                        DataContext.Entry(attachedEntity).State = EntityState.Detached;
                    }
                    DataContext.KpiTargets.Attach(kpiTarget);
                    DataContext.Entry(kpiTarget).State = EntityState.Modified;
                    DataContext.SaveChanges();
                }
                else
                {
                    kpiTarget.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.KpiId);
                    DataContext.KpiTargets.Add(kpiTarget);
                    DataContext.SaveChanges();
                }
                response.Id = kpiTarget.Id;
                response.IsSuccess = true;
                response.Message = "KPI Target item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        /*public BaseResponse BatchUpdateKpiTargetss(BatchUpdateTargetRequest request)
        {
            var response = new BaseResponse();
            try
            {
                int i = 0;
                foreach (var item in request.BatchUpdateKpiTargetItemRequest)
                {
                    var kpiTarget = item.MapTo<KpiTarget>();
                    var exist = DataContext.KpiTargets.FirstOrDefault(x => x.Kpi.Id == item.KpiId && x.PeriodeType == item.PeriodeType && x.Periode == item.Periode && x.Value == item.Value && x.Remark == item.Remark);
                    //skip no change value
                    if (exist != null)
                    {
                        continue;
                    }
                    var attachedEntity = DataContext.KpiTargets.FirstOrDefault(x => x.Kpi.Id == item.KpiId && x.PeriodeType == item.PeriodeType && x.Periode == item.Periode);
                    if (attachedEntity != null)
                    {
                        kpiTarget.Id = attachedEntity.Id;
                    }
                    //jika tidak ada perubahan di skip aja
                    //if (existing.Value.Equals(item.Value) && existing.Periode.Equals(item.Periode) && existing.Kpi.Id.Equals(item.KpiId) && existing.PeriodeType.Equals(item.PeriodeType)) {
                    //    break;
                    //}
                    if (kpiTarget.Id != 0)
                    {
                        //var attachedEntity = DataContext.KpiAchievements.Find(item.Id);
                        if (attachedEntity != null && DataContext.Entry(attachedEntity).State != EntityState.Detached)
                        {
                            DataContext.Entry(attachedEntity).State = EntityState.Detached;
                        }
                        DataContext.KpiTargets.Attach(kpiTarget);
                        DataContext.Entry(kpiTarget).State = EntityState.Modified;
                    }
                    else
                    {
                        kpiTarget.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == item.KpiId);
                        DataContext.KpiTargets.Add(kpiTarget);
                    }
                    i++;
                }
                DataContext.SaveChanges();
                response.IsSuccess = true;
                if (i > 0)
                {
                    response.Message = string.Format("{0}  KPI Target items has been updated successfully", i.ToString());
                }
                else
                {
                    response.Message = "File Successfully Parsed, but no data changed!";
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
        }*/

        public BaseResponse BatchUpdateKpiTargetss(BatchUpdateTargetRequest request)
        {
            var response = new BaseResponse();
            try
            {
                int deletedCounter = 0;
                int updatedCounter = 0;
                int addedCounter = 0;
                int skippedCounter = 0;
                foreach (var item in request.BatchUpdateKpiTargetItemRequest)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        var existedKpiTarget =
                            DataContext.KpiTargets.FirstOrDefault(
                                x =>
                                x.Kpi.Id == item.KpiId && x.PeriodeType == item.PeriodeType && x.Periode == item.Periode);


                        if (existedKpiTarget != null)
                        {
                            if (item.Value.Equals("-") || item.Value.ToLowerInvariant().Equals("null"))
                            {
                                DataContext.KpiTargets.Remove(existedKpiTarget);
                                deletedCounter++;
                            }
                            else
                            {
                                if (existedKpiTarget.Value.Equals(item.RealValue))
                                {
                                    skippedCounter++;
                                }
                                else
                                {
                                    existedKpiTarget.Value = item.RealValue;
                                    DataContext.Entry(existedKpiTarget).State = EntityState.Modified;
                                    updatedCounter++;
                                }
                            }
                        }
                        else
                        {
                            var kpiTarget = item.MapTo<KpiTarget>();
                            if (kpiTarget.Value.HasValue)
                            {
                                kpiTarget.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == item.KpiId);
                                DataContext.KpiTargets.Add(kpiTarget);
                                addedCounter++;
                            }
                            else
                            {
                                skippedCounter++;
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

        public GetKpiTargetItemResponse GetKpiTarget(int kpiId, DateTime date, RangeFilter rangeFilter)
        {
            var response = new GetKpiTargetItemResponse();
            try
            {
                switch (rangeFilter)
                {
                    case RangeFilter.CurrentDay:
                    case RangeFilter.CurrentMonth:
                    case RangeFilter.CurrentYear:
                    case RangeFilter.MTD:
                    case RangeFilter.YTD:
                    case RangeFilter.AllExistingYears:
                        {
                            var kpi = DataContext.Kpis
                                .Include(x => x.Measurement)
                                .Single(x => x.Id == kpiId);

                            return GetKpiTarget(kpi.Id, date, rangeFilter, kpi.YtdFormula);
                        }
                }

            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }


            return response;
        }

        public GetKpiTargetItemResponse GetKpiTarget(int kpiId, DateTime date, RangeFilter rangeFilter, YtdFormula ytdFormula)
        {
            var response = new GetKpiTargetItemResponse();
            try
            {
                switch (rangeFilter)
                {
                    case RangeFilter.CurrentDay:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiTargets.Include(x => x.Kpi).FirstOrDefault(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Daily && x.Periode == date);
                            var kpiResponse = new GetKpiTargetItemResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiTargetItemResponse
                            {
                                Value = (data != null) ? data.Value : null,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }

                    case RangeFilter.CurrentMonth:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiTargets.Include(x => x.Kpi).FirstOrDefault(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Monthly && x.Periode.Month == date.Month && x.Periode.Year == date.Year);
                            var kpiResponse = new GetKpiTargetItemResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiTargetItemResponse
                            {
                                Value = (data != null) ? data.Value : null,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }

                    case RangeFilter.CurrentYear:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiTargets.Include(x => x.Kpi).FirstOrDefault(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Yearly && x.Periode.Year == date.Year);
                            var kpiResponse = new GetKpiTargetItemResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiTargetItemResponse
                            {
                                Value = (data != null) ? data.Value : null,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }

                    case RangeFilter.MTD:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiTargets.Include(x => x.Kpi)
                                .Where(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Daily &&
                                    (x.Periode.Day >= 1 && x.Periode.Day <= date.Day && x.Periode.Month == date.Month && x.Periode.Year == date.Year)).AsQueryable();
                            double? kpiAchievement = ytdFormula == YtdFormula.Average ? data.Average(x => x.Value) : data.Sum(x => x.Value);
                            var kpiResponse = new GetKpiTargetItemResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiTargetItemResponse
                            {
                                Value = kpiAchievement,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }

                    case RangeFilter.YTD:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiTargets.Include(x => x.Kpi)
                                    .Where(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Monthly && x.Value.HasValue &&
                                    (x.Periode.Month >= 1 && x.Periode.Month <= date.Month && x.Periode.Year == date.Year)).AsQueryable();
                            double? kpiAchievement = ytdFormula == YtdFormula.Average ? data.Average(x => x.Value) : data.Sum(x => x.Value);
                            var kpiResponse = new GetKpiTargetItemResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiTargetItemResponse
                            {
                                Value = kpiAchievement,
                                Kpi = kpiResponse,
                                IsSuccess = true
                            };
                        }

                    case RangeFilter.AllExistingYears:
                        {
                            var kpi = DataContext.Kpis.Include(x => x.Measurement).Single(x => x.Id == kpiId);
                            var data = DataContext.KpiTargets.Include(x => x.Kpi)
                                    .Where(x => x.Kpi.Id == kpiId && x.PeriodeType == PeriodeType.Yearly && x.Value.HasValue &&
                                    (x.Periode.Year >= 2011 && x.Periode.Year <= date.Year)).AsQueryable();
                            double? kpiAchievement = ytdFormula == YtdFormula.Average ? data.Average(x => x.Value) : data.Sum(x => x.Value);
                            var kpiResponse = new GetKpiTargetItemResponse.KpiResponse
                            {
                                Id = kpi.Id,
                                Measurement = kpi.Measurement.Name,
                                Name = kpi.Name,
                                Remark = kpi.Remark,
                            };

                            return new GetKpiTargetItemResponse
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

        public AllKpiTargetsResponse GetAllKpiTargetByRole(GetKpiTargetsConfigurationRequest request)
        {
            var response = new AllKpiTargetsResponse();
            try
            {
                var kpiTargets = DataContext.Kpis
                                            .Include(x => x.Measurement)
                                            .Include(x => x.Type)
                                            .Include(x => x.RoleGroup)
                                            .AsEnumerable()
                                            .OrderBy(x => x.Order)
                                            .Where(x => x.RoleGroup.Id == request.RoleGroupId)
                                            .GroupBy(x => x.RoleGroup).ToDictionary(x => x.Key);
                foreach (var item in kpiTargets)
                {
                    var kpis = new List<AllKpiTargetsResponse.Kpi>();
                    foreach (var val in item.Value)
                    {
                        kpis.Add(val.MapTo<AllKpiTargetsResponse.Kpi>());
                    }

                    response.RoleGroups.Add(new AllKpiTargetsResponse.RoleGroup
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

        public UpdateKpiTargetItemResponse UpdateOriginalData(SaveKpiTargetRequest request)
        {
            var response = new UpdateKpiTargetItemResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var kpiTarget = request.MapTo<KpiTarget>();
                var user = DataContext.Users.First(x => x.Id == request.UserId);
                if (request.Id != 0)
                {
                    if ((string.IsNullOrEmpty(request.Value) && request.Remark == null) || request.Value == "-" || (!string.IsNullOrEmpty(request.Value) && request.Value.ToLowerInvariant() == "null"))
                    {
                        kpiTarget = DataContext.KpiTargets.Single(x => x.Id == request.Id);
                        DataContext.KpiTargets.Remove(kpiTarget);
                    }
                    else
                    {
                        kpiTarget = DataContext.KpiTargets
                                          .Include(x => x.Kpi)
                                          .Include(x => x.UpdatedBy)
                                          .Single(x => x.Id == request.Id);
                        if (request.Value != null)
                        {
                            kpiTarget.Value = request.RealValue;
                        }
                        if (request.Remark != null)
                        {
                            kpiTarget.Remark = request.Remark;
                        }
                        //request.MapPropertiesToInstance<KpiAchievement>(kpiAchievement);
                        kpiTarget.UpdatedBy = user;
                        kpiTarget.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                    }

                }
                else
                {
                    var exist = DataContext.KpiTargets.FirstOrDefault(x => x.Kpi.Id == request.KpiId && x.PeriodeType == request.PeriodeType && x.Periode == request.Periode);
                    if (exist != null)
                    {
                        if (request.Remark != null || !string.IsNullOrEmpty(request.Remark))
                        {
                            exist.Remark = request.Remark;
                        }
                        if (!string.IsNullOrEmpty(request.Value) && request.Value.ToLowerInvariant() != "null" && request.Value != "-")
                        {
                            exist.Value = double.Parse(request.Value);
                        }
                        exist.UpdatedBy = user;
                        exist.UpdatedDate = DateTime.Now;
                        kpiTarget = exist;
                    }
                    else
                    {
                        kpiTarget.CreatedBy = user;
                        kpiTarget.UpdatedBy = user;
                        kpiTarget.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.KpiId);
                        DataContext.KpiTargets.Add(kpiTarget);
                    }

                    /*
                    if (((string.IsNullOrEmpty(request.Value) && request.Remark == null) || request.Value == "-" ||
                         (!string.IsNullOrEmpty(request.Value) && request.Value.ToLowerInvariant() == "null")) && request.Id == 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "You can not update this item because it is not existed";
                        return response;
                    }
                    else {
                        kpiTarget.CreatedBy = user;
                        kpiTarget.UpdatedBy = user;
                        kpiTarget.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.KpiId);
                        DataContext.KpiTargets.Add(kpiTarget);
                    }
                    */
                }
                DataContext.SaveChanges(action);
                response.Id = kpiTarget.Id;
                response.IsSuccess = true;
                response.Message = "KPI Target item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }
            return response;
        }

        public GetKpiTargetsResponse GetKpiTargets(int[] kpiIds, DateTime? start, DateTime? end, string periodeType)
        {
            PeriodeType pType = (PeriodeType)Enum.Parse(typeof(PeriodeType), periodeType);
            var response = new GetKpiTargetsResponse();
            try
            {
                var kpiTarget = DataContext.KpiTargets
                    .Include(x => x.Kpi.Measurement)
                    .OrderBy(x => x.Kpi.Order)
                    .Where(x => kpiIds.Contains(x.Kpi.Id) && x.PeriodeType == pType && x.Periode >= start.Value && x.Periode <= end.Value)
                    .ToList();
                if (kpiTarget.Count > 0)
                {
                    foreach (var item in kpiTarget)
                    {
                        response.KpiTargets.Add(item.MapTo<GetKpiTargetsResponse.KpiTarget>());
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

        public GetKpiTargetItemResponse GetKpiTarget(int kpiId, DateTime date, PeriodeType periodeType)
        {
            var response = new GetKpiTargetItemResponse();
            
            var kpiTarget = DataContext.KpiTargets
                .Include(x => x.Kpi)
                .Where(x => x.PeriodeType == periodeType &&
                         x.Periode == date && x.Kpi.Id == kpiId).FirstOrDefault();

            if(kpiTarget != null)
            {
                response.Kpi = new GetKpiTargetItemResponse.KpiResponse
                {
                    Id = kpiTarget.Kpi.Id,
                    Name = kpiTarget.Kpi.Name
                };
                response.Value = kpiTarget.Value;
                response.Periode = kpiTarget.Periode;
                response.PeriodeType = kpiTarget.PeriodeType;
            }

            return response;

        }
    }
}