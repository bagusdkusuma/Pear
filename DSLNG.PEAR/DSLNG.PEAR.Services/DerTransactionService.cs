using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DSLNG.PEAR.Services.Requests.DerTransaction;
using DSLNG.PEAR.Services.Responses.DerTransaction;
using DSLNG.PEAR.Data.Persistence;
using LinqKit;
using DSLNG.PEAR.Data.Entities.Der;
using DSLNG.PEAR.Data.Enums;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Services.Requests.Der;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Globalization;

namespace DSLNG.PEAR.Services
{
    public class DerTransactionService : BaseService, IDerTransactionService
    {
        public DerTransactionService(IDataContext dataContext) : base(dataContext) { }

        public GetDerLayoutItemsResponse GetDerLayoutItems(GetDerLayoutItemsRequest request)
        {
            var predicate = PredicateBuilder.False<DerLayoutItem>();
            foreach (var position in request.Positions)
            {
                var row = position.Row;
                var col = position.Column;
                var inner = PredicateBuilder.True<DerLayoutItem>();
                inner = inner.And(p => p.Row == row);
                inner = inner.And(p => p.Column == col);
                predicate = predicate.Or(inner);
            }
            var query = DataContext.DerLayoutItems.AsQueryable();
            if (request.DerLayoutItemTypes.Contains(DerLayoutItemType.Highlight))
            {
                query = query.Include(x => x.Highlight).Include(x => x.Highlight.SelectOption);
            }
            if (request.DerLayoutItemTypes.Contains(DerLayoutItemType.KpiInformations))
            {
                query = query.Include(x => x.KpiInformations).Include(x => x.KpiInformations.Select(y => y.Kpi));
            }
            return new GetDerLayoutItemsResponse
            {
                DerLayoutItems = query.Where(x => x.DerLayout.Id == request.LayoutId).AsExpandable()
                .Where(predicate).ToList().MapTo<GetDerLayoutItemsResponse.DerLayoutItem>()
            };
        }

        private List<Data.Entities.KpiAchievement> GetAchievements(IEnumerable<int> kpis, DateTime periode)
        {
            var response = new List<Data.Entities.KpiAchievement>();
            foreach (var kpi in kpis)
            {
                var daily = DataContext.KpiAchievements.OrderByDescending(x => x.Periode).Include(x => x.Kpi).Where(x => x.Kpi.Id == kpi && x.Periode <= periode && x.PeriodeType == PeriodeType.Daily).FirstOrDefault();
                if (daily != null)
                {
                    response.Add(daily);
                }
                var monthly = new Data.Entities.KpiAchievement();
                if (kpi == 385)
                {
                    DateTime prevMonth = periode.AddMonths(-1);
                    monthly = DataContext.KpiAchievements.OrderByDescending(x => x.Periode).Include(x => x.Kpi).Where(x => x.Kpi.Id == kpi && x.Periode <= prevMonth && x.PeriodeType == PeriodeType.Monthly).FirstOrDefault();
                }
                else
                {
                    monthly = DataContext.KpiAchievements.OrderByDescending(x => x.Periode).Include(x => x.Kpi).Where(x => x.Kpi.Id == kpi && x.Periode <= periode && x.PeriodeType == PeriodeType.Monthly).FirstOrDefault();
                }
                if (monthly != null)
                {
                    response.Add(monthly);
                }

                var yearly = DataContext.KpiAchievements.OrderByDescending(x => x.Periode).Include(x => x.Kpi).Where(x => x.Kpi.Id == kpi && x.Periode <= periode && x.PeriodeType == PeriodeType.Yearly).FirstOrDefault();
                if (yearly != null)
                {
                    response.Add(yearly);
                }
            }
            return response;
        }

        private List<Data.Entities.KpiTarget> GetTargets(IEnumerable<int> kpis, DateTime periode)
        {
            var response = new List<Data.Entities.KpiTarget>();
            foreach (var kpi in kpis)
            {
                var daily = DataContext.KpiTargets.Include(x => x.Kpi).Where(x => x.Kpi.Id == kpi && x.Periode == periode && x.PeriodeType == PeriodeType.Daily).FirstOrDefault();
                if (daily != null)
                {
                    response.Add(daily);
                }
                var monthly = new Data.Entities.KpiTarget();
                if (kpi == 385)
                {
                    DateTime prevMonth = periode.AddMonths(-1);
                    monthly = DataContext.KpiTargets.OrderByDescending(x => x.Periode).Include(x => x.Kpi).Where(x => x.Kpi.Id == kpi && x.Periode <= prevMonth && x.PeriodeType == PeriodeType.Monthly).FirstOrDefault();
                }
                else
                {
                    monthly = DataContext.KpiTargets.Include(x => x.Kpi).Where(x => x.Kpi.Id == kpi && x.Periode.Month == periode.Month && x.PeriodeType == PeriodeType.Monthly).FirstOrDefault();
                }
                if (monthly != null)
                {
                    response.Add(monthly);
                }

                var yearly = DataContext.KpiTargets.Include(x => x.Kpi).Where(x => x.Kpi.Id == kpi && x.Periode.Year == periode.Year && x.PeriodeType == PeriodeType.Yearly).FirstOrDefault();
                if (yearly != null)
                {
                    response.Add(yearly);
                }
            }
            return response;
        }

        public GetKpiInformationValuesResponse GetKpiInformationValues(GetKpiInformationValuesRequest request)
        {
            //var kpiIds = 
            //achievement section
            var kpiIdsForActual = request.ActualKpiIds;
            var previousDate = request.Date.AddDays(-1);
            var previousMonth = request.Date.AddMonths(-1);
            var previous2Month = request.Date.AddMonths(-2);
            var previousYear = request.Date.AddYears(-1);
            //var achievements = GetAchievements(kpiIdsForActual, request.Date);
            var achievements = DataContext.KpiAchievements.OrderByDescending(x=>x.Periode).Include(x => x.Kpi)
                .Where(x => kpiIdsForActual.Contains(x.Kpi.Id) &&
                (((x.Periode == request.Date || x.Periode == previousDate) && x.PeriodeType == PeriodeType.Daily) ||
                (x.PeriodeType == PeriodeType.Yearly && (x.Periode.Year == request.Date.Year || x.Periode.Year == previousYear.Year)) ||
                (x.PeriodeType == PeriodeType.Monthly && (x.Periode.Month == request.Date.Month && x.Periode.Year == request.Date.Year 
                || x.Periode.Month == previousMonth.Month && x.Periode.Year == previousMonth.Year 
                || x.Periode.Month == previous2Month.Month && x.Periode.Year == previous2Month.Year)))).ToList();
            var kpiIdsForTarget = request.TargetKpiIds;
            //var targets = GetTargets(kpiIdsForTarget, request.Date);
            var targets = DataContext.KpiTargets.Include(x => x.Kpi)
               .Where(x => kpiIdsForTarget.Contains(x.Kpi.Id) &&
               (((x.Periode == request.Date || x.Periode == previousDate) && x.PeriodeType == PeriodeType.Daily) ||
               (x.PeriodeType == PeriodeType.Yearly && x.Periode.Year == request.Date.Year) ||
               (x.PeriodeType == PeriodeType.Monthly && x.Periode.Month == request.Date.Month && x.Periode.Year == request.Date.Year
               || x.Periode.Month == previousMonth.Month && x.Periode.Year == previousMonth.Year 
               || x.Periode.Month == previous2Month.Month && x.Periode.Year == previous2Month.Year)))
               .OrderByDescending(x=>x.Periode).ToList();

            var response = new GetKpiInformationValuesResponse();
            foreach (var kpiId in kpiIdsForActual)
            {
                var kpiInformation = response.KpiInformations.FirstOrDefault(x => x.KpiId == kpiId);
                if (kpiInformation == null)
                {
                    kpiInformation = new GetKpiInformationValuesResponse.KpiInformation { KpiId = kpiId };
                    response.KpiInformations.Add(kpiInformation);
                }
            }
            #region actual
            if (achievements != null)
            {
                foreach (var actual in achievements)
                {
                    var kpiInformation = response.KpiInformations.FirstOrDefault(x => x.KpiId == actual.Kpi.Id);
                    //if (kpiInformation == null) {
                    //    kpiInformation = new GetKpiInformationValuesResponse.KpiInformation { KpiId = actual.Kpi.Id };
                    //    response.KpiInformations.Add(kpiInformation);
                    //}
                    //var actual = achievements.FirstOrDefault(x => x.Kpi.Id == achievement.Kpi.Id);
                    if (actual == null)
                    {
                        continue;
                    }
                    #region daily
                    if (actual.PeriodeType == PeriodeType.Daily)
                    {
                        if (kpiInformation.DailyActual == null)
                        {
                            var isTodayValue = actual.Periode == request.Date;
                            if (isTodayValue)
                            {
                                var prevActual = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == previousDate && x.PeriodeType == PeriodeType.Daily);
                                kpiInformation.DailyActual = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = actual.Periode,
                                    Value = actual.Value.HasValue ? actual.Value : null,
                                    Remark = string.IsNullOrEmpty(actual.Remark) ? (prevActual != null ? "prev--" + prevActual.Remark : actual.Remark) : actual.Remark,
                                    Id = actual.Id,
                                    Type = "now"
                                };
                            }
                            else
                            {
                                //var todayValue = achievements.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == request.Date && x.PeriodeType == PeriodeType.Daily);
                                var prevActual = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == previousDate && x.PeriodeType == PeriodeType.Daily);
                                var todayValue = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == request.Date && x.PeriodeType == PeriodeType.Daily);
                                if (todayValue != null)
                                {
                                    kpiInformation.DailyActual = new GetKpiInformationValuesResponse.KpiValue
                                    {
                                        Date = todayValue.Periode,
                                        Value = todayValue.Value ?? null,
                                        Remark = string.IsNullOrEmpty(todayValue.Remark) ? (prevActual != null ? "prev--" + prevActual.Remark : todayValue.Remark) : todayValue.Remark,
                                        Id = todayValue.Id,
                                        Type = "now"
                                    };

                                }
                                else
                                {
                                    //yesterday value selected
                                    kpiInformation.DailyActual = new GetKpiInformationValuesResponse.KpiValue
                                    {
                                        Date = actual.Periode,
                                        Value = actual.Value ?? null,
                                        Remark = actual.Remark,
                                        Type = "prev"
                                    };
                                    // check last value of this kpi
                                    //var lastValue = achievements.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == request.Date && x.PeriodeType == PeriodeType.Daily);
                                    //if (lastValue != null)
                                    //{
                                    //    kpiInformation.DailyActual = new GetKpiInformationValuesResponse.KpiValue
                                    //    {
                                    //        Date = lastValue.Periode,
                                    //        Value = lastValue.Value.HasValue ? lastValue.Value : null,
                                    //        Remark = lastValue.Remark,
                                    //        Id = lastValue.Id,
                                    //        Type = "prev"
                                    //    };

                                    //}
                                    //else
                                    //{
                                    //    //yesterday value selected
                                    //    kpiInformation.DailyActual = new GetKpiInformationValuesResponse.KpiValue
                                    //    {
                                    //        Date = actual.Periode,
                                    //        Value = actual.Value.HasValue ? actual.Value : null,
                                    //        Remark = actual.Remark,
                                    //        Type = "prev"
                                    //    };
                                    //}
                                }
                            }
                        }
                    }
                    #endregion
                    #region if monthly
                    if (actual.PeriodeType == PeriodeType.Monthly)
                    {
                        if (kpiInformation.MonthlyActual == null)
                        {
                            bool isCurrentMonthValue = false;
                            if (actual.Kpi.Id == 385)
                            {
                                var y = request.Date.Year;
                                if(request.Date.Month == 1)
                                {
                                    y = y - 1;
                                }
                                isCurrentMonthValue = actual.Periode.Month == request.Date.AddMonths(-1).Month && actual.Periode.Year == y && actual.PeriodeType == PeriodeType.Monthly;
                            }
                            else
                            {
                                isCurrentMonthValue = actual.Periode.Month == request.Date.Month && actual.Periode.Year == request.Date.Year && actual.PeriodeType == PeriodeType.Monthly;
                            }
                            //var isCurrentMonthValue = actual.Periode.Month == request.Date.Month && actual.Periode.Year == request.Date.Year && actual.PeriodeType == PeriodeType.Monthly;
                           
                            if (isCurrentMonthValue)
                            {
                                var prevActual = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == request.Date.AddMonths(-1) && x.PeriodeType == PeriodeType.Monthly);
                                kpiInformation.MonthlyActual = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = actual.Periode,
                                    Value = actual.Value.HasValue ? actual.Value : null,
                                    Remark = string.IsNullOrEmpty(actual.Remark) ? (prevActual != null ? "prev--" + prevActual.Remark : actual.Remark) : actual.Remark,
                                    Type = "now",
                                    Id = actual.Id
                                };
                            }
                            else
                            {
                                var currentMonthValue = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode.Month == request.Date.Month && x.Periode.Year == request.Date.Year && x.PeriodeType == PeriodeType.Monthly);
                                if (actual.Kpi.Id == 385)
                                {
                                    var y = request.Date.Year;
                                    if (request.Date.Month == 1)
                                    {
                                        y = y - 1;
                                    }
                                    currentMonthValue = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode.Month == request.Date.AddMonths(-1).Month && x.Periode.Year == y && x.PeriodeType == PeriodeType.Monthly);
                                }
                                    var prevActual = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == request.Date.AddMonths(-1) && x.PeriodeType == PeriodeType.Monthly);
                                if (currentMonthValue != null)
                                {
                                    kpiInformation.MonthlyActual = new GetKpiInformationValuesResponse.KpiValue
                                    {
                                        Date = currentMonthValue.Periode,
                                        Value = currentMonthValue.Value.HasValue ? currentMonthValue.Value : null,
                                        Remark = string.IsNullOrEmpty(currentMonthValue.Remark) ? (prevActual != null ? "prev--" + prevActual.Remark : currentMonthValue.Remark) : currentMonthValue.Remark,
                                        Id = currentMonthValue.Id,
                                        Type = "now"
                                    };
                                }
                                else
                                {
                                    kpiInformation.MonthlyActual = new GetKpiInformationValuesResponse.KpiValue
                                    {
                                        Date = actual.Periode,
                                        Value = actual.Value.HasValue ? actual.Value : null,
                                        Remark = actual.Remark,
                                        Type = "prev"
                                    };

                                    //var previousMonthValue = achievements.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode.Month <= request.Date.Month && x.Periode.Year == request.Date.Year && x.PeriodeType == PeriodeType.Monthly);
                                    //if (previousMonthValue != null)
                                    //{
                                    //    kpiInformation.MonthlyActual = new GetKpiInformationValuesResponse.KpiValue
                                    //    {
                                    //        Date = previousMonthValue.Periode,
                                    //        Value = previousMonthValue.Value.HasValue ? previousMonthValue.Value : null,
                                    //        Remark = previousMonthValue.Remark,
                                    //        Id = previousMonthValue.Id,
                                    //        Type = "prev"
                                    //    };
                                    //}
                                    //else
                                    //{
                                    //    kpiInformation.MonthlyActual = new GetKpiInformationValuesResponse.KpiValue
                                    //    {
                                    //        Date = actual.Periode,
                                    //        Value = actual.Value.HasValue ? actual.Value : null,
                                    //        Remark = actual.Remark,
                                    //        Type = "prev"
                                    //    };
                                    //}
                                }

                            }
                        }
                    }

                    #endregion
                    #region if yearly
                    if (actual.PeriodeType == PeriodeType.Yearly)
                    {
                        if (kpiInformation.YearlyActual == null)
                        {
                            var isCurrentYearValue = actual.Periode.Year == request.Date.Year && actual.PeriodeType == PeriodeType.Yearly;
                            if (isCurrentYearValue)
                            {
                                var prevActual = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == request.Date.AddYears(-1) && x.PeriodeType == PeriodeType.Yearly);
                                kpiInformation.YearlyActual = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = actual.Periode,
                                    Value = actual.Value.HasValue ? actual.Value : null,
                                    Remark = string.IsNullOrEmpty(actual.Remark) ? (prevActual != null ? "prev--" + prevActual.Remark : actual.Remark) : actual.Remark,
                                    Type = "now",
                                    Id = actual.Id
                                };
                            }
                            else
                            {
                                var currentYearValue = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode.Year == request.Date.Year && x.PeriodeType == PeriodeType.Yearly);
                                if (currentYearValue != null)
                                {
                                    var prevActual = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == request.Date.AddYears(-1) && x.PeriodeType == PeriodeType.Yearly);
                                    kpiInformation.YearlyActual = new GetKpiInformationValuesResponse.KpiValue
                                    {
                                        Date = currentYearValue.Periode,
                                        Value = currentYearValue.Value.HasValue ? currentYearValue.Value : null,
                                        Remark = string.IsNullOrEmpty(currentYearValue.Remark) ? (prevActual != null ? "prev--" + prevActual.Remark : currentYearValue.Remark) : currentYearValue.Remark,
                                        Id = currentYearValue.Id,
                                        Type = "now"
                                    };
                                }
                                else
                                {
                                    kpiInformation.YearlyActual = new GetKpiInformationValuesResponse.KpiValue
                                    {
                                        Date = actual.Periode,
                                        Value = actual.Value.HasValue ? actual.Value : null,
                                        Remark = actual.Remark,
                                        Type = "prev"
                                    };
                                }
                            }
                        }

                    }
                    #endregion
                }
            }
            #endregion
            #region target
            foreach (var kpiId in kpiIdsForTarget)
            {
                var kpiInformation = response.KpiInformations.FirstOrDefault(x => x.KpiId == kpiId);
                if (kpiInformation == null)
                {
                    kpiInformation = new GetKpiInformationValuesResponse.KpiInformation { KpiId = kpiId };
                    response.KpiInformations.Add(kpiInformation);
                }
                //kpiInformation = new GetKpiInformationValuesResponse.KpiInformation { KpiId = kpiId };
                //response.KpiInformations.Add(kpiInformation);
            }
            if (targets != null)
            {
                foreach (var target in targets)
                {
                    var kpiInformation = response.KpiInformations.FirstOrDefault(x => x.KpiId == target.Kpi.Id);
                    //if (kpiInformation == null)
                    //{
                    //    kpiInformation = new GetKpiInformationValuesResponse.KpiInformation { KpiId = target.Kpi.Id };
                    //    response.KpiInformations.Add(kpiInformation);
                    //}
                    //var target = targets.FirstOrDefault(x => x.Kpi.Id == kpiId);
                    if (target == null)
                    {
                        continue;
                    }
                    #region target-daily
                    if (target.PeriodeType == PeriodeType.Daily)
                    {
                        if (kpiInformation.DailyTarget == null)
                        {
                            var isTodayValue = target.Periode == request.Date;
                            if (isTodayValue)
                            {
                                var prevTarget = targets.FirstOrDefault(x => x.Kpi.Id == target.Kpi.Id && x.Periode == previousDate && x.PeriodeType == PeriodeType.Daily);
                                kpiInformation.DailyTarget = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = target.Periode,
                                    Value = target.Value.HasValue ? target.Value : null,
                                    Remark = string.IsNullOrEmpty(target.Remark) ? (prevTarget != null ? "prev--" + prevTarget.Remark : target.Remark) : target.Remark,
                                    Type = "now",
                                    Id = target.Id
                                };
                            }
                            else
                            {
                                var todayValue = targets.FirstOrDefault(x => x.Kpi.Id == target.Kpi.Id && x.Periode == request.Date && x.PeriodeType == PeriodeType.Daily);
                                var prevTarget = targets.FirstOrDefault(x => x.Kpi.Id == target.Kpi.Id && x.Periode == previousDate && x.PeriodeType == PeriodeType.Daily);
                                if (todayValue != null)
                                {
                                    kpiInformation.DailyTarget = new GetKpiInformationValuesResponse.KpiValue
                                    {
                                        Date = todayValue.Periode,
                                        Value = todayValue.Value.HasValue ? todayValue.Value : null,
                                        Remark = string.IsNullOrEmpty(todayValue.Remark) ? (prevTarget != null ? "prev--" + prevTarget.Remark : todayValue.Remark) : todayValue.Remark,
                                        Type = "now",
                                        Id = todayValue.Id
                                    };

                                }
                                else
                                {
                                    //var prevDayValue = targets.OrderByDescending(x => x.Periode).FirstOrDefault(x => x.Kpi.Id == target.Kpi.Id && x.Periode <= request.Date && x.PeriodeType == PeriodeType.Daily);
                                    //yesterday value selected
                                    kpiInformation.DailyTarget = new GetKpiInformationValuesResponse.KpiValue
                                    {
                                        Date = target.Periode,
                                        Value = target.Value.HasValue ? target.Value : null,
                                        Remark = target.Remark,
                                        Type = "prev",
                                        Id = target.Id
                                    };
                                }
                            }
                        }
                    }
                    #endregion
                    #region target-monthly
                    if (target.PeriodeType == PeriodeType.Monthly)
                    {
                        if (kpiInformation.MonthlyTarget == null)
                        {
                            bool currentMonthTarget = false;
                            if (target.Kpi.Id == 385)
                            {
                                DateTime prevMonth = request.Date.AddMonths(-1);
                                currentMonthTarget = target.Periode.Year == prevMonth.Year && target.Periode.Month == prevMonth.Month && target.PeriodeType == PeriodeType.Monthly;
                            }
                            else {
                                currentMonthTarget = target.Periode.Year == request.Date.Year && target.Periode.Month == request.Date.Month && target.PeriodeType == PeriodeType.Monthly;
                            }

                            //var currentMonthTarget = target.Periode.Year  == request.Date.Year && target.Periode.Month == request.Date.Month && target.PeriodeType == PeriodeType.Monthly;
                            if (currentMonthTarget)
                            {
                                var prevTarget = targets.FirstOrDefault(x => x.Kpi.Id == target.Kpi.Id && x.Periode == request.Date.AddMonths(-1) && x.PeriodeType == PeriodeType.Monthly);
                                kpiInformation.MonthlyTarget = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = target.Periode,
                                    Value = target.Value ?? null,
                                    Remark = string.IsNullOrEmpty(target.Remark) ? (prevTarget != null ? "prev--" + prevTarget.Remark : target.Remark) : target.Remark,
                                    Type = "now",
                                    Id = target.Id
                                };
                            }
                            else
                            {
                                kpiInformation.MonthlyTarget = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = target.Periode,
                                    Value = target.Value ?? null,
                                    Remark = target.Remark,
                                    Type = "prev",
                                    Id = 0
                                };
                            }
                        }
                    }
                    #endregion
                    #region target-yearly
                    if (target.PeriodeType == PeriodeType.Yearly)
                    {
                        if (kpiInformation.YearlyTarget == null)
                        {
                            var currentYearValue = target.Periode.Year == request.Date.Year && target.PeriodeType == PeriodeType.Yearly;
                            if (currentYearValue)
                            {
                                var prevTarget = targets.FirstOrDefault(x => x.Kpi.Id == target.Kpi.Id && x.Periode == request.Date.AddYears(-1) && x.PeriodeType == PeriodeType.Yearly);
                                kpiInformation.YearlyTarget = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = target.Periode,
                                    Value = target.Value ?? null,
                                    Remark = string.IsNullOrEmpty(target.Remark) ? (prevTarget != null ? "prev--" + prevTarget.Remark : target.Remark) : target.Remark,
                                    Type = "now",
                                    Id = target.Id
                                };
                            }
                            else
                            {
                                kpiInformation.YearlyTarget = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = target.Periode,
                                    Value = target.Value ?? null,
                                    Remark = target.Remark,
                                    Type = "prev",
                                    Id = target.Id
                                };
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion
            return response;
        }

        private List<Data.Entities.Highlight> GetHighligts(IEnumerable<int> highlightTypeIds, DateTime period)
        {
            var response = new List<Data.Entities.Highlight>();
            if (highlightTypeIds != null)
            {
                foreach (var id in highlightTypeIds)
                {
                    var daily = DataContext.Highlights.OrderByDescending(x => x.Date).Include(x => x.HighlightType).FirstOrDefault(x => x.HighlightType.Id == id && x.PeriodeType == PeriodeType.Daily && x.Date <= period.Date);
                    if (daily != null)
                    {
                        response.Add(daily);
                    }
                    var monthly = DataContext.Highlights.OrderByDescending(x => x.Date).Include(x => x.HighlightType).FirstOrDefault(x => x.HighlightType.Id == id && x.PeriodeType == PeriodeType.Monthly && x.Date <= period.Date);
                    if (monthly != null)
                    {
                        response.Add(monthly);
                    }
                    var yearly = DataContext.Highlights.OrderByDescending(x => x.Date).Include(x => x.HighlightType).FirstOrDefault(x => x.HighlightType.Id == id && x.PeriodeType == PeriodeType.Yearly && x.Date <= period.Date);
                    if (yearly != null)
                    {
                        response.Add(yearly);
                    }
                }
            }
            return response;
        }

        public GetHighlightValuesResponse GetHighlightValues(GetHighlightValuesRequest request)
        {
            var prevDate = request.Date.AddDays(-1);
            var derHighlights = request.HighlightTypeIds;
            var highlights = DataContext.Highlights.Include(x => x.HighlightType)
                .Where(x => derHighlights.Contains(x.HighlightType.Id) && x.PeriodeType == PeriodeType.Daily && (x.Date == request.Date || x.Date == prevDate)).ToList();

            //var highlights = GetHighligts(derHighlights, request.Date);
            var response = new GetHighlightValuesResponse();
            if (highlights != null)
            {
                foreach (var highlight in highlights)
                {
                    if(highlight == null)
                    {
                        continue;
                    }
                    #region daily
                    if (highlight.PeriodeType == PeriodeType.Daily)
                    {
                        var highlightResp = response.Highlights.FirstOrDefault(x => x.HighlightTypeId == highlight.HighlightType.Id);
                        if (highlightResp == null)
                        {
                            highlightResp = new GetHighlightValuesResponse.DerHighlight { HighlightTypeId = highlight.HighlightType.Id };
                            response.Highlights.Add(highlightResp);
                        }
                        var isTodayValue = highlight.Date == request.Date && highlight.PeriodeType == PeriodeType.Daily;
                        if (isTodayValue)
                        {
                            highlightResp.HighlightTypeId = highlight.HighlightType.Id;
                            highlightResp.HighlightTypeValue = highlight.HighlightType.Value;
                            highlightResp.HighlightMessage = highlight.Message;
                            highlightResp.HighlightTitle = highlight.Title;
                            highlightResp.Date = highlight.Date;
                            highlightResp.Type = "now";
                            highlightResp.Id = highlight.Id;
                        }
                        else
                        {
                            var todayValue = highlights.FirstOrDefault(x => x.HighlightType.Id == highlight.HighlightType.Id && x.Date == request.Date && highlight.PeriodeType == PeriodeType.Daily);
                            if (todayValue != null)
                            {
                                highlightResp.HighlightTypeId = todayValue.HighlightType.Id;
                                highlightResp.HighlightTypeValue = todayValue.HighlightType.Value;
                                highlightResp.HighlightMessage = todayValue.Message;
                                highlightResp.HighlightTitle = todayValue.Title;
                                highlightResp.Date = todayValue.Date;
                                highlightResp.Type = "now";
                                highlightResp.Id = todayValue.Id;
                            }
                            else
                            {
                                var prevValue = highlights.OrderByDescending(x=>x.Date).FirstOrDefault(x => x.HighlightType.Id == highlight.HighlightType.Id && x.Date <= request.Date && highlight.PeriodeType == PeriodeType.Daily);
                                if (prevValue != null)
                                {
                                    highlightResp.HighlightTypeId = prevValue.HighlightType.Id;
                                    highlightResp.HighlightTypeValue = prevValue.HighlightType.Value;
                                    highlightResp.HighlightMessage = prevValue.Message;
                                    highlightResp.HighlightTitle = prevValue.Title;
                                    highlightResp.Date = prevValue.Date;
                                    highlightResp.Type = "prev";
                                    highlightResp.Id = 0;
                                }
                                else
                                {
                                    //yesterday value selected
                                    highlightResp.HighlightTypeId = highlight.HighlightType.Id;
                                    highlightResp.HighlightTypeValue = highlight.HighlightType.Value;
                                    highlightResp.HighlightMessage = highlight.Message;
                                    highlightResp.HighlightTitle = highlight.Title;
                                    highlightResp.Date = highlight.Date;
                                    highlightResp.Type = "prev";
                                    highlightResp.Id = 0;
                                }
                            }
                        }
                    }
                    #endregion
                    #region monthly
                    if (highlight.PeriodeType == PeriodeType.Monthly)
                    {
                        var highlightResp = response.Highlights.FirstOrDefault(x => x.HighlightTypeId == highlight.HighlightType.Id);
                        if (highlightResp == null)
                        {
                            highlightResp = new GetHighlightValuesResponse.DerHighlight { HighlightTypeId = highlight.HighlightType.Id };
                            response.Highlights.Add(highlightResp);
                        }
                        var isCurrentMonthValue = (highlight.Date.Month == request.Date.Month && highlight.Date.Year == request.Date.Year) && highlight.PeriodeType == PeriodeType.Monthly;
                        if (isCurrentMonthValue)
                        {
                            highlightResp.HighlightTypeId = highlight.HighlightType.Id;
                            highlightResp.HighlightTypeValue = highlight.HighlightType.Value;
                            highlightResp.HighlightMessage = highlight.Message;
                            highlightResp.HighlightTitle = highlight.Title;
                            highlightResp.Date = highlight.Date;
                            highlightResp.Type = "now";
                            highlightResp.Id = highlight.Id;
                        }
                        else
                        {
                            var currentMonthValue = highlights.FirstOrDefault(x => x.HighlightType.Id == highlight.HighlightType.Id && (x.Date.Month == request.Date.Month && x.Date.Year == request.Date.Year) && highlight.PeriodeType == PeriodeType.Monthly);
                            if (currentMonthValue != null)
                            {
                                highlightResp.HighlightTypeId = currentMonthValue.HighlightType.Id;
                                highlightResp.HighlightTypeValue = currentMonthValue.HighlightType.Value;
                                highlightResp.HighlightMessage = currentMonthValue.Message;
                                highlightResp.HighlightTitle = currentMonthValue.Title;
                                highlightResp.Date = currentMonthValue.Date;
                                highlightResp.Type = "now";
                                highlightResp.Id = currentMonthValue.Id;
                            }
                            else
                            {
                                var prevMonthValue = highlights.OrderByDescending(x=>x.Date).FirstOrDefault(x => x.HighlightType.Id == highlight.HighlightType.Id && x.Date <= request.Date && highlight.PeriodeType == PeriodeType.Monthly);
                                if (prevMonthValue != null)
                                {
                                    highlightResp.HighlightTypeId = prevMonthValue.HighlightType.Id;
                                    highlightResp.HighlightTypeValue = prevMonthValue.HighlightType.Value;
                                    highlightResp.HighlightMessage = prevMonthValue.Message;
                                    highlightResp.HighlightTitle = prevMonthValue.Title;
                                    highlightResp.Date = prevMonthValue.Date;
                                    highlightResp.Type = "prev";
                                    highlightResp.Id = prevMonthValue.Id;
                                }
                                else
                                {
                                    //yesterday value selected
                                    highlightResp.HighlightTypeId = highlight.HighlightType.Id;
                                    highlightResp.HighlightTypeValue = highlight.HighlightType.Value;
                                    highlightResp.HighlightMessage = highlight.Message;
                                    highlightResp.HighlightTitle = highlight.Title;
                                    highlightResp.Date = highlight.Date;
                                    highlightResp.Type = "prev";
                                    highlightResp.Id = highlight.Id;
                                }
                            }
                        }
                    }
                    #endregion
                    #region yearly
                    if(highlight.PeriodeType == PeriodeType.Yearly)
                    {
                        var highlightResp = response.Highlights.FirstOrDefault(x => x.HighlightTypeId == highlight.HighlightType.Id);
                        if (highlightResp == null)
                        {
                            highlightResp = new GetHighlightValuesResponse.DerHighlight { HighlightTypeId = highlight.HighlightType.Id };
                            response.Highlights.Add(highlightResp);
                        }
                        var isCurrentYear = highlight.Date == request.Date && highlight.PeriodeType == PeriodeType.Yearly;
                        if (isCurrentYear)
                        {
                            highlightResp.HighlightTypeId = highlight.HighlightType.Id;
                            highlightResp.HighlightTypeValue = highlight.HighlightType.Value;
                            highlightResp.HighlightMessage = highlight.Message;
                            highlightResp.HighlightTitle = highlight.Title;
                            highlightResp.Date = highlight.Date;
                            highlightResp.Type = "now";
                            highlightResp.Id = highlight.Id;
                        }
                        else
                        {
                            var CurrentYearValue = highlights.FirstOrDefault(x => x.HighlightType.Id == highlight.HighlightType.Id && x.Date.Year == request.Date.Year && highlight.PeriodeType == PeriodeType.Daily);
                            if (CurrentYearValue != null)
                            {
                                highlightResp.HighlightTypeId = CurrentYearValue.HighlightType.Id;
                                highlightResp.HighlightTypeValue = CurrentYearValue.HighlightType.Value;
                                highlightResp.HighlightMessage = CurrentYearValue.Message;
                                highlightResp.HighlightTitle = CurrentYearValue.Title;
                                highlightResp.Date = CurrentYearValue.Date;
                                highlightResp.Type = "now";
                                highlightResp.Id = CurrentYearValue.Id;
                            }
                            else
                            {
                                var lastYearValue = highlights.OrderByDescending(x => x.Date).FirstOrDefault(x => x.HighlightType.Id == highlight.HighlightType.Id && x.Date.Year <= request.Date.Year && highlight.PeriodeType == PeriodeType.Yearly);
                                if (lastYearValue != null)
                                {
                                    highlightResp.HighlightTypeId = lastYearValue.HighlightType.Id;
                                    highlightResp.HighlightTypeValue = lastYearValue.HighlightType.Value;
                                    highlightResp.HighlightMessage = lastYearValue.Message;
                                    highlightResp.HighlightTitle = lastYearValue.Title;
                                    highlightResp.Date = lastYearValue.Date;
                                    highlightResp.Type = "prev";
                                    highlightResp.Id = lastYearValue.Id;
                                }
                                else
                                {
                                    //yesterday value selected
                                    highlightResp.HighlightTypeId = highlight.HighlightType.Id;
                                    highlightResp.HighlightTypeValue = highlight.HighlightType.Value;
                                    highlightResp.HighlightMessage = highlight.Message;
                                    highlightResp.HighlightTitle = highlight.Title;
                                    highlightResp.Date = highlight.Date;
                                    highlightResp.Type = "prev";
                                    highlightResp.Id = highlight.Id;
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            return response;
        }

        public BaseResponse CreateDerInputFile(CreateDerInputFileRequest request)
        {            
            var response = new BaseResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var derInputFile = new DerInputFile();
                derInputFile.Date = request.Date;
                derInputFile.FileName = request.FileName;
                derInputFile.Title = request.Title;
                var user = DataContext.Users.Single(x => x.Id == request.CreatedBy);
                //DataContext.Users.Attach(user);
                derInputFile.CreatedBy = user;
                derInputFile.UpdatedBy = user;
                derInputFile.CreatedDate = DateTime.Now;
                derInputFile.UpdatedDate = DateTime.Now;
                DataContext.DerInputFiles.Add(derInputFile);

                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "Der Input File has been added successfully";
            }
            catch (Exception exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }

            return response;
        }

        public GetDerInputFilesResponse GetDerInputFiles(GetDerInputFilesRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, request.Date, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }
            var derInputFiles = data.ToList();
            var response = new GetDerInputFilesResponse();
            response.TotalRecords = totalRecords;
            //response.DerInputFiles = derInputFiles.Select(x => new GetDerInputFilesResponse.DerInputFile { Date = x.Key }).ToList();
            response.DerInputFiles = derInputFiles.MapTo<GetDerInputFilesResponse.DerInputFile>();
            return response;
        }

        public IEnumerable<DerInputFile> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, string Date, out int TotalRecords)
        {
            var data = DataContext.DerInputFiles.AsQueryable();
            data = data.Include(x => x.CreatedBy)
                .Include(x => x.UpdatedBy);

            if (!string.IsNullOrEmpty(Date) && !string.IsNullOrWhiteSpace(Date))
            {
                var currentDate = DateTime.ParseExact(Date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                data = data.Where(x => x.Date == currentDate);                
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {                    
                    case "Date":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Date)
                            : data.OrderByDescending(x => x.Date);
                        break;
                    case "FileName":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.FileName)
                            : data.OrderByDescending(x => x.FileName);
                        break;
                    case "CreatedBy":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.CreatedBy.Username)
                            : data.OrderByDescending(x => x.CreatedBy.Username);
                        break;
                }
            }
            
            TotalRecords = data.Count();
            return data;
        }

        public DeleteDerInputFileResponse DeleteDerInputFile(DerDeleteRequest req)
        {
            var response = new DeleteDerInputFileResponse();
            try
            {
                var action = req.MapTo<BaseAction>();
                var derInputFile = DataContext.DerInputFiles.Single(x => x.Id == req.Id);
                response.Date = new DateTime(derInputFile.Date.Year, derInputFile.Date.Month, derInputFile.Date.Day);
                DataContext.DerInputFiles.Attach(derInputFile);
                DataContext.Entry(derInputFile).State = EntityState.Deleted;
                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "DER input file attachment has been deleted successfully";
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
    }
}
