using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSLNG.PEAR.Services.Responses.KpiTransformationSchedule;
using FluentScheduler;
using DSLNG.PEAR.Data.Enums;
using System.Text.RegularExpressions;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.KpiAchievement;
using NCalc;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services;
using DSLNG.PEAR.Services.Requests.KpiTransformationLog;

namespace DSLNG.PEAR.Web.Scheduler
{
    public class KpiTransformationJob : IKpiTransformationJob
    {
         public void Process(SaveKpiTransformationScheduleResponse kpiTransformationSchedule)
        {
               var kpiPattern = @"k(\d+)";
                JobManager.AddJob(() =>
                {
                    var complete = true;
                    using (var dataContext = new DataContext())
                    {
                        var kpiAchievementService = new KpiAchievementService(dataContext);
                        var logService = new KpiTransformationLogService(dataContext);
                        var kpiTransformationScheduleService = new KpiTransformationScheduleService(dataContext);
                        for (var date = kpiTransformationSchedule.Start; date <= kpiTransformationSchedule.End; date = Increment(kpiTransformationSchedule, date))
                        {
                            foreach (var kpi in kpiTransformationSchedule.SelectedKpis)
                            {
                                var kpiTransformed = kpi.CustomFormula;
                                var ytdTransformed = kpi.CustomFormula;
                                var mtdTransformed = kpi.CustomFormula;
                                var itdTransformed = kpi.CustomFormula;
                                var existingKpiActual = kpiAchievementService.GetKpiAchievement(kpi.Id, date, kpiTransformationSchedule.PeriodeType);
                                if (kpi.CustomFormula != null)
                                {
                                    Regex r = new Regex(kpiPattern, RegexOptions.IgnoreCase);
                                    Match m = r.Match(kpi.CustomFormula);
                                    var meetRequirements = true;
                                    while (m.Success)
                                    {
                                        Group g = m.Groups[1];
                                        var relatedKpiId = int.Parse(g.Value);
                                        var relatedKpiActual = kpiAchievementService.GetKpiAchievement(relatedKpiId, date, kpiTransformationSchedule.PeriodeType);
                                        if (relatedKpiActual.IsSuccess && relatedKpiActual.Value.HasValue)
                                        {
                                            kpiTransformed = Regex.Replace(kpiTransformed, "k" + g.Value, relatedKpiActual.Value.ToString(), RegexOptions.IgnoreCase);
                                            if (kpi.YtdFormula == YtdFormula.Custom && relatedKpiActual.Mtd.HasValue && relatedKpiActual.Ytd.HasValue && relatedKpiActual.Itd.HasValue)
                                            {
                                                mtdTransformed = Regex.Replace(mtdTransformed, "k" + g.Value, relatedKpiActual.Mtd.ToString(), RegexOptions.IgnoreCase);
                                                ytdTransformed = Regex.Replace(ytdTransformed, "k" + g.Value, relatedKpiActual.Ytd.ToString(), RegexOptions.IgnoreCase);
                                                itdTransformed = Regex.Replace(itdTransformed, "k" + g.Value, relatedKpiActual.Itd.ToString(), RegexOptions.IgnoreCase);
                                            }
                                            else {
                                                //log here for dependency error
                                                var logRequest = new SaveKpiTransformationLogRequest
                                                {
                                                    KpiId =kpi.Id,
                                                    KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                    Periode = date,
                                                    Status = KpiTransformationStatus.Error,
                                                    Notes = "The aggregation for <strong>" + relatedKpiActual.Kpi.Name + " (" + relatedKpiActual.Kpi.Measurement + ")</strong> has not been proceed"
                                                };
                                                logService.Save(logRequest);
                                                meetRequirements = false;
                                                complete = false;
                                            }
                                        }
                                        else {
                                            var logRequest = new SaveKpiTransformationLogRequest
                                            {
                                                KpiId = kpi.Id,
                                                KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                Periode = date,
                                                Status = KpiTransformationStatus.Error,
                                                Notes = "Kpi <strong>" + relatedKpiActual.Kpi.Name + " (" + relatedKpiActual.Kpi.Measurement + ")</strong> has no value for this periode of time"
                                            };
                                            logService.Save(logRequest);
                                            meetRequirements = false;
                                            complete = false;
                                        }
                                        m = m.NextMatch();
                                    }
                                    if (kpi.YtdFormula == YtdFormula.Custom && meetRequirements)
                                    {
                                        
                                        var kpiActualRequest = new UpdateKpiAchievementItemRequest
                                        {
                                            Id = existingKpiActual.IsSuccess ? existingKpiActual.Id : 0,
                                            KpiId = kpi.Id,
                                            Periode = date,
                                            PeriodeType = kpiTransformationSchedule.PeriodeType,
                                            Value = new Expression(kpiTransformed).Evaluate().ToString(),
                                            UserId = kpiTransformationSchedule.UserId
                                        };
                                        kpiActualRequest.Mtd = (double?)new Expression(mtdTransformed).Evaluate();
                                        kpiActualRequest.Ytd = (double?)new Expression(ytdTransformed).Evaluate();
                                        kpiActualRequest.Itd = (double?)new Expression(itdTransformed).Evaluate();
                                        var resp = kpiAchievementService.UpdateKpiAchievementItem(kpiActualRequest);
                                        if (resp.IsSuccess)
                                        {
                                            var logRequest = new SaveKpiTransformationLogRequest
                                            {
                                                KpiId = kpi.Id,
                                                KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                Periode = date,
                                                Status = KpiTransformationStatus.Complete,
                                            };
                                            logService.Save(logRequest);
                                        }
                                        else {
                                            var logRequest = new SaveKpiTransformationLogRequest
                                            {
                                                KpiId = kpi.Id,
                                                KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                Periode = date,
                                                Status = KpiTransformationStatus.Error,
                                                Notes = resp.Message
                                            };
                                            logService.Save(logRequest);
                                            complete = false;
                                        }
                                        
                                    }
                                    else {
                                        if (existingKpiActual.IsSuccess && existingKpiActual.Value.HasValue)
                                        {
                                            var request = new UpdateKpiAchievementItemRequest
                                            {
                                                Periode = date,
                                                PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                Id = existingKpiActual.IsSuccess ? existingKpiActual.Id : 0,
                                                KpiId = kpi.Id,
                                                UserId = kpiTransformationSchedule.UserId,
                                                Value = new Expression(kpiTransformed).Evaluate().ToString()
                                            };
                                            var resp = kpiAchievementService.UpdateOriginalData(request);
                                            if (resp.IsSuccess)
                                            {
                                                var logRequest = new SaveKpiTransformationLogRequest
                                                {
                                                    KpiId = kpi.Id,
                                                    KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                    Periode = date,
                                                    Status = KpiTransformationStatus.Complete,
                                                };
                                                logService.Save(logRequest);
                                            }
                                            else
                                            {
                                                var logRequest = new SaveKpiTransformationLogRequest
                                                {
                                                    KpiId = kpi.Id,
                                                    KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                    Periode = date,
                                                    Status = KpiTransformationStatus.Error,
                                                    Notes = resp.Message
                                                };
                                                logService.Save(logRequest);
                                                complete = false;
                                            }
                                        }
                                        else {
                                            var logRequest = new SaveKpiTransformationLogRequest
                                            {
                                                KpiId = kpi.Id,
                                                KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                Periode = date,
                                                Status = KpiTransformationStatus.Error,
                                                Notes = "Kpi <strong>" + existingKpiActual.Kpi.Name + " (" + existingKpiActual.Kpi.Measurement + ")</strong> has no value for this periode of time"
                                            };
                                            logService.Save(logRequest);
                                            meetRequirements = false;
                                            complete = false;
                                        }
                                    }
                                    
                                }
                                else {
                                    var request = new UpdateKpiAchievementItemRequest
                                    {
                                        Periode = date,
                                        PeriodeType = kpiTransformationSchedule.PeriodeType,
                                        Id = existingKpiActual.IsSuccess ? existingKpiActual.Id : 0,
                                        KpiId = kpi.Id,
                                        UserId = kpiTransformationSchedule.UserId,
                                        Value = existingKpiActual.Value.ToString(),
                                        Remark = existingKpiActual.Remark
                                    };
                                    var resp = kpiAchievementService.UpdateOriginalData(request);
                                    if (resp.IsSuccess)
                                    {
                                        var logRequest = new SaveKpiTransformationLogRequest
                                        {
                                            KpiId = kpi.Id,
                                            KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                            Periode = date,
                                            Status = KpiTransformationStatus.Complete,
                                        };
                                        logService.Save(logRequest);
                                    }
                                    else
                                    {
                                        var logRequest = new SaveKpiTransformationLogRequest
                                        {
                                            KpiId = kpi.Id,
                                            KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                            Periode = date,
                                            Status = KpiTransformationStatus.Error,
                                            Notes = resp.Message
                                        };
                                        logService.Save(logRequest);
                                        complete = false;
                                    }
                                }
                                
                            }
                            if (complete)
                            {
                                kpiTransformationScheduleService.UpdateStatus(kpiTransformationSchedule.Id, KpiTransformationStatus.Complete);
                            }
                            else {
                                kpiTransformationScheduleService.UpdateStatus(kpiTransformationSchedule.Id, KpiTransformationStatus.Error);
                            }
                        }
                    }

                }, (s) => s.ToRunNow());
        }
        private DateTime Increment(SaveKpiTransformationScheduleResponse kpiTransformationSchedule, DateTime periode)
        {
            switch (kpiTransformationSchedule.PeriodeType) {
                case PeriodeType.Yearly:
                    return periode.AddYears(1);
                case PeriodeType.Monthly:
                    return periode.AddMonths(1);
                default:
                    return periode.AddDays(1);
            }
        }
    }
}