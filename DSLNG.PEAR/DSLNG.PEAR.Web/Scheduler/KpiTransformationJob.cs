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
using System.Web.Hosting;

namespace DSLNG.PEAR.Web.Scheduler
{
    public class KpiTransformationJob : IKpiTransformationJob, IRegisteredObject
    {
        private readonly object _lock = new object();

        private bool _shuttingDown;

        public KpiTransformationJob()
        {
            HostingEnvironment.RegisterObject(this);
        }

        public void Process(SaveKpiTransformationScheduleResponse kpiTransformationSchedule)
        {
            try
            {
                lock (_lock)
                {
                    if (_shuttingDown)
                        return;

                    Execute(kpiTransformationSchedule);
                }
            }
            finally
            {
                // Always unregister the job when done.
                HostingEnvironment.UnregisterObject(this);
            }
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }

        private void Execute(SaveKpiTransformationScheduleResponse kpiTransformationSchedule)
        {
            var action = new DSLNG.PEAR.Data.Entities.BaseAction
            {
                UserId = kpiTransformationSchedule.UserId,
                ControllerName = kpiTransformationSchedule.ControllerName,
                ActionName = kpiTransformationSchedule.ActionName
            };
            var kpiPattern = @"k(\d+)";
            JobManager.AddJob(() =>
            {
                var complete = true;
                using (var dataContext = new DataContext())
                {
                    var kpiAchievementService = new KpiAchievementService(dataContext);
                    var logService = new KpiTransformationLogService(dataContext);
                    var kpiTransformationScheduleService = new KpiTransformationScheduleService(dataContext);
                    var kpiService = new KpiService(dataContext);
                    #region loop date
                    foreach (var kpi in kpiTransformationSchedule.SelectedKpis)
                    {
                        for (var date = kpiTransformationSchedule.Start; date <= kpiTransformationSchedule.End; date = Increment(kpiTransformationSchedule, date))
                        {

                            try
                            {
                                var kpiTransformed = kpi.CustomFormula;
                                var ytdTransformed = kpi.CustomFormula;
                                var mtdTransformed = kpi.CustomFormula;
                                var itdTransformed = kpi.CustomFormula;
                                var existingKpiActual = kpiAchievementService.GetKpiAchievement(kpi.Id, date, kpiTransformationSchedule.PeriodeType);
                                // exception. could not be run because data dependency not found before Sept 30, 2016
                                // di remark kembali karena permintaan tanggal 25 Januari 2017, see Telegram 
                                //if ((kpi.Id == 37 || kpi.Id == 8) && date < new DateTime(2016, 09, 30))
                                //{
                                //    continue;
                                //}
                                // pilih yang methodnya formula
                                if (kpi.MethodId == 1)
                                {
                                    if (string.IsNullOrEmpty(kpi.CustomFormula))
                                    {
                                        //log here for dependency error
                                        var logRequest = new SaveKpiTransformationLogRequest
                                        {
                                            KpiId = kpi.Id,
                                            KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                            PeriodeType = kpiTransformationSchedule.PeriodeType,
                                            Periode = date,
                                            Status = KpiTransformationStatus.Error,
                                            MethodId = kpi.MethodId,
                                            Notes = "Method input is formula but the formula is not defined"
                                        };
                                        logService.Save(logRequest);
                                        continue;
                                    }
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
                                            kpiTransformed = Regex.Replace(kpiTransformed, "k" + g.Value, (relatedKpiActual.Value % 1) == 0 ? relatedKpiActual.Value.ToString() + ".00" : relatedKpiActual.Value.ToString(), RegexOptions.IgnoreCase);
                                            if (kpi.YtdFormula == YtdFormula.Custom)
                                            {
                                                if ((relatedKpiActual.Mtd.HasValue && relatedKpiActual.Ytd.HasValue && relatedKpiActual.Itd.HasValue && kpiTransformationSchedule.PeriodeType == PeriodeType.Daily)
                                                || (relatedKpiActual.Ytd.HasValue && relatedKpiActual.Itd.HasValue && kpiTransformationSchedule.PeriodeType == PeriodeType.Monthly)
                                                || (relatedKpiActual.Itd.HasValue && kpiTransformationSchedule.PeriodeType == PeriodeType.Yearly))
                                                {
                                                    switch (kpiTransformationSchedule.PeriodeType)
                                                    {
                                                        case PeriodeType.Daily:
                                                            mtdTransformed = Regex.Replace(mtdTransformed, "k" + g.Value, (relatedKpiActual.Mtd % 1) == 0 ? relatedKpiActual.Mtd.ToString() + ".00" : relatedKpiActual.Mtd.ToString(), RegexOptions.IgnoreCase);
                                                            ytdTransformed = Regex.Replace(ytdTransformed, "k" + g.Value, (relatedKpiActual.Ytd % 1) == 0 ? relatedKpiActual.Ytd.ToString() + ".00" : relatedKpiActual.Ytd.ToString(), RegexOptions.IgnoreCase);
                                                            itdTransformed = Regex.Replace(itdTransformed, "k" + g.Value, (relatedKpiActual.Itd % 1) == 0 ? relatedKpiActual.Itd.ToString() + ".00" : relatedKpiActual.Itd.ToString(), RegexOptions.IgnoreCase);
                                                            break;
                                                        case PeriodeType.Monthly:
                                                            ytdTransformed = Regex.Replace(ytdTransformed, "k" + g.Value, (relatedKpiActual.Ytd % 1) == 0 ? relatedKpiActual.Ytd.ToString() + ".00" : relatedKpiActual.Ytd.ToString(), RegexOptions.IgnoreCase);
                                                            itdTransformed = Regex.Replace(itdTransformed, "k" + g.Value, (relatedKpiActual.Itd % 1) == 0 ? relatedKpiActual.Itd.ToString() + ".00" : relatedKpiActual.Itd.ToString(), RegexOptions.IgnoreCase);
                                                            break;
                                                        case PeriodeType.Yearly:
                                                            itdTransformed = Regex.Replace(itdTransformed, "k" + g.Value, (relatedKpiActual.Itd % 1) == 0 ? relatedKpiActual.Itd.ToString() + ".00" : relatedKpiActual.Itd.ToString(), RegexOptions.IgnoreCase);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                                    //log here for dependency error
                                                    var logRequest = new SaveKpiTransformationLogRequest
                                                    {
                                                        KpiId = kpi.Id,
                                                        KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                        PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                        Periode = date,
                                                        Status = KpiTransformationStatus.Error,
                                                        MethodId = kpi.MethodId,
                                                        Notes = "The aggregation for <strong>" + relatedKpiActual.Kpi.Name + " (" + relatedKpiActual.Kpi.Measurement + ")</strong> has not been proceed"
                                                    };
                                                    logService.Save(logRequest);
                                                    meetRequirements = false;
                                                    complete = false;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            var relatedKpi = kpiService.GetBy(new Services.Requests.Kpi.GetKpiRequest { Id = relatedKpiId });
                                            var logRequest = new SaveKpiTransformationLogRequest
                                            {
                                                KpiId = kpi.Id,
                                                KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                Periode = date,
                                                Status = KpiTransformationStatus.Error,
                                                MethodId = kpi.MethodId,
                                                Notes = "Kpi <strong>" + relatedKpi.Name + " (" + relatedKpi.Measurement.Name + ")</strong> has no value for this periode of time"
                                            };
                                            logService.Save(logRequest);
                                            meetRequirements = false;
                                            complete = false;
                                        }
                                        m = m.NextMatch();
                                    }
                                    if (kpi.YtdFormula == YtdFormula.Custom)
                                    {
                                        if (meetRequirements)
                                        {

                                            var kpiActualRequest = new UpdateKpiAchievementItemRequest
                                            {
                                                Id = existingKpiActual.IsSuccess ? existingKpiActual.Id : 0,
                                                KpiId = kpi.Id,
                                                Periode = date,
                                                PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                Value = null,
                                                UserId = kpiTransformationSchedule.UserId,
                                                ControllerName = action.ControllerName,
                                                ActionName = action.ActionName
                                            };

                                            if (kpiTransformed != kpi.CustomFormula)
                                            {
                                                var val = new Expression(kpiTransformed).Evaluate().ToString();
                                                if (!Double.IsInfinity(double.Parse(val)) && !Double.IsNaN(double.Parse(val)))
                                                {
                                                    kpiActualRequest.Value = val; //new Expression(kpiTransformed).Evaluate().ToString();
                                                }
                                                else
                                                {
                                                    var logRequest = new SaveKpiTransformationLogRequest
                                                    {
                                                        KpiId = kpi.Id,
                                                        KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                        PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                        Periode = date,
                                                        Status = KpiTransformationStatus.Error,
                                                        MethodId = kpi.MethodId,
                                                        Notes = "Infinite Result",
                                                        NeedCleanRowWhenError = true
                                                    };
                                                    logService.Save(logRequest);
                                                    complete = false;
                                                    continue;
                                                }
                                            }
                                            if (mtdTransformed != kpi.CustomFormula)
                                            {
                                                var val = double.Parse(new Expression(mtdTransformed).Evaluate().ToString());
                                                if (!Double.IsInfinity(val) && !Double.IsNaN(val))
                                                {
                                                    kpiActualRequest.Mtd = val;
                                                }
                                                else
                                                {
                                                    var logRequest = new SaveKpiTransformationLogRequest
                                                    {
                                                        KpiId = kpi.Id,
                                                        KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                        PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                        Periode = date,
                                                        Status = KpiTransformationStatus.Error,
                                                        MethodId = kpi.MethodId,
                                                        Notes = "Infinite Result",
                                                        NeedCleanRowWhenError = true
                                                    };
                                                    logService.Save(logRequest);
                                                    complete = false;
                                                    continue;
                                                }
                                            }
                                            if (ytdTransformed != kpi.CustomFormula)
                                            {
                                                var ytd = double.Parse(new Expression(ytdTransformed).Evaluate().ToString());
                                                if (!Double.IsInfinity(ytd) && !Double.IsNaN(ytd))
                                                {
                                                    kpiActualRequest.Ytd = ytd;
                                                }
                                                else
                                                {
                                                    var logRequest = new SaveKpiTransformationLogRequest
                                                    {
                                                        KpiId = kpi.Id,
                                                        KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                        PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                        Periode = date,
                                                        Status = KpiTransformationStatus.Error,
                                                        MethodId = kpi.MethodId,
                                                        Notes = "Infinite Result",
                                                        NeedCleanRowWhenError = true
                                                    };
                                                    logService.Save(logRequest);
                                                    complete = false;
                                                    continue;
                                                }
                                            }
                                            if (itdTransformed != kpi.CustomFormula)
                                            {
                                                var itd = double.Parse(new Expression(itdTransformed).Evaluate().ToString());
                                                if (!Double.IsInfinity(itd) && !Double.IsNaN(itd))
                                                {
                                                    kpiActualRequest.Itd = itd;
                                                }
                                                else
                                                {
                                                    var logRequest = new SaveKpiTransformationLogRequest
                                                    {
                                                        KpiId = kpi.Id,
                                                        KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                        PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                        Periode = date,
                                                        Status = KpiTransformationStatus.Error,
                                                        MethodId = kpi.MethodId,
                                                        Notes = "Infinite Result",
                                                        NeedCleanRowWhenError = true
                                                    };
                                                    logService.Save(logRequest);
                                                    complete = false;
                                                    continue;
                                                }
                                            }
                                            kpiActualRequest.UpdateDeviation = true;
                                            var resp = kpiAchievementService.UpdateKpiAchievementItem(kpiActualRequest);
                                            if (resp.IsSuccess)
                                            {
                                                var thatYear = new DateTime(date.Year, 1, 1);
                                                var existingYearKpiActual = kpiAchievementService.GetKpiAchievement(kpi.Id, thatYear, PeriodeType.Yearly);
                                                var kpiactualYearlyRequest = new UpdateKpiAchievementItemRequest
                                                {
                                                    Id = existingYearKpiActual.IsSuccess ? existingYearKpiActual.Id : 0,
                                                    KpiId = kpi.Id,
                                                    Periode = thatYear,
                                                    PeriodeType = PeriodeType.Yearly,
                                                    Value = kpiActualRequest.Ytd.ToString(),
                                                    UserId = kpiTransformationSchedule.UserId,
                                                    Itd = kpiActualRequest.Itd,
                                                    ControllerName = action.ControllerName,
                                                    ActionName = action.ActionName
                                                };
                                                switch (kpiTransformationSchedule.PeriodeType)
                                                {
                                                    case PeriodeType.Daily:
                                                        var theDate = new DateTime(date.Year, date.Month, 1);
                                                        var existingMonthlyKpiActual = kpiAchievementService.GetKpiAchievement(kpi.Id, theDate, PeriodeType.Monthly);

                                                        var kpiActualMonthlyRequest = new UpdateKpiAchievementItemRequest
                                                        {
                                                            Id = existingMonthlyKpiActual.IsSuccess ? existingMonthlyKpiActual.Id : 0,
                                                            KpiId = kpi.Id,
                                                            Periode = theDate,
                                                            PeriodeType = PeriodeType.Monthly,
                                                            Value = kpiActualRequest.Mtd.ToString(),
                                                            UserId = kpiTransformationSchedule.UserId,
                                                            Ytd = kpiActualRequest.Ytd,
                                                            Itd = kpiActualRequest.Itd,
                                                            ControllerName = action.ControllerName,
                                                            ActionName = action.ActionName
                                                        };
                                                        kpiAchievementService.UpdateKpiAchievementItem(kpiActualMonthlyRequest);
                                                        //kpiAchievementService.UpdateKpiAchievementItem(kpi.Id, PeriodeType.Monthly, new DateTime(date.Year, date.Month, 1), kpiActualRequest.Mtd, kpiTransformationSchedule.UserId);
                                                        //kpiAchievementService.UpdateKpiAchievementItem(kpi.Id, PeriodeType.Yearly, new DateTime(date.Year, 1, 1), kpiActualRequest.Ytd, kpiTransformationSchedule.UserId);
                                                        kpiAchievementService.UpdateKpiAchievementItem(kpiactualYearlyRequest);
                                                        break;
                                                    case PeriodeType.Monthly:
                                                        kpiAchievementService.UpdateKpiAchievementItem(kpiactualYearlyRequest);
                                                        //kpiAchievementService.UpdateKpiAchievementItem(kpi.Id, PeriodeType.Yearly, new DateTime(date.Year, 1, 1), kpiActualRequest.Ytd, kpiTransformationSchedule.UserId);
                                                        break;
                                                    default:
                                                        break;
                                                }

                                                var logRequest = new SaveKpiTransformationLogRequest
                                                {
                                                    KpiId = kpi.Id,
                                                    KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                    PeriodeType = kpiTransformationSchedule.PeriodeType,
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
                                                    PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                    Periode = date,
                                                    Status = KpiTransformationStatus.Error,
                                                    MethodId = kpi.MethodId,
                                                    Notes = resp.Message
                                                };
                                                logService.Save(logRequest);
                                                complete = false;
                                            }

                                        }

                                    }
                                    else
                                    {
                                        if (meetRequirements)
                                        {
                                             var request = new UpdateKpiAchievementItemRequest
                                            {
                                                Periode = date,
                                                PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                Id = existingKpiActual.IsSuccess ? existingKpiActual.Id : 0,
                                                KpiId = kpi.Id,
                                                UserId = kpiTransformationSchedule.UserId,
                                                ControllerName = action.ControllerName,
                                                ActionName = action.ActionName
                                            };
                                            var val = new Expression(kpiTransformed).Evaluate().ToString();
                                         
                                            if (!Double.IsInfinity(double.Parse(val)) && !Double.IsNaN(double.Parse(val)))
                                            {
                                                request.Value = val; //new Expression(kpiTransformed).Evaluate().ToString();
                                            }
                                            else
                                            {
                                                var logRequest = new SaveKpiTransformationLogRequest
                                                {
                                                    KpiId = kpi.Id,
                                                    KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                    PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                    Periode = date,
                                                    Status = KpiTransformationStatus.Error,
                                                    MethodId = kpi.MethodId,
                                                    Notes = "Infinite Result",
                                                    NeedCleanRowWhenError = true
                                                };
                                                logService.Save(logRequest);
                                                complete = false;
                                                continue;
                                            }

                                            var resp = kpiAchievementService.UpdateOriginalData(request);

                                            if (resp.IsSuccess)
                                            {
                                                //if(kpi.YtdFormula == YtdFormula.NaN)
                                                //{
                                                //    switch (kpiTransformationSchedule.PeriodeType)
                                                //    {
                                                //        case PeriodeType.Daily:
                                                //            kpiAchievementService.UpdateKpiAchievementItem(kpi.Id, PeriodeType.Monthly, new DateTime(date.Year, date.Month, 1), double.Parse(request.Value), kpiTransformationSchedule.UserId);
                                                //            kpiAchievementService.UpdateKpiAchievementItem(kpi.Id, PeriodeType.Yearly, new DateTime(date.Year, 1, 1), double.Parse(request.Value), kpiTransformationSchedule.UserId);
                                                //            break;
                                                //        case PeriodeType.Monthly:
                                                //            kpiAchievementService.UpdateKpiAchievementItem(kpi.Id, PeriodeType.Yearly, new DateTime(date.Year, 1, 1), double.Parse(request.Value), kpiTransformationSchedule.UserId);
                                                //            break;
                                                //        default:
                                                //            break;
                                                //    }
                                                //}
                                                var logRequest = new SaveKpiTransformationLogRequest
                                                {
                                                    KpiId = kpi.Id,
                                                    KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                                    PeriodeType = kpiTransformationSchedule.PeriodeType,
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
                                                    PeriodeType = kpiTransformationSchedule.PeriodeType,
                                                    Periode = date,
                                                    Status = KpiTransformationStatus.Error,
                                                    MethodId = kpi.MethodId,
                                                    Notes = resp.Message
                                                };
                                                logService.Save(logRequest);
                                                complete = false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var request = new UpdateKpiAchievementItemRequest
                                    {
                                        Periode = date,
                                        PeriodeType = kpiTransformationSchedule.PeriodeType,
                                        Id = existingKpiActual.IsSuccess ? existingKpiActual.Id : 0,
                                        KpiId = kpi.Id,
                                        UserId = kpiTransformationSchedule.UserId,
                                        Value = existingKpiActual.Value.ToString(),
                                        Remark = existingKpiActual.Remark,
                                        ControllerName = action.ControllerName,
                                        ActionName = action.ActionName
                                    };
                                    var resp = kpiAchievementService.UpdateOriginalData(request);
                                    if (resp.IsSuccess)
                                    {
                                        if (date == kpiTransformationSchedule.End)
                                        {

                                        }
                                        var logRequest = new SaveKpiTransformationLogRequest
                                        {
                                            KpiId = kpi.Id,
                                            KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                            PeriodeType = kpiTransformationSchedule.PeriodeType,
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
                                            PeriodeType = kpiTransformationSchedule.PeriodeType,
                                            Periode = date,
                                            Status = KpiTransformationStatus.Error,
                                            MethodId = kpi.MethodId,
                                            Notes = resp.Message
                                        };
                                        logService.Save(logRequest);
                                        complete = false;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                var logRequest = new SaveKpiTransformationLogRequest
                                {
                                    KpiId = kpi.Id,
                                    KpiTransformationScheduleId = kpiTransformationSchedule.Id,
                                    PeriodeType = kpiTransformationSchedule.PeriodeType,
                                    Periode = date,
                                    Status = KpiTransformationStatus.Error,
                                    MethodId = kpi.MethodId,
                                    Notes = string.Format("Excception Message :{0}<br/>Inner Exception Message : {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "")
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
                    else
                    {
                        kpiTransformationScheduleService.UpdateStatus(kpiTransformationSchedule.Id, KpiTransformationStatus.Error);
                    }

                    #endregion
                }


            }, (s) => s.ToRunNow());
        }
        private DateTime Increment(SaveKpiTransformationScheduleResponse kpiTransformationSchedule, DateTime periode)
        {
            switch (kpiTransformationSchedule.PeriodeType)
            {
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
