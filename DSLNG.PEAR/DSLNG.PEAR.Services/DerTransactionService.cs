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

        public GetKpiInformationValuesResponse GetKpiInformationValues(GetKpiInformationValuesRequest request)
        {
            //var kpiIds = 
            //achievement section
            var kpiIdsForActual = request.ActualKpiIds;
            var previousDate = request.Date.AddDays(-1);
            var achievements = DataContext.KpiAchievements.Include(x => x.Kpi)
                .Where(x => kpiIdsForActual.Contains(x.Kpi.Id) &&
                (((x.Periode == request.Date || x.Periode == previousDate) && x.PeriodeType == PeriodeType.Daily) ||
                (x.PeriodeType == PeriodeType.Yearly && x.Periode.Year == request.Date.Year) ||
                (x.PeriodeType == PeriodeType.Monthly && x.Periode.Month == request.Date.Month))).ToList();
            var kpiIdsForTarget = request.TargetKpiIds;
            var targets = DataContext.KpiTargets.Include(x => x.Kpi)
               .Where(x => kpiIdsForActual.Contains(x.Kpi.Id) &&
               (((x.Periode == request.Date || x.Periode == previousDate) && x.PeriodeType == PeriodeType.Daily) ||
               (x.PeriodeType == PeriodeType.Yearly && x.Periode.Year == request.Date.Year) ||
               (x.PeriodeType == PeriodeType.Monthly && x.Periode.Month == request.Date.Month))).ToList();

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
                if (actual.PeriodeType == PeriodeType.Daily)
                {
                    if (kpiInformation.DailyActual == null)
                    {
                        var isTodayValue = actual.Periode == request.Date;
                        if (isTodayValue)
                        {
                            kpiInformation.DailyActual = new GetKpiInformationValuesResponse.KpiValue
                            {
                                Date = actual.Periode,
                                Value = actual.Value.Value,
                                Remark = actual.Remark,
                                Id = actual.Id,
                                Type = "now"
                            };
                        }
                        else
                        {
                            var todayValue = achievements.FirstOrDefault(x => x.Kpi.Id == actual.Kpi.Id && x.Periode == request.Date);
                            if (todayValue != null)
                            {
                                kpiInformation.DailyActual = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = todayValue.Periode,
                                    Value = todayValue.Value.Value,
                                    Remark = todayValue.Remark,
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
                                    Value = actual.Value.Value,
                                    Remark = actual.Remark,
                                    Type = "prev"
                                };
                            }
                        }
                    }
                }
                if (actual.PeriodeType == PeriodeType.Monthly)
                {
                    kpiInformation.MonthlyActual = new GetKpiInformationValuesResponse.KpiValue
                    {
                        Date = actual.Periode,
                        Value = actual.Value.Value,
                        Remark = actual.Remark,
                        Id = actual.Id
                    };
                }
                if (actual.PeriodeType == PeriodeType.Yearly)
                {
                    kpiInformation.YearlyActual = new GetKpiInformationValuesResponse.KpiValue
                    {
                        Date = actual.Periode,
                        Value = actual.Value.Value,
                        Remark = actual.Remark,
                        Id = actual.Id
                    };
                }

            }
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
                if (target.PeriodeType == PeriodeType.Daily)
                {
                    if (kpiInformation.DailyTarget == null)
                    {
                        var isTodayValue = target.Periode == request.Date;
                        if (isTodayValue)
                        {
                            kpiInformation.DailyTarget = new GetKpiInformationValuesResponse.KpiValue
                            {
                                Date = target.Periode,
                                Value = target.Value.Value,
                                Remark = target.Remark,
                                Type = "now"
                            };
                        }
                        else
                        {
                            var todayValue = targets.FirstOrDefault(x => x.Kpi.Id == target.Kpi.Id && x.Periode == request.Date);
                            if (todayValue != null)
                            {
                                kpiInformation.DailyTarget = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = todayValue.Periode,
                                    Value = todayValue.Value.Value,
                                    Remark = todayValue.Remark,
                                    Type = "now"
                                };

                            }
                            else
                            {
                                //yesterday value selected
                                kpiInformation.DailyTarget = new GetKpiInformationValuesResponse.KpiValue
                                {
                                    Date = target.Periode,
                                    Value = target.Value.Value,
                                    Remark = target.Remark,
                                    Type = "prev"
                                };
                            }
                        }
                    }
                }
                if (target.PeriodeType == PeriodeType.Monthly)
                {
                    kpiInformation.MonthlyTarget = new GetKpiInformationValuesResponse.KpiValue
                    {
                        Date = target.Periode,
                        Value = target.Value.Value,
                        Remark = target.Remark
                    };
                }
                if (target.PeriodeType == PeriodeType.Yearly)
                {
                    kpiInformation.YearlyTarget = new GetKpiInformationValuesResponse.KpiValue
                    {
                        Date = target.Periode,
                        Value = target.Value.Value,
                        Remark = target.Remark
                    };
                }
            }
            return response;
        }

        public GetHighlightValuesResponse GetHighlightValues(GetHighlightValuesRequest request)
        {
            var derHighlights = request.HighlightTypeIds;
            var highlights = DataContext.Highlights.Include(x => x.HighlightType)
                .Where(x => derHighlights.Contains(x.HighlightType.Id) && x.PeriodeType == PeriodeType.Daily).ToList();
            var response = new GetHighlightValuesResponse();
            foreach (var highlight in highlights)
            {
                var highlightResp = response.Highlights.FirstOrDefault(x => x.HighlightTypeId == highlight.HighlightType.Id);
                if (highlightResp == null)
                {
                    highlightResp = new GetHighlightValuesResponse.DerHighlight { HighlightTypeId = highlight.HighlightType.Id };
                    response.Highlights.Add(highlightResp);
                }
                var isTodayValue = highlight.Date == request.Date;
                if (isTodayValue)
                {
                    highlightResp.HighlightTypeId = highlight.HighlightType.Id;
                    highlightResp.HighlightMessage = highlight.Message;
                    highlightResp.HighlightTitle = highlight.Title;
                    highlightResp.Date = highlight.Date;
                    highlightResp.Type = "now";
                }
                else
                {
                    var todayValue = highlights.FirstOrDefault(x => x.HighlightType.Id == highlight.HighlightType.Id && x.Date == request.Date);
                    if (todayValue != null)
                    {
                        highlightResp.HighlightTypeId = todayValue.HighlightType.Id;
                        highlightResp.HighlightMessage = todayValue.Message;
                        highlightResp.HighlightTitle = todayValue.Title;
                        highlightResp.Date = todayValue.Date;
                        highlightResp.Type = "now";
                    }
                    else
                    {
                        //yesterday value selected
                        highlightResp.HighlightTypeId = highlight.HighlightType.Id;
                        highlightResp.HighlightMessage = highlight.Message;
                        highlightResp.HighlightTitle = highlight.Title;
                        highlightResp.Date = highlight.Date;
                        highlightResp.Type = "prev";
                    }
                }
            }
            return response;
        }
    }
}
