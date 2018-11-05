

using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Artifact;
using DSLNG.PEAR.Services.Responses.Artifact;
using System;
using System.Collections.Generic;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using System.Globalization;
using DSLNG.PEAR.Common.Contants;
using System.Data.SqlClient;
using DSLNG.PEAR.Data.Entities.EconomicModel;

namespace DSLNG.PEAR.Services
{
    public class ArtifactService : BaseService, IArtifactService
    {
        private IKpiAchievementService _kpiAchievementService;
        private IKpiTargetService _kpiTargetService;
        public ArtifactService(IDataContext dataContext, IKpiTargetService kpiTargetService, IKpiAchievementService kpiAchievementService)
            : base(dataContext)
        {
            _kpiTargetService = kpiTargetService;
            _kpiAchievementService = kpiAchievementService;
        }

        public GetTankDataResponse GetTankData(GetTankDataRequest request)
        {
            var response = request.Tank.MapTo<GetTankDataResponse>();
            var volumeInventory = DataContext.Kpis.Include(x => x.Measurement).Where(x => x.Id == request.Tank.VolumeInventoryId).First();
            response.VolumeInventoryUnit = volumeInventory.Measurement.Name;
            var daysToTankTop = DataContext.Kpis.Include(x => x.Measurement).Where(x => x.Id == request.Tank.DaysToTankTopId).First();
            response.DaysToTankTopUnit = daysToTankTop.Measurement.Name;
            IList<DateTime> dateTimePeriodes = new List<DateTime>();
            string timeInformation;
            this.GetPeriodes(request.PeriodeType, request.RangeFilter, request.Start, request.End, out dateTimePeriodes, out timeInformation);
            var start = dateTimePeriodes[0];
            var end = dateTimePeriodes[dateTimePeriodes.Count - 1];
            switch (volumeInventory.YtdFormula)
            {

                case YtdFormula.Sum:
                default:
                    {
                        response.VolumeInventory = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == volumeInventory.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                    }
                    break;
                case YtdFormula.Average:
                    {
                        response.VolumeInventory = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == volumeInventory.Id)
                                    .GroupBy(x => x.Kpi.Id)
                                    .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                    }
                    break;
            }
            switch (daysToTankTop.YtdFormula)
            {
                case YtdFormula.Sum:
                default:
                    {
                        response.DaysToTankTop = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == daysToTankTop.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                    }
                    break;
                case YtdFormula.Average:
                    {
                        response.DaysToTankTop = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == daysToTankTop.Id)
                                    .GroupBy(x => x.Kpi.Id)
                                    .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                    }
                    break;
            }
            KpiAchievement latestVolInventory = null;
            if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                       (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                       (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                       (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear))
            {
                var actual = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
              x.Periode <= end && x.Kpi.Id == volumeInventory.Id && (x.Value != null && x.Value.Value != 0))
              .OrderByDescending(x => x.Periode).FirstOrDefault();
                if (actual != null)
                {
                    latestVolInventory = actual;
                    response.VolumeInventory = actual.Value.Value;
                }
            }
            if (latestVolInventory != null)
            {
                if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                         (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                         (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                         (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear))
                {
                    var actual = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                  x.Periode == latestVolInventory.Periode && x.Kpi.Id == daysToTankTop.Id && (x.Value != null && x.Value.Value != 0))
                  .OrderByDescending(x => x.Periode).FirstOrDefault();
                    if (actual != null)
                    {
                        response.DaysToTankTop = actual.Value.Value;
                    }
                    switch (request.PeriodeType)
                    {
                        case PeriodeType.Hourly:
                            timeInformation = latestVolInventory.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Daily:
                            timeInformation = latestVolInventory.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Monthly:
                            timeInformation = latestVolInventory.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Yearly:
                            timeInformation = latestVolInventory.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                            break;
                    }
                }
            }
            response.Subtitle = timeInformation;
            return response;
        }

        public GetTabularDataResponse GetTabularData(GetTabularDataRequest request)
        {
            var response = request.MapTo<GetTabularDataResponse>();
            foreach (var row in request.Rows)
            {
                var kpi = DataContext.Kpis.Include(x => x.Measurement).Where(x => x.Id == row.KpiId).First();
                IList<DateTime> dateTimePeriodes = new List<DateTime>();
                string timeInformation;
                var oldRangeFilter = row.RangeFilter;
                if (row.RangeFilter.Equals(RangeFilter.SpecificYear) || row.RangeFilter.Equals(RangeFilter.SpecificMonth) || row.RangeFilter.Equals(RangeFilter.SpecificDay))
                {
                    var timeValue = ChangeFromSpecificToInterval(row.Start, row.End, row.RangeFilter);
                    row.Start = timeValue.Start;
                    row.End = timeValue.End;
                    row.RangeFilter = timeValue.RangeFilter;
                }

                this.GetPeriodes(row.PeriodeType, row.RangeFilter, row.Start, row.End, out dateTimePeriodes,
                                  out timeInformation);

                if (oldRangeFilter.Equals(RangeFilter.SpecificYear) || oldRangeFilter.Equals(RangeFilter.SpecificMonth) || oldRangeFilter.Equals(RangeFilter.SpecificDay))
                {
                    timeInformation = ChangeTimeInformationFromSpecificToInterval(row.Start, row.End, oldRangeFilter);
                }

                //this.GetPeriodes(row.PeriodeType, row.RangeFilter, row.Start, row.End, out dateTimePeriodes, out timeInformation);
                var start = dateTimePeriodes[0];
                var end = dateTimePeriodes[dateTimePeriodes.Count - 1];
                var rowResponse = new GetTabularDataResponse.RowResponse();
                rowResponse.KpiId = kpi.Id;
                rowResponse.KpiName = kpi.Name;
                rowResponse.Measurement = kpi.Measurement.Name;
                rowResponse.PeriodeType = row.PeriodeType.ToString();
                rowResponse.Periode = timeInformation;//start.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + end.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                rowResponse.PeriodeDateTime = dateTimePeriodes.Count > 0 ? dateTimePeriodes[0] : DateTime.Now;
                if (request.Remark)
                {
                    var actual = DataContext.KpiAchievements.Where(x => x.PeriodeType == row.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == row.KpiId).FirstOrDefault();
                    rowResponse.Remark = actual != null ? actual.Remark : "";
                }
                #region switch
                switch (kpi.YtdFormula)
                {
                    case YtdFormula.Sum:
                        if (request.Target)
                        {
                            rowResponse.Target = DataContext.KpiTargets.Where(x => x.PeriodeType == row.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == row.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        if (request.Actual)
                        {
                            rowResponse.Actual = DataContext.KpiAchievements.Where(x => x.PeriodeType == row.PeriodeType &&
                                   x.Periode >= start && x.Periode <= end && x.Kpi.Id == row.KpiId)
                                   .GroupBy(x => x.Kpi.Id)
                                   .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        break;
                    case YtdFormula.Average:
                        if (request.Target)
                        {
                            rowResponse.Target = DataContext.KpiTargets.Where(x => x.PeriodeType == row.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == row.KpiId)
                                    .GroupBy(x => x.Kpi.Id)
                                    .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        if (request.Actual)
                        {
                            rowResponse.Actual = DataContext.KpiAchievements.Where(x => x.PeriodeType == row.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == row.KpiId)
                                    .GroupBy(x => x.Kpi.Id)
                                    .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        break;
                }
                #endregion

                #region if
                KpiAchievement latestActual = null;
                if (request.Actual)
                {
                    if ((row.PeriodeType == PeriodeType.Hourly && row.RangeFilter == RangeFilter.CurrentHour) ||
                           (row.PeriodeType == PeriodeType.Daily && row.RangeFilter == RangeFilter.CurrentDay) ||
                           (row.PeriodeType == PeriodeType.Monthly && row.RangeFilter == RangeFilter.CurrentMonth) ||
                           (row.PeriodeType == PeriodeType.Yearly && row.RangeFilter == RangeFilter.CurrentYear))
                    {
                        var kpiActual = DataContext.KpiAchievements.Where(x => x.PeriodeType == row.PeriodeType &&
                      x.Periode <= end && x.Kpi.Id == row.KpiId && (x.Value != null))
                      .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiActual != null && kpiActual.Value.HasValue)
                        {
                            latestActual = kpiActual;
                            rowResponse.Actual = kpiActual.Value.Value;
                        }
                        else
                        {
                            latestActual = kpiActual;
                            rowResponse.Actual = null;
                        }
                    }
                }
                if (request.Target && latestActual != null)
                {
                    if ((row.PeriodeType == PeriodeType.Hourly && row.RangeFilter == RangeFilter.CurrentHour) ||
                           (row.PeriodeType == PeriodeType.Daily && row.RangeFilter == RangeFilter.CurrentDay) ||
                           (row.PeriodeType == PeriodeType.Monthly && row.RangeFilter == RangeFilter.CurrentMonth) ||
                           (row.PeriodeType == PeriodeType.Yearly && row.RangeFilter == RangeFilter.CurrentYear))
                    {
                        var kpiTarget = DataContext.KpiTargets.Where(x => x.PeriodeType == row.PeriodeType &&
                      x.Periode == latestActual.Periode && x.Kpi.Id == row.KpiId)
                      .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiTarget != null && kpiTarget.Value.HasValue)
                        {
                            rowResponse.Target = kpiTarget.Value.Value;
                        }
                        else
                        {
                            rowResponse.Target = null;
                        }
                    }
                }
                if (latestActual != null)
                {
                    switch (row.PeriodeType)
                    {
                        case PeriodeType.Hourly:
                            rowResponse.Periode = latestActual.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Daily:
                            rowResponse.Periode = latestActual.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Monthly:
                            rowResponse.Periode = latestActual.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Yearly:
                            rowResponse.Periode = latestActual.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                            break;
                    }
                }
                #endregion
                response.Rows.Add(rowResponse);
            }
            return response;
        }

        public GetPieDataResponse GetPieData(GetPieDataRequest request)
        {
            var response = new GetPieDataResponse();
            IList<DateTime> dateTimePeriodes = new List<DateTime>();
            string timeInformation;
            var oldRangeFilter = request.RangeFilter;
            if (request.RangeFilter.Equals(RangeFilter.SpecificYear) || request.RangeFilter.Equals(RangeFilter.SpecificMonth) || request.RangeFilter.Equals(RangeFilter.SpecificDay))
            {
                var timeValue = ChangeFromSpecificToInterval(request.Start, request.End, request.RangeFilter);
                request.Start = timeValue.Start;
                request.End = timeValue.End;
                request.RangeFilter = timeValue.RangeFilter;
            }

            this.GetPeriodes(request.PeriodeType, request.RangeFilter, request.Start, request.End, out dateTimePeriodes,
                              out timeInformation);

            if (oldRangeFilter.Equals(RangeFilter.SpecificYear) || oldRangeFilter.Equals(RangeFilter.SpecificMonth) || oldRangeFilter.Equals(RangeFilter.SpecificDay))
            {
                timeInformation = ChangeTimeInformationFromSpecificToInterval(request.Start, request.End, oldRangeFilter);
            }

            foreach (var series in request.Series)
            {
                var kpi = DataContext.Kpis.Include(x => x.Measurement).First(x => x.Id == series.KpiId);
                var seriesResponse = new GetPieDataResponse.SeriesResponse();
                seriesResponse.color = series.Color;
                seriesResponse.name = string.IsNullOrEmpty(series.Label) ? kpi.Name : series.Label;
                seriesResponse.measurement = kpi.Measurement.Name;
                var start = dateTimePeriodes[0];
                var end = dateTimePeriodes[dateTimePeriodes.Count - 1];
                #region switch
                switch (kpi.YtdFormula)
                {
                    case YtdFormula.Sum:
                        if (request.ValueAxis == ValueAxis.KpiTarget)
                        {
                            seriesResponse.y = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        else if (request.ValueAxis == ValueAxis.KpiActual)
                        {
                            seriesResponse.y = SumSeries(request.ValueInformation, request.PeriodeType, start, end, kpi.Id);
                        }
                        else if (request.ValueAxis == ValueAxis.KpiEconomic)
                        {
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            seriesResponse.y = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType
                               && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id && x.Scenario.Id == scenarioId)
                               .GroupBy(x => x.Kpi.Id)
                               .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        break;

                    case YtdFormula.Average:
                        if (request.ValueAxis == ValueAxis.KpiTarget)
                        {
                            seriesResponse.y = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        else if (request.ValueAxis == ValueAxis.KpiActual)
                        {
                            seriesResponse.y = AverageSeries(request.ValueInformation, request.PeriodeType, start, end, kpi.Id);
                        }
                        else if (request.ValueAxis == ValueAxis.KpiEconomic)
                        {
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            seriesResponse.y = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType
                               && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id && x.Scenario.Id == scenarioId)
                               .GroupBy(x => x.Kpi.Id)
                               .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        break;
                    default: //nan or custom:
                        if (request.ValueAxis == ValueAxis.KpiTarget)
                        {
                            seriesResponse.y = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        else if (request.ValueAxis == ValueAxis.KpiActual)
                        {
                            seriesResponse.y = SumSeries(request.ValueInformation, request.PeriodeType, start, end, kpi.Id);
                        }
                        else if (request.ValueAxis == ValueAxis.KpiEconomic)
                        {
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            seriesResponse.y = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType
                               && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id && x.Scenario.Id == scenarioId)
                               .GroupBy(x => x.Kpi.Id)
                               .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        break;
                }
                #endregion

                KpiAchievement latestActual = null;
                if (request.ValueAxis == ValueAxis.KpiActual)
                {
                    if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                        (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                        (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                        (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear))
                    {
                        var kpiActual = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                                                               x.Periode <= end &&
                                                                               x.Kpi.Id == kpi.Id && (x.Value != null))
                                                   .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiActual != null && kpiActual.Value.HasValue)
                        {
                            latestActual = kpiActual;
                            seriesResponse.y = kpiActual.Value.Value;
                        }
                    }
                }

                if (request.ValueAxis == ValueAxis.KpiTarget && latestActual != null)
                {
                    if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                        (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                        (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                        (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear))
                    {
                        var kpiTarget = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType &&
                                                                          x.Periode == latestActual.Periode &&
                                                                          x.Kpi.Id == kpi.Id)
                                                   .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiTarget != null && kpiTarget.Value.HasValue)
                        {
                            seriesResponse.y = kpiTarget.Value.Value;
                        }
                    }
                }
                if (request.ValueAxis == ValueAxis.KpiEconomic && latestActual != null)
                {
                    if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                        (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                        (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                        (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear))
                    {
                        var scenarioId = 0;
                        var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                        if (scenario != null)
                        {
                            scenarioId = scenario.Id;
                        }
                        var kpiEconomic = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType &&
                                                                          x.Periode == latestActual.Periode &&
                                                                          x.Kpi.Id == kpi.Id &&
                                                                          x.Scenario.Id == scenarioId)
                                                   .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiEconomic != null && kpiEconomic.Value.HasValue)
                        {
                            seriesResponse.y = kpiEconomic.Value.Value;
                        }
                    }
                }

                if (latestActual != null)
                {
                    switch (request.PeriodeType)
                    {
                        case PeriodeType.Hourly:
                            timeInformation = latestActual.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Daily:
                            timeInformation = latestActual.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Monthly:
                            timeInformation = latestActual.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Yearly:
                            timeInformation = latestActual.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                            break;
                    }
                }

                response.SeriesResponses.Add(seriesResponse);
            }

            response.Subtitle = timeInformation;
            response.Title = request.HeaderTitle;
            return response;
        }

        private double? SumSeries(ArtifactValueInformation valueInformation, PeriodeType periodEType, DateTime start, DateTime end, int kpiId)
        {
            switch (valueInformation)
            {
                case ArtifactValueInformation.Ytd:
                    return
                        DataContext.KpiAchievements.Where(x => x.PeriodeType == periodEType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Ytd ?? 0)).FirstOrDefault();
                case ArtifactValueInformation.Mtd:
                    return
                        DataContext.KpiAchievements.Where(x => x.PeriodeType == periodEType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Mtd ?? 0)).FirstOrDefault();
                default:
                    return DataContext.KpiAchievements.Where(x => x.PeriodeType == periodEType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
            }
        }

        private double? AverageSeries(ArtifactValueInformation valueInformation, PeriodeType periodEType, DateTime start, DateTime end, int kpiId)
        {
            switch (valueInformation)
            {
                case ArtifactValueInformation.Ytd:
                    return
                        DataContext.KpiAchievements.Where(x => x.PeriodeType == periodEType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => (double?)y.Ytd ?? 0)).FirstOrDefault();
                case ArtifactValueInformation.Mtd:
                    return
                        DataContext.KpiAchievements.Where(x => x.PeriodeType == periodEType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => (double?)y.Mtd ?? 0)).FirstOrDefault();
                default:
                    return DataContext.KpiAchievements.Where(x => x.PeriodeType == periodEType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
            }
        }

        public GetSpeedometerChartDataResponse GetSpeedometerChartData(GetSpeedometerChartDataRequest request)
        {
            var response = new GetSpeedometerChartDataResponse();

            var kpi = DataContext.Kpis.Where(x => x.Id == request.Series.KpiId).First();
            IList<DateTime> dateTimePeriodes = new List<DateTime>();
            string timeInformation;
            this.GetPeriodes(request.PeriodeType, request.RangeFilter, request.Start, request.End, out dateTimePeriodes, out timeInformation);
            var start = dateTimePeriodes[0];
            var end = dateTimePeriodes[dateTimePeriodes.Count - 1];

            switch (kpi.YtdFormula)
            {

                case YtdFormula.Average:
                    switch (request.ValueAxis)
                    {
                        case ValueAxis.KpiTarget:
                            response.Series = new GetSpeedometerChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => y.Value).Value).FirstOrDefault()
                            };
                            if (request.LabelSeries != null)
                            {
                                response.LabelSeries = new GetSpeedometerChartDataResponse.SeriesResponse
                                {
                                    name = request.LabelSeries.Label,
                                    data = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.LabelSeries.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => y.Value).Value).FirstOrDefault()
                                };
                            }
                            break;
                        case ValueAxis.KpiActual:
                            response.Series = new GetSpeedometerChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => y.Value).Value).FirstOrDefault()
                            };
                            if (request.LabelSeries != null)
                            {
                                response.LabelSeries = new GetSpeedometerChartDataResponse.SeriesResponse
                                {
                                    name = request.LabelSeries.Label,
                                    data = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.LabelSeries.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => y.Value).Value).FirstOrDefault()
                                };
                            }
                            break;
                        case ValueAxis.KpiEconomic:
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            response.Series = new GetSpeedometerChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId && x.Scenario.Id == scenarioId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => y.Value).Value).FirstOrDefault()
                            };
                            if (request.LabelSeries != null)
                            {
                                response.LabelSeries = new GetSpeedometerChartDataResponse.SeriesResponse
                                {
                                    name = request.LabelSeries.Label,
                                    data = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.LabelSeries.KpiId && x.Scenario.Id == scenarioId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => y.Value).Value).FirstOrDefault()
                                };
                            }
                            break;
                    }
                    break;
                case YtdFormula.Sum:
                default:
                    switch (request.ValueAxis)
                    {
                        case ValueAxis.KpiTarget:
                            response.Series = new GetSpeedometerChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => y.Value).Value).FirstOrDefault()
                            };
                            if (request.LabelSeries != null)
                            {
                                response.LabelSeries = new GetSpeedometerChartDataResponse.SeriesResponse
                                {
                                    name = request.Series.Label,
                                    data = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.LabelSeries.KpiId)
                               .GroupBy(x => x.Kpi.Id)
                               .Select(x => x.Sum(y => y.Value).Value).FirstOrDefault()
                                };
                            }
                            break;
                        case ValueAxis.KpiActual:
                            response.Series = new GetSpeedometerChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => y.Value).Value).FirstOrDefault()
                            };
                            if (request.LabelSeries != null)
                            {
                                response.LabelSeries = new GetSpeedometerChartDataResponse.SeriesResponse
                                {
                                    name = request.Series.Label,
                                    data = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.LabelSeries.KpiId)
                               .GroupBy(x => x.Kpi.Id)
                               .Select(x => x.Sum(y => y.Value).Value).FirstOrDefault()
                                };
                            }
                            break;
                        case ValueAxis.KpiEconomic:
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            response.Series = new GetSpeedometerChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId && x.Scenario.Id == scenarioId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => y.Value).Value).FirstOrDefault()
                            };
                            if (request.LabelSeries != null)
                            {
                                response.LabelSeries = new GetSpeedometerChartDataResponse.SeriesResponse
                                {
                                    name = request.LabelSeries.Label,
                                    data = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.LabelSeries.KpiId && x.Scenario.Id == scenarioId)
                              .GroupBy(x => x.Kpi.Id)
                              .Select(x => x.Sum(y => y.Value).Value).FirstOrDefault()
                                };
                            }
                            break;
                    }
                    break;
            }
            KpiAchievement latestActual = null;
            if (request.ValueAxis == ValueAxis.KpiActual)
            {
                if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                      (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                      (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                      (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear) ||
                     (request.Start.Value == request.End.Value))
                {
                    var actual = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType && x.Periode >= start &&
                  x.Periode <= end && x.Kpi.Id == request.Series.KpiId && (x.Value != null && x.Value.Value != 0))
                  .OrderByDescending(x => x.Periode).FirstOrDefault();
                    if (actual != null)
                    {
                        response.Series = new GetSpeedometerChartDataResponse.SeriesResponse
                        {
                            name = request.Series.Label,
                            data = actual.Value.Value
                        };
                        latestActual = actual;
                    }
                }
            }
            if (request.ValueAxis == ValueAxis.KpiTarget && latestActual != null)
            {
                if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                      (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                      (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                      (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear) ||
                     (request.Start.Value == request.End.Value))
                {
                    var target = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType &&
                  x.Periode == latestActual.Periode && x.Kpi.Id == request.Series.KpiId)
                  .OrderByDescending(x => x.Periode).FirstOrDefault();
                    if (target != null)
                    {
                        response.Series = new GetSpeedometerChartDataResponse.SeriesResponse
                        {
                            name = request.Series.Label,
                            data = target.Value.Value
                        };
                    }
                }
            }
            if (request.ValueAxis == ValueAxis.KpiEconomic && latestActual != null)
            {
                if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                      (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                      (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                      (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear) ||
                     (request.Start.Value == request.End.Value))
                {
                    var scenarioId = 0;
                    var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                    if (scenario != null)
                    {
                        scenarioId = scenario.Id;
                    }
                    var economic = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType &&
                  x.Periode == latestActual.Periode && x.Kpi.Id == request.Series.KpiId && x.Scenario.Id == scenarioId)
                  .OrderByDescending(x => x.Periode).FirstOrDefault();
                    if (economic != null)
                    {
                        response.Series = new GetSpeedometerChartDataResponse.SeriesResponse
                        {
                            name = request.Series.Label,
                            data = economic.Value.Value
                        };
                    }
                }
            }

            if (latestActual != null)
            {
                switch (request.PeriodeType)
                {
                    case PeriodeType.Hourly:
                        timeInformation = latestActual.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                        break;
                    case PeriodeType.Daily:
                        timeInformation = latestActual.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                        break;
                    case PeriodeType.Monthly:
                        timeInformation = latestActual.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                        break;
                    case PeriodeType.Yearly:
                        timeInformation = latestActual.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                        break;
                }
            }

            foreach (var plot in request.PlotBands)
            {
                response.PlotBands.Add(new GetSpeedometerChartDataResponse.PlotBandResponse
                {
                    from = plot.From,
                    to = plot.To,
                    color = plot.Color
                });
            }
            response.Subtitle = timeInformation;
            return response;
        }

        public GetTrafficLightChartDataResponse GetTrafficLightChartData(GetTrafficLightChartDataRequest request)
        {
            var response = new GetTrafficLightChartDataResponse();

            var kpi = DataContext.Kpis.First(x => x.Id == request.Series.KpiId);
            IList<DateTime> dateTimePeriodes = new List<DateTime>();
            string timeInformation;
            this.GetPeriodes(request.PeriodeType, request.RangeFilter, request.Start, request.End, out dateTimePeriodes, out timeInformation);
            var start = dateTimePeriodes[0];
            var end = dateTimePeriodes[dateTimePeriodes.Count - 1];

            switch (kpi.YtdFormula)
            {
                case YtdFormula.Sum:
                    switch (request.ValueAxis)
                    {
                        case ValueAxis.KpiTarget:
                            response.Series = new GetTrafficLightChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => y.Value).Value).FirstOrDefault()
                            };
                            break;
                        case ValueAxis.KpiActual:
                            response.Series = new GetTrafficLightChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => y.Value).Value).FirstOrDefault()
                            };
                            break;
                        case ValueAxis.KpiEconomic:
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            response.Series = new GetTrafficLightChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId && x.Scenario.Id == scenarioId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => y.Value).Value).FirstOrDefault()
                            };
                            break;
                    }
                    break;
                case YtdFormula.Average:
                    switch (request.ValueAxis)
                    {
                        case ValueAxis.KpiTarget:
                            response.Series = new GetTrafficLightChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => y.Value).Value).FirstOrDefault()
                            };
                            break;
                        case ValueAxis.KpiActual:
                            response.Series = new GetTrafficLightChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => y.Value).Value).FirstOrDefault()
                            };
                            break;
                        case ValueAxis.KpiEconomic:
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            response.Series = new GetTrafficLightChartDataResponse.SeriesResponse
                            {
                                name = request.Series.Label,
                                data = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == request.Series.KpiId && x.Scenario.Id == scenarioId)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => y.Value).Value).FirstOrDefault()
                            };
                            break;
                    }
                    break;
            }
            KpiAchievement latestActual = null;
            if (request.ValueAxis == ValueAxis.KpiActual)
            {
                if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                      (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                      (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                      (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear))
                {
                    var actual = DataContext.KpiAchievements.Where(x => x.PeriodeType == request.PeriodeType &&
                  x.Periode <= end && x.Kpi.Id == request.Series.KpiId && (x.Value != null && x.Value.Value != 0))
                  .OrderByDescending(x => x.Periode).FirstOrDefault();
                    if (actual != null)
                    {
                        response.Series = new GetTrafficLightChartDataResponse.SeriesResponse
                        {
                            name = request.Series.Label,
                            data = actual.Value.Value
                        };
                        latestActual = actual;
                    }
                }
            }
            if (request.ValueAxis == ValueAxis.KpiTarget && latestActual != null)
            {
                if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                      (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                      (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                      (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear))
                {
                    var target = DataContext.KpiTargets.Where(x => x.PeriodeType == request.PeriodeType &&
                  x.Periode == latestActual.Periode && x.Kpi.Id == request.Series.KpiId)
                  .OrderByDescending(x => x.Periode).FirstOrDefault();
                    if (target != null)
                    {
                        response.Series = new GetTrafficLightChartDataResponse.SeriesResponse
                        {
                            name = request.Series.Label,
                            data = target.Value.Value
                        };
                    }
                }
            }
            if (request.ValueAxis == ValueAxis.KpiEconomic && latestActual != null)
            {
                if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                      (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                      (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                      (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear))
                {
                    var scenarioId = 0;
                    var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                    if (scenario != null)
                    {
                        scenarioId = scenario.Id;
                    }
                    var economic = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == request.PeriodeType &&
                  x.Periode == latestActual.Periode && x.Kpi.Id == request.Series.KpiId && x.Scenario.Id == scenarioId)
                  .OrderByDescending(x => x.Periode).FirstOrDefault();
                    if (economic != null)
                    {
                        response.Series = new GetTrafficLightChartDataResponse.SeriesResponse
                        {
                            name = request.Series.Label,
                            data = economic.Value.Value
                        };
                    }
                }
            }
            if (latestActual != null)
            {
                switch (request.PeriodeType)
                {
                    case PeriodeType.Hourly:
                        timeInformation = latestActual.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                        break;
                    case PeriodeType.Daily:
                        timeInformation = latestActual.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                        break;
                    case PeriodeType.Monthly:
                        timeInformation = latestActual.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                        break;
                    case PeriodeType.Yearly:
                        timeInformation = latestActual.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                        break;
                }
            }
            foreach (var plot in request.PlotBands)
            {
                response.PlotBands.Add(new GetTrafficLightChartDataResponse.PlotBandResponse
                {
                    from = plot.From,
                    to = plot.To,
                    color = plot.Color,
                    label = plot.Label
                });
            }
            response.Subtitle = timeInformation;
            return response;
        }

        public GetMultiaxisChartDataResponse GetMultiaxisChartData(GetMultiaxisChartDataRequest request)
        {
            var response = new GetMultiaxisChartDataResponse();
            foreach (var chart in request.Charts)
            {
                var chartReq = request.MapTo<GetCartesianChartDataRequest>();
                chart.MapPropertiesToInstance<GetCartesianChartDataRequest>(chartReq);
                var cartesianChartRes = GetChartData(chartReq);
                if (response.Subtitle == null) response.Subtitle = cartesianChartRes.Subtitle;
                if (response.Periodes == null) response.Periodes = cartesianChartRes.Periodes;
                if (response.TimePeriodes == null) response.TimePeriodes = cartesianChartRes.TimePeriodes;
                var multiaxisChart = cartesianChartRes.MapTo<GetMultiaxisChartDataResponse.ChartResponse>();
                multiaxisChart.GraphicType = chartReq.GraphicType;
                multiaxisChart.FractionScale = chart.FractionScale;
                multiaxisChart.MaxFractionScale = chart.MaxFractionScale;
                multiaxisChart.Measurement = DataContext.Measurements.First(x => x.Id == chartReq.MeasurementId).Name;
                multiaxisChart.ValueAxisTitle = chart.ValueAxisTitle;
                multiaxisChart.ValueAxisColor = chart.ValueAxisColor;
                multiaxisChart.IsOpposite = chart.IsOpposite;
                multiaxisChart.SeriesType = cartesianChartRes.SeriesType;
                response.Charts.Add(multiaxisChart);
            }
            return response;
        }

        public GetComboChartDataResponse GetComboChartData(GetComboChartDataRequest request)
        {
            var response = new GetComboChartDataResponse();
            foreach (var chart in request.Charts)
            {
                var chartReq = request.MapTo<GetCartesianChartDataRequest>();
                chart.MapPropertiesToInstance<GetCartesianChartDataRequest>(chartReq);
                var cartesianChartRes = GetChartData(chartReq);
                if (response.Subtitle == null) response.Subtitle = cartesianChartRes.Subtitle;
                if (response.Periodes == null) response.Periodes = cartesianChartRes.Periodes;
                if (response.TimePeriodes == null) response.TimePeriodes = cartesianChartRes.TimePeriodes;
                var comboChart = cartesianChartRes.MapTo<GetComboChartDataResponse.ChartResponse>();
                comboChart.GraphicType = chartReq.GraphicType;
                comboChart.SeriesType = cartesianChartRes.SeriesType;
                response.Charts.Add(comboChart);
                response.Measurement = DataContext.Measurements.First(x => x.Id == chartReq.MeasurementId).Name;
            }
            return response;
        }

        public GetCartesianChartDataResponse GetChartData(GetCartesianChartDataRequest request, bool multiaxisAsOrigin = false)
        {
            var response = new GetCartesianChartDataResponse();
            IList<DateTime> dateTimePeriodes = new List<DateTime>();
            string timeInformation;
            response.Periodes = this.GetPeriodes(request.PeriodeType, request.RangeFilter, request.Start, request.End, out dateTimePeriodes, out timeInformation);
            response.TimePeriodes = dateTimePeriodes;
            response.Subtitle = timeInformation;
            IList<GetCartesianChartDataResponse.SeriesResponse> seriesResponse = new List<GetCartesianChartDataResponse.SeriesResponse>();
            var seriesType = "single-stack";
            if (request.Series.Count == 1 && (request.GraphicType == "baraccumulative" || request.GraphicType == "barachievement"))
            {
                seriesType = "multi-stacks";
            }
            else if (request.Series.Count > 1)
            {
                if (request.Series.Where(x => x.Stacks.Count > 0).FirstOrDefault() != null || request.GraphicType == "baraccumulative" || request.GraphicType == "barachievement" || request.AsNetbackChart)
                {
                    seriesType = "multi-stacks-grouped";
                }
            }
            else
            {
                if (request.Series.Where(x => x.Stacks.Count > 0).FirstOrDefault() != null || request.GraphicType == "baraccumulative")
                {
                    seriesType = "multi-stack";
                }
            }

            //workaround for bar multiaxis bug : multiple stacks
            if (multiaxisAsOrigin = true && request.GraphicType == "bar" && seriesType == "multi-stack")
            {
                seriesType = "multi-stacks-grouped";
            }

            string newTimeInformation;
            IList<DateTime> newDateTimePeriodes;
            switch (request.ValueAxis)
            {
                case ValueAxis.KpiTarget:
                    seriesResponse = this._getKpiTargetSeries(request.Series, request.PeriodeType, dateTimePeriodes, seriesType, request.RangeFilter, request.GraphicType, out newTimeInformation, out newDateTimePeriodes, false, request.AsNetbackChart);
                    break;
                case ValueAxis.KpiActual:
                    seriesResponse = this._getKpiActualSeries(request.Series, request.PeriodeType, dateTimePeriodes, seriesType, request.RangeFilter, request.GraphicType, out newTimeInformation, out newDateTimePeriodes, false, request.AsNetbackChart);
                    break;
                case ValueAxis.KpiEconomic:
                    seriesResponse = this._getKpiEconomicSeries(request.Series, request.PeriodeType, dateTimePeriodes, seriesType, request.RangeFilter, request.GraphicType, out newTimeInformation, out newDateTimePeriodes, false, request.AsNetbackChart);
                    break;
                default:
                    var i = 0;
                    foreach (var series in request.Series)
                    {
                        series.Order = i++;
                    }
                    var actualSeries = request.Series.Where(x => x.ValueAxis == ValueAxis.KpiActual).ToList();
                    var targetSeries = request.Series.Where(x => x.ValueAxis == ValueAxis.KpiTarget).ToList();
                    var economicSeries = request.Series.Where(x => x.ValueAxis == ValueAxis.KpiEconomic).ToList();
                    //seriesType = "multi-stacks-grouped";
                    var series1 = this._getKpiTargetSeries(targetSeries, request.PeriodeType, dateTimePeriodes, seriesType, request.RangeFilter, request.GraphicType, out newTimeInformation, out newDateTimePeriodes, false, request.AsNetbackChart);
                    var series2 = this._getKpiActualSeries(actualSeries, request.PeriodeType, dateTimePeriodes, seriesType, request.RangeFilter, request.GraphicType, out newTimeInformation, out newDateTimePeriodes, false, request.AsNetbackChart);
                    var series3 = this._getKpiEconomicSeries(economicSeries, request.PeriodeType, dateTimePeriodes, seriesType, request.RangeFilter, request.GraphicType, out newTimeInformation, out newDateTimePeriodes, false, request.AsNetbackChart);
                    seriesResponse = series1.Concat(series2).Concat(series3).ToList();
                    seriesResponse = seriesResponse.OrderBy(x => x.Order).ToList();
                    break;

            }
            if ((request.PeriodeType == PeriodeType.Hourly && request.RangeFilter == RangeFilter.CurrentHour) ||
                     (request.PeriodeType == PeriodeType.Daily && request.RangeFilter == RangeFilter.CurrentDay) ||
                     (request.PeriodeType == PeriodeType.Monthly && request.RangeFilter == RangeFilter.CurrentMonth) ||
                     (request.PeriodeType == PeriodeType.Yearly && request.RangeFilter == RangeFilter.CurrentYear))
            {
                response.Subtitle = newTimeInformation;
                if (newDateTimePeriodes.Count > 0)
                {
                    switch (request.PeriodeType)
                    {
                        case PeriodeType.Hourly:
                            response.Periodes = new List<string> { newDateTimePeriodes.First().ToString("hh tt", CultureInfo.InvariantCulture) }.ToArray();
                            //timeInformation = kpiActual.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Daily:
                            response.Periodes = new List<string> { newDateTimePeriodes.First().ToString("dd", CultureInfo.InvariantCulture) }.ToArray();
                            //timeInformation = kpiActual.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Monthly:
                            response.Periodes = new List<string> { newDateTimePeriodes.First().ToString("MMMM", CultureInfo.InvariantCulture) }.ToArray();
                            //timeInformation = kpiActual.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Yearly:
                            response.Periodes = new List<string> { newDateTimePeriodes.First().ToString(DateFormat.Yearly, CultureInfo.InvariantCulture) }.ToArray();
                            //timeInformation = kpiActual.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                            break;
                    }
                }

            }
            if (request.AsNetbackChart)
            {
                response.Subtitle = newTimeInformation;
                if (newDateTimePeriodes.Count > 0)
                {
                    switch (request.PeriodeType)
                    {
                        case PeriodeType.Hourly:
                            response.Periodes = new List<string> { newDateTimePeriodes.First().ToString("hh tt", CultureInfo.InvariantCulture) + " - " + newDateTimePeriodes.Last().ToString("hh tt", CultureInfo.InvariantCulture) }.ToArray();
                            //timeInformation = kpiActual.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Daily:
                            response.Periodes = new List<string> { newDateTimePeriodes.First().ToString("dd", CultureInfo.InvariantCulture) + " - " + newDateTimePeriodes.Last().ToString("dd", CultureInfo.InvariantCulture) }.ToArray();
                            //timeInformation = kpiActual.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Monthly:
                            response.Periodes = new List<string> { newDateTimePeriodes.First().ToString("MMMM", CultureInfo.InvariantCulture) + " - " + newDateTimePeriodes.Last().ToString("MMMM", CultureInfo.InvariantCulture) }.ToArray();
                            //timeInformation = kpiActual.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Yearly:
                            response.Periodes = new List<string> { newDateTimePeriodes.First().ToString(DateFormat.Yearly, CultureInfo.InvariantCulture) + " - " + newDateTimePeriodes.Last().ToString(DateFormat.Yearly, CultureInfo.InvariantCulture) }.ToArray();
                            //timeInformation = kpiActual.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                            break;
                    }
                }
            }

            response.SeriesType = seriesType;
            response.Series = seriesResponse;
            return response;
        }

        public string[] GetPeriodes(PeriodeType periodeType, RangeFilter rangeFilter, DateTime? Start, DateTime? End, out IList<DateTime> dateTimePeriodes, out string timeInformation) //, out string timeInformation
        {
            //var ci = new CultureInfo("en-GB");
            var periodes = new List<string>();
            dateTimePeriodes = new List<DateTime>();
            switch (periodeType)
            {
                case PeriodeType.Hourly:
                    var hourlyFormat = "hh tt";
                    switch (rangeFilter)
                    {
                        case RangeFilter.CurrentHour:
                            {
                                var currentHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
                                dateTimePeriodes.Add(currentHour);
                                periodes.Add(currentHour.ToString(hourlyFormat));
                                timeInformation = currentHour.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            }
                            break;
                        case RangeFilter.CurrentDay:
                            {
                                var startHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                                periodes.Add(startHour.ToString(hourlyFormat));
                                dateTimePeriodes.Add(startHour);
                                for (double i = 1; i < 24; i++)
                                {
                                    startHour = startHour.AddHours(1);
                                    periodes.Add(startHour.ToString(hourlyFormat));
                                    dateTimePeriodes.Add(startHour);
                                }
                                timeInformation = startHour.AddHours(-1).ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            }
                            break;
                        case RangeFilter.DTD:
                            {
                                //var currentDay = DateTime.Now.Day;
                                var startHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                                var currentHour = DateTime.Now;
                                timeInformation = startHour.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                while (startHour <= currentHour)
                                {
                                    periodes.Add(startHour.ToString(hourlyFormat));
                                    dateTimePeriodes.Add(startHour);
                                    startHour = startHour.AddHours(1);
                                }
                                timeInformation += " - " + startHour.AddHours(-1).ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                            }
                            break;
                        default:
                            timeInformation = Start.Value.ToString("dd MMM yy", CultureInfo.InvariantCulture) + " - " + End.Value.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            while (Start.Value <= End.Value)
                            {
                                periodes.Add(Start.Value.ToString(hourlyFormat));
                                dateTimePeriodes.Add(Start.Value);
                                Start = Start.Value.AddHours(1);
                            }
                            break;
                    }
                    break;
                case PeriodeType.Daily:
                    var dailyFormat = "dd MMM";
                    switch (rangeFilter)
                    {
                        case RangeFilter.CurrentDay:
                            {
                                var currentDay = DateTime.Now.Date;
                                periodes.Add(currentDay.ToString(dailyFormat));
                                dateTimePeriodes.Add(currentDay);
                                timeInformation = currentDay.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            }
                            break;
                        case RangeFilter.CurrentWeek:
                            {
                                var startDay = StartOfWeek();
                                var endDay = startDay.AddDays(6);
                                timeInformation = startDay.ToString("dd MMM yy", CultureInfo.InvariantCulture) + " - " + endDay.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                while (startDay <= endDay)
                                {
                                    periodes.Add(startDay.ToString(dailyFormat));
                                    dateTimePeriodes.Add(startDay);
                                    startDay = startDay.AddDays(1);
                                }

                                break;
                            }
                        case RangeFilter.CurrentMonth:
                            {
                                var currentMonth = DateTime.Now.Month;
                                var startDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                while (currentMonth == startDay.Month)
                                {
                                    periodes.Add(startDay.ToString(dailyFormat));
                                    dateTimePeriodes.Add(startDay);
                                    startDay = startDay.AddDays(1);
                                }
                                timeInformation = startDay.AddDays(-1).ToString("MMM yy", CultureInfo.InvariantCulture);
                            }
                            break;
                        case RangeFilter.MTD:
                            {
                                var currentMonth = DateTime.Now.Month;
                                var startDay = new DateTime(DateTime.Now.Year, currentMonth, 1);
                                var currentDay = DateTime.Now;
                                timeInformation = startDay.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                while (startDay <= currentDay)
                                {
                                    periodes.Add(startDay.ToString(dailyFormat));
                                    dateTimePeriodes.Add(startDay);
                                    startDay = startDay.AddDays(1);
                                }
                                timeInformation += " - " + startDay.AddDays(-1).ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            }
                            break;
                        case RangeFilter.YTD:
                            {
                                var startDay = new DateTime(DateTime.Now.Year, 1, 1);
                                var endDay = DateTime.Now;
                                timeInformation = startDay.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                while (startDay <= endDay)
                                {
                                    periodes.Add(startDay.ToString(dailyFormat));
                                    dateTimePeriodes.Add(startDay);
                                    startDay = startDay.AddDays(1);
                                }
                                timeInformation += " - " + startDay.AddDays(-1).ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            }
                            break;
                        default:
                            timeInformation = Start.Value.ToString("dd MMM yy", CultureInfo.InvariantCulture) + " - " + End.Value.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            while (Start.Value <= End.Value)
                            {
                                periodes.Add(Start.Value.ToString(dailyFormat));
                                dateTimePeriodes.Add(Start.Value);
                                Start = Start.Value.AddDays(1);
                            }
                            break;

                    }
                    break;
                case PeriodeType.Monthly:
                    var monthlyFormat = "MMM";
                    switch (rangeFilter)
                    {
                        case RangeFilter.CurrentMonth:
                            {
                                var currentMonth = DateTime.Now.Date;
                                dateTimePeriodes.Add(currentMonth);
                                periodes.Add(currentMonth.ToString(monthlyFormat, CultureInfo.InvariantCulture));
                                timeInformation = currentMonth.ToString("MMM yy", CultureInfo.InvariantCulture);
                            }
                            break;
                        case RangeFilter.CurrentYear:
                            {
                                var currentYear = DateTime.Now.Year;
                                var startMonth = new DateTime(DateTime.Now.Year, 1, 1);
                                timeInformation = currentYear.ToString();
                                while (currentYear == startMonth.Year)
                                {
                                    periodes.Add(startMonth.ToString(monthlyFormat));
                                    dateTimePeriodes.Add(startMonth);
                                    startMonth = startMonth.AddMonths(1);
                                }
                            }
                            break;
                        case RangeFilter.YTD:
                            {
                                var currentYear = DateTime.Now.Year;
                                var startMonth = new DateTime(DateTime.Now.Year, 1, 1);
                                var currentMont = DateTime.Now;
                                timeInformation = startMonth.ToString("MMM yy", CultureInfo.InvariantCulture);
                                while (startMonth <= currentMont)
                                {
                                    periodes.Add(startMonth.ToString(monthlyFormat));
                                    dateTimePeriodes.Add(startMonth);
                                    startMonth = startMonth.AddMonths(1);
                                }
                                timeInformation += " - " + startMonth.AddMonths(-1).ToString("MMM yy", CultureInfo.InvariantCulture);
                            }
                            break;
                        default:
                            timeInformation = Start.Value.ToString("MMM yy", CultureInfo.InvariantCulture) + " - " + End.Value.ToString("MMM yy", CultureInfo.InvariantCulture);
                            while (Start.Value <= End.Value)
                            {
                                dateTimePeriodes.Add(Start.Value);
                                periodes.Add(Start.Value.ToString(monthlyFormat));
                                Start = Start.Value.AddMonths(1);
                            }
                            break;
                    }
                    break;
                default:
                    var yearlyFormat = DateFormat.Yearly;
                    switch (rangeFilter)
                    {
                        case RangeFilter.CurrentYear:
                            periodes.Add(DateTime.Now.Year.ToString());
                            dateTimePeriodes.Add(new DateTime(DateTime.Now.Year, 1, 1));
                            timeInformation = DateTime.Now.Year.ToString();
                            break;
                        case RangeFilter.AllExistingYears:
                            timeInformation = "All Existing Years";
                            break;
                        default:
                            timeInformation = Start.Value.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture) + " - " + End.Value.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                            while (Start.Value <= End.Value)
                            {
                                periodes.Add(Start.Value.ToString(yearlyFormat));
                                dateTimePeriodes.Add(Start.Value);
                                Start = Start.Value.AddYears(1);
                            }
                            break;
                    }
                    break;
            }

            return periodes.ToArray();
        }

        private IList<GetCartesianChartDataResponse.SeriesResponse> _getKpiTargetSeries(IList<GetCartesianChartDataRequest.SeriesRequest> configSeries, PeriodeType periodeType, IList<DateTime> dateTimePeriodes, string seriesType, RangeFilter rangeFilter, string graphicType, out string newTimeInformation, out IList<DateTime> newDatetimePeriodes, bool comparison = false, bool asNetbackChart = false)
        {
            var seriesResponse = new List<GetCartesianChartDataResponse.SeriesResponse>();
            var start = rangeFilter == RangeFilter.AllExistingYears ? DateTime.MinValue : dateTimePeriodes[0];
            var end = rangeFilter == RangeFilter.AllExistingYears ? DateTime.MaxValue : dateTimePeriodes[dateTimePeriodes.Count - 1];
            newTimeInformation = null;
            newDatetimePeriodes = new List<DateTime>();
            foreach (var series in configSeries)
            {

                if (series.Stacks.Count == 0)
                {
                    var kpiTargets = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType &&
                      x.Periode >= start && x.Periode <= end && x.Kpi.Id == series.KpiId)
                      .OrderBy(x => x.Periode).ToList();

                    if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                        (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                        (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                        (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                    {
                        var kpiTarget = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType &&
                      x.Periode <= end && x.Kpi.Id == series.KpiId && (x.Value != null && x.Value.Value != 0))
                      .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiTarget != null)
                        {
                            kpiTargets = new List<KpiTarget> { kpiTarget };
                            switch (periodeType)
                            {
                                case PeriodeType.Hourly:
                                    newTimeInformation = kpiTarget.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                    break;
                                case PeriodeType.Daily:
                                    newTimeInformation = kpiTarget.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                    break;
                                case PeriodeType.Monthly:
                                    newTimeInformation = kpiTarget.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                                    break;
                                case PeriodeType.Yearly:
                                    newTimeInformation = kpiTarget.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                                    break;
                            }
                            dateTimePeriodes = new List<DateTime> { kpiTarget.Periode };
                            newDatetimePeriodes = dateTimePeriodes;
                        }

                    }

                    if (seriesType == "multi-stacks-grouped")
                    {
                        var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = series.Label,
                            Stack = series.Label,
                            Color = series.Color,
                            Order = series.Order,
                            MarkerColor = series.MarkerColor,
                            LineType = series.LineType
                        };
                        if (asNetbackChart)
                        {
                            var sumValue = kpiTargets.Where(x => x.Value.HasValue).Average(x => x.Value);
                            aSeries.BorderColor = "transparent";
                            aSeries.Data.Add(sumValue);
                            if (newTimeInformation == null && newDatetimePeriodes.Count == 0)
                            {
                                if (rangeFilter == RangeFilter.AllExistingYears)
                                {
                                    newTimeInformation = "2011 - 2030";
                                    newDatetimePeriodes = new List<DateTime> { new DateTime(2011, 1, 1, 0, 0, 0), new DateTime(2030, 1, 1, 0, 0, 0) };
                                }
                                else
                                {
                                    switch (periodeType)
                                    {
                                        case PeriodeType.Hourly:
                                            newTimeInformation = start.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture) + " - " + end.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                            break;
                                        case PeriodeType.Daily:
                                            newTimeInformation = start.ToString("dd MMM yy", CultureInfo.InvariantCulture) + " - " + end.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                            break;
                                        case PeriodeType.Monthly:
                                            newTimeInformation = start.ToString("MMM yy", CultureInfo.InvariantCulture) + " - " + end.ToString("MMM yy", CultureInfo.InvariantCulture);
                                            break;
                                        case PeriodeType.Yearly:
                                            newTimeInformation = start.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture) + " - " + end.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                                            break;
                                    }
                                    dateTimePeriodes = new List<DateTime> { start, end };
                                    newDatetimePeriodes = dateTimePeriodes;
                                }
                            }

                        }
                        else
                            //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                            //   {

                            //       foreach (var periode in dateTimePeriodes)
                            //       {
                            //           var targetValue = kpiTargets.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                            //               .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                            //           if (targetValue == null || !targetValue.HasValue)
                            //           {
                            //               aSeries.Data.Add(null);
                            //           }
                            //           else
                            //           {
                            //               aSeries.Data.Add(targetValue.Value);
                            //           }
                            //       }
                            //   }
                            //   else
                            //   {
                            foreach (var periode in dateTimePeriodes)
                            {
                                var target = kpiTargets.Where(x => x.Periode == periode).FirstOrDefault();
                                if (target == null || !target.Value.HasValue)
                                {
                                    aSeries.Data.Add(null);
                                }
                                else
                                {
                                    aSeries.Data.Add(target.Value.Value);
                                }
                            }
                        //}


                        if (graphicType == "baraccumulative")
                        {
                            var previousSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = "Previous Accumulation",
                                Color = string.IsNullOrEmpty(series.PreviousColor) ? "#004071" : series.PreviousColor,
                                Stack = series.Label,
                                Order = series.Order
                            };
                            for (var i = 0; i < aSeries.Data.Count; i++)
                            {
                                double data = 0;
                                for (var j = 0; j < i; j++)
                                {
                                    data += aSeries.Data[j].HasValue ? aSeries.Data[j].Value : 0;
                                }
                                previousSeries.Data.Add(data);
                            }
                            seriesResponse.Add(previousSeries);
                        }
                        seriesResponse.Add(aSeries);
                        if (asNetbackChart && seriesResponse.Count > 1)
                        {
                            var invicibleSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = "invisible_" + series.Label,
                                Stack = series.Label,
                                Color = "transparent",
                                BorderColor = "transparent",
                                ShowInLegend = false
                            };
                            invicibleSeries.Data.Add(seriesResponse[seriesResponse.Count - 2].Data[0] - seriesResponse[seriesResponse.Count - 1].Data[0]);
                            seriesResponse.Add(invicibleSeries);
                        }
                    }
                    else
                    {
                        var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = series.Label,
                            Color = series.Color,
                            Order = series.Order,
                            MarkerColor = series.MarkerColor,
                            LineType = series.LineType
                        };
                        if (comparison)
                        {
                            aSeries.Stack = "KpiTarget";
                        }
                        //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                        //{

                        //    foreach (var periode in dateTimePeriodes)
                        //    {
                        //        var targetValue = kpiTargets.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                        //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                        //        if (targetValue == null || !targetValue.HasValue)
                        //        {
                        //            aSeries.Data.Add(null);
                        //        }
                        //        else
                        //        {
                        //            aSeries.Data.Add(targetValue.Value);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        foreach (var periode in dateTimePeriodes)
                        {
                            var target = kpiTargets.Where(x => x.Periode == periode).FirstOrDefault();
                            if (target == null || !target.Value.HasValue)
                            {
                                aSeries.Data.Add(null);
                            }
                            else
                            {
                                aSeries.Data.Add(target.Value.Value);
                            }
                        }
                        //}

                        if (graphicType == "baraccumulative")
                        {
                            var previousSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = "Previous Accumulation",
                                Color = string.IsNullOrEmpty(series.PreviousColor) ? "#004071" : series.PreviousColor,
                                Order = series.Order
                            };
                            if (comparison)
                            {
                                previousSeries.Stack = "KpiTarget";
                            }
                            for (var i = 0; i < aSeries.Data.Count; i++)
                            {
                                double data = 0;
                                for (var j = 0; j < i; j++)
                                {
                                    data += aSeries.Data[j].HasValue ? aSeries.Data[j].Value : 0;
                                }
                                previousSeries.Data.Add(data);
                            }
                            seriesResponse.Add(previousSeries);
                        }
                        seriesResponse.Add(aSeries);
                    }

                }
                else
                {
                    foreach (var stack in series.Stacks)
                    {
                        var kpiTargets = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType &&
                        x.Periode >= start && x.Periode <= end && x.Kpi.Id == stack.KpiId)
                        .OrderBy(x => x.Periode).ToList();

                        if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                        (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                        (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                        (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                        {
                            var kpiTarget = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType &&
                          x.Periode <= end && x.Kpi.Id == stack.KpiId && (x.Value != null && x.Value.Value != 0))
                          .OrderByDescending(x => x.Periode).FirstOrDefault();
                            if (kpiTarget != null)
                            {
                                kpiTargets = new List<KpiTarget> { kpiTarget };
                                switch (periodeType)
                                {
                                    case PeriodeType.Hourly:
                                        newTimeInformation = kpiTarget.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                        break;
                                    case PeriodeType.Daily:
                                        newTimeInformation = kpiTarget.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                        break;
                                    case PeriodeType.Monthly:
                                        newTimeInformation = kpiTarget.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                                        break;
                                    case PeriodeType.Yearly:
                                        newTimeInformation = kpiTarget.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                                        break;
                                }
                                dateTimePeriodes = new List<DateTime> { kpiTarget.Periode };
                                newDatetimePeriodes = dateTimePeriodes;
                            }

                        }

                        if (seriesType == "multi-stacks-grouped")
                        {
                            var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = stack.Label,
                                Stack = series.Label,
                                Color = stack.Color,
                                Order = series.Order,
                                MarkerColor = series.MarkerColor,
                                LineType = series.LineType
                            };
                            //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                            //{

                            //    foreach (var periode in dateTimePeriodes)
                            //    {
                            //        var targetValue = kpiTargets.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                            //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                            //        if (targetValue == null || !targetValue.HasValue)
                            //        {
                            //            aSeries.Data.Add(null);
                            //        }
                            //        else
                            //        {
                            //            aSeries.Data.Add(targetValue.Value);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            foreach (var periode in dateTimePeriodes)
                            {
                                var target = kpiTargets.Where(x => x.Periode == periode).FirstOrDefault();
                                if (target == null || !target.Value.HasValue)
                                {
                                    aSeries.Data.Add(null);
                                }
                                else
                                {
                                    aSeries.Data.Add(target.Value.Value);
                                }
                            }
                            //}
                            seriesResponse.Add(aSeries);
                        }
                        else
                        {

                            var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = stack.Label,
                                Color = stack.Color,
                                Order = series.Order,
                                MarkerColor = series.MarkerColor,
                                LineType = series.LineType
                            };
                            if (comparison)
                            {
                                aSeries.Stack = "KpiTarget";
                            }
                            //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                            //{

                            //    foreach (var periode in dateTimePeriodes)
                            //    {
                            //        var targetValue = kpiTargets.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                            //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                            //        if (targetValue == null || !targetValue.HasValue)
                            //        {
                            //            aSeries.Data.Add(null);
                            //        }
                            //        else
                            //        {
                            //            aSeries.Data.Add(targetValue.Value);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            foreach (var periode in dateTimePeriodes)
                            {
                                var target = kpiTargets.Where(x => x.Periode == periode).FirstOrDefault();
                                if (target == null || !target.Value.HasValue)
                                {
                                    aSeries.Data.Add(null);
                                }
                                else
                                {
                                    aSeries.Data.Add(target.Value.Value);
                                }
                            }
                            //}
                            seriesResponse.Add(aSeries);
                        }
                    }
                }
            }
            return seriesResponse;
        }

        private IList<GetCartesianChartDataResponse.SeriesResponse> _getKpiEconomicSeries(IList<GetCartesianChartDataRequest.SeriesRequest> configSeries, PeriodeType periodeType, IList<DateTime> dateTimePeriodes, string seriesType, RangeFilter rangeFilter, string graphicType, out string newTimeInformation, out IList<DateTime> newDatetimePeriodes, bool comparison = false, bool asNetbackChart = false)
        {
            var seriesResponse = new List<GetCartesianChartDataResponse.SeriesResponse>();
            var start = rangeFilter == RangeFilter.AllExistingYears ? DateTime.MinValue : dateTimePeriodes[0];
            var end = rangeFilter == RangeFilter.AllExistingYears ? DateTime.MaxValue : dateTimePeriodes[dateTimePeriodes.Count - 1];
            var scenarioId = 0;
            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
            if (scenario != null)
            {
                scenarioId = scenario.Id;
            }
            newTimeInformation = null;
            newDatetimePeriodes = new List<DateTime>();
            foreach (var series in configSeries)
            {

                if (series.Stacks.Count == 0)
                {
                    IList<KeyOperationData> kpiEconomics = new List<KeyOperationData>();
                    if (rangeFilter == RangeFilter.AllExistingYears)
                    {
                        kpiEconomics = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == periodeType &&
                            x.Kpi.Id == series.KpiId && x.Scenario.Id == scenarioId)
                    .OrderBy(x => x.Periode).ToList();
                    }
                    else
                    {
                        kpiEconomics = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == periodeType &&
                          x.Periode >= start && x.Periode <= end && x.Kpi.Id == series.KpiId && x.Scenario.Id == scenarioId)
                          .OrderBy(x => x.Periode).ToList();
                    }

                    if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                        (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                        (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                        (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                    {
                        var kpiEconomic = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == periodeType &&
                      x.Periode <= end && x.Kpi.Id == series.KpiId && (x.Value != null && x.Value.Value != 0) && x.Scenario.Id == scenarioId)
                      .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiEconomic != null)
                        {
                            kpiEconomics = new List<KeyOperationData> { kpiEconomic };
                            switch (periodeType)
                            {
                                case PeriodeType.Hourly:
                                    newTimeInformation = kpiEconomic.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                    break;
                                case PeriodeType.Daily:
                                    newTimeInformation = kpiEconomic.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                    break;
                                case PeriodeType.Monthly:
                                    newTimeInformation = kpiEconomic.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                                    break;
                                case PeriodeType.Yearly:
                                    newTimeInformation = kpiEconomic.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                                    break;
                            }
                            dateTimePeriodes = new List<DateTime> { kpiEconomic.Periode };
                            newDatetimePeriodes = dateTimePeriodes;
                        }

                    }

                    if (seriesType == "multi-stacks-grouped")
                    {
                        var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = series.Label,
                            Stack = series.Label,
                            Color = series.Color,
                            Order = series.Order,
                            MarkerColor = series.MarkerColor,
                            LineType = series.LineType
                        };
                        if (asNetbackChart)
                        {
                            var sumValue = kpiEconomics.Where(x => x.Value.HasValue).Average(x => x.Value);
                            aSeries.BorderColor = "transparent";
                            aSeries.Data.Add(sumValue);
                            if (newTimeInformation == null && newDatetimePeriodes.Count == 0)
                            {
                                if (rangeFilter == RangeFilter.AllExistingYears)
                                {
                                    newTimeInformation = "2011 - 2030";
                                    newDatetimePeriodes = new List<DateTime> { new DateTime(2011, 1, 1, 0, 0, 0), new DateTime(2030, 1, 1, 0, 0, 0) };
                                }
                                else
                                {
                                    switch (periodeType)
                                    {
                                        case PeriodeType.Hourly:
                                            newTimeInformation = start.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture) + " - " + end.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                            break;
                                        case PeriodeType.Daily:
                                            newTimeInformation = start.ToString("dd MMM yy", CultureInfo.InvariantCulture) + " - " + end.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                            break;
                                        case PeriodeType.Monthly:
                                            newTimeInformation = start.ToString("MMM yy", CultureInfo.InvariantCulture) + " - " + end.ToString("MMM yy", CultureInfo.InvariantCulture);
                                            break;
                                        case PeriodeType.Yearly:
                                            newTimeInformation = start.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture) + " - " + end.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                                            break;
                                    }
                                    dateTimePeriodes = new List<DateTime> { start, end };
                                    newDatetimePeriodes = dateTimePeriodes;
                                }
                            }

                        }
                        //else if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                        //{

                        //    foreach (var periode in dateTimePeriodes)
                        //    {
                        //        var economicValue = kpiEconomics.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                        //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                        //        if (economicValue == null || !economicValue.HasValue)
                        //        {
                        //            aSeries.Data.Add(null);
                        //        }
                        //        else
                        //        {
                        //            aSeries.Data.Add(economicValue.Value);
                        //        }
                        //    }

                        //}
                        else
                        {
                            foreach (var periode in dateTimePeriodes)
                            {
                                var target = kpiEconomics.Where(x => x.Periode == periode).FirstOrDefault();
                                if (target == null || !target.Value.HasValue)
                                {
                                    aSeries.Data.Add(null);
                                }
                                else
                                {
                                    aSeries.Data.Add(target.Value.Value);
                                }
                            }
                        }


                        if (graphicType == "baraccumulative")
                        {
                            var previousSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = "Previous Accumulation",
                                Color = string.IsNullOrEmpty(series.PreviousColor) ? "#004071" : series.PreviousColor,
                                Stack = series.Label,
                                Order = series.Order
                            };
                            for (var i = 0; i < aSeries.Data.Count; i++)
                            {
                                double data = 0;
                                for (var j = 0; j < i; j++)
                                {
                                    data += aSeries.Data[j].HasValue ? aSeries.Data[j].Value : 0;
                                }
                                previousSeries.Data.Add(data);
                            }
                            seriesResponse.Add(previousSeries);
                        }
                        seriesResponse.Add(aSeries);
                        if (asNetbackChart && seriesResponse.Count > 1)
                        {
                            var invicibleSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = "invisible_" + series.Label,
                                Stack = series.Label,
                                Color = "transparent",
                                BorderColor = "transparent",
                                ShowInLegend = false
                            };
                            invicibleSeries.Data.Add(seriesResponse[seriesResponse.Count - 2].Data[0] - seriesResponse[seriesResponse.Count - 1].Data[0]);
                            seriesResponse.Add(invicibleSeries);
                        }
                    }
                    else
                    {
                        var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = series.Label,
                            Color = series.Color,
                            Order = series.Order,
                            MarkerColor = series.MarkerColor,
                            LineType = series.LineType
                        };
                        if (comparison)
                        {
                            aSeries.Stack = "KpiTarget";
                        }
                        //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                        //{

                        //    foreach (var periode in dateTimePeriodes)
                        //    {
                        //        var targetValue = kpiEconomics.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                        //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                        //        if (targetValue == null || !targetValue.HasValue)
                        //        {
                        //            aSeries.Data.Add(null);
                        //        }
                        //        else
                        //        {
                        //            aSeries.Data.Add(targetValue.Value);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        foreach (var periode in dateTimePeriodes)
                        {
                            var target = kpiEconomics.Where(x => x.Periode == periode).FirstOrDefault();
                            if (target == null || !target.Value.HasValue)
                            {
                                aSeries.Data.Add(null);
                            }
                            else
                            {
                                aSeries.Data.Add(target.Value.Value);
                            }
                        }
                        //}

                        if (graphicType == "baraccumulative")
                        {
                            var previousSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = "Previous Accumulation",
                                Color = string.IsNullOrEmpty(series.PreviousColor) ? "#004071" : series.PreviousColor,
                                Order = series.Order
                            };
                            if (comparison)
                            {
                                previousSeries.Stack = "KpiTarget";
                            }
                            for (var i = 0; i < aSeries.Data.Count; i++)
                            {
                                double data = 0;
                                for (var j = 0; j < i; j++)
                                {
                                    data += aSeries.Data[j].HasValue ? aSeries.Data[j].Value : 0;
                                }
                                previousSeries.Data.Add(data);
                            }
                            seriesResponse.Add(previousSeries);
                        }
                        seriesResponse.Add(aSeries);
                    }

                }
                else
                {
                    foreach (var stack in series.Stacks)
                    {
                        var kpiEconomics = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == periodeType &&
                        x.Periode >= start && x.Periode <= end && x.Kpi.Id == stack.KpiId && x.Scenario.Id == scenarioId)
                        .OrderBy(x => x.Periode).ToList();

                        if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                        (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                        (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                        (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                        {
                            var kpiEconomic = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == periodeType &&
                          x.Periode <= end && x.Kpi.Id == stack.KpiId && (x.Value != null && x.Value.Value != 0))
                          .OrderByDescending(x => x.Periode).FirstOrDefault();
                            if (kpiEconomic != null)
                            {
                                kpiEconomics = new List<KeyOperationData> { kpiEconomic };
                                switch (periodeType)
                                {
                                    case PeriodeType.Hourly:
                                        newTimeInformation = kpiEconomic.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                        break;
                                    case PeriodeType.Daily:
                                        newTimeInformation = kpiEconomic.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                        break;
                                    case PeriodeType.Monthly:
                                        newTimeInformation = kpiEconomic.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                                        break;
                                    case PeriodeType.Yearly:
                                        newTimeInformation = kpiEconomic.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                                        break;
                                }
                                dateTimePeriodes = new List<DateTime> { kpiEconomic.Periode };
                                newDatetimePeriodes = dateTimePeriodes;
                            }

                        }

                        if (seriesType == "multi-stacks-grouped")
                        {
                            var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = stack.Label,
                                Stack = series.Label,
                                Color = stack.Color,
                                Order = series.Order,
                                MarkerColor = series.MarkerColor,
                                LineType = series.LineType
                            };
                            //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                            //{

                            //    foreach (var periode in dateTimePeriodes)
                            //    {
                            //        var economicValue = kpiEconomics.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                            //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                            //        if (economicValue == null || !economicValue.HasValue)
                            //        {
                            //            aSeries.Data.Add(null);
                            //        }
                            //        else
                            //        {
                            //            aSeries.Data.Add(economicValue.Value);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            foreach (var periode in dateTimePeriodes)
                            {
                                var economic = kpiEconomics.Where(x => x.Periode == periode).FirstOrDefault();
                                if (economic == null || !economic.Value.HasValue)
                                {
                                    aSeries.Data.Add(null);
                                }
                                else
                                {
                                    aSeries.Data.Add(economic.Value.Value);
                                }
                            }
                            //}
                            seriesResponse.Add(aSeries);
                        }
                        else
                        {

                            var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = stack.Label,
                                Color = stack.Color,
                                Order = series.Order,
                                MarkerColor = series.MarkerColor,
                                LineType = series.LineType
                            };
                            if (comparison)
                            {
                                aSeries.Stack = "KpiTarget";
                            }
                            //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                            //{

                            //    foreach (var periode in dateTimePeriodes)
                            //    {
                            //        var economicValue = kpiEconomics.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                            //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                            //        if (economicValue == null || !economicValue.HasValue)
                            //        {
                            //            aSeries.Data.Add(null);
                            //        }
                            //        else
                            //        {
                            //            aSeries.Data.Add(economicValue.Value);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            foreach (var periode in dateTimePeriodes)
                            {
                                var economic = kpiEconomics.Where(x => x.Periode == periode).FirstOrDefault();
                                if (economic == null || !economic.Value.HasValue)
                                {
                                    aSeries.Data.Add(null);
                                }
                                else
                                {
                                    aSeries.Data.Add(economic.Value.Value);
                                }
                            }
                            //}
                            seriesResponse.Add(aSeries);
                        }
                    }
                }
            }
            return seriesResponse;
        }

        private IList<GetCartesianChartDataResponse.SeriesResponse> _getKpiActualSeries(IList<GetCartesianChartDataRequest.SeriesRequest> configSeries, PeriodeType periodeType, IList<DateTime> dateTimePeriodes, string seriesType, RangeFilter rangeFilter, string graphicType, out string newTimeInformation, out IList<DateTime> newDatetimePeriodes, bool comparison = false, bool asNetbackChart = false)
        {
            var seriesResponse = new List<GetCartesianChartDataResponse.SeriesResponse>();
            var start = rangeFilter == RangeFilter.AllExistingYears ? DateTime.MinValue : dateTimePeriodes[0];
            var end = rangeFilter == RangeFilter.AllExistingYears ? DateTime.MaxValue : dateTimePeriodes[dateTimePeriodes.Count - 1];
            newTimeInformation = null;
            newDatetimePeriodes = new List<DateTime>();
            foreach (var series in configSeries)
            {

                if (series.Stacks.Count == 0)
                {
                    var kpiActuals = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                      x.Periode >= start && x.Periode <= end && x.Kpi.Id == series.KpiId)
                      .OrderBy(x => x.Periode).ToList();

                    if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                       (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                       (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                       (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                    {
                        var kpiActual = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                      x.Periode <= end && x.Kpi.Id == series.KpiId && (x.Value != null && x.Value.Value != 0))
                      .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiActual != null)
                        {
                            kpiActuals = new List<KpiAchievement> { kpiActual };
                            switch (periodeType)
                            {
                                case PeriodeType.Hourly:
                                    newTimeInformation = kpiActual.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                    break;
                                case PeriodeType.Daily:
                                    newTimeInformation = kpiActual.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                    break;
                                case PeriodeType.Monthly:
                                    newTimeInformation = kpiActual.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                                    break;
                                case PeriodeType.Yearly:
                                    newTimeInformation = kpiActual.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                                    break;
                            }
                            dateTimePeriodes = new List<DateTime> { kpiActual.Periode };
                            newDatetimePeriodes = dateTimePeriodes;

                        }
                    }

                    if (seriesType == "multi-stacks-grouped" && graphicType == "baraccumulative")
                    {
                        var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = series.Label,
                            Stack = series.Label,
                            Color = series.Color,
                            Order = series.Order
                        };
                        //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                        //{

                        //    foreach (var periode in dateTimePeriodes)
                        //    {
                        //        var targetValue = kpiActuals.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                        //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                        //        if (targetValue == null || !targetValue.HasValue)
                        //        {
                        //            aSeries.Data.Add(null);
                        //        }
                        //        else
                        //        {
                        //            aSeries.Data.Add(targetValue.Value);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        foreach (var periode in dateTimePeriodes)
                        {
                            var target = kpiActuals.Where(x => x.Periode == periode).FirstOrDefault();
                            if (target == null || !target.Value.HasValue)
                            {
                                aSeries.Data.Add(null);
                            }
                            else
                            {
                                aSeries.Data.Add(target.Value.Value);
                            }
                        }
                        //}


                        var previousSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = "Previous Accumulation",
                            Color = string.IsNullOrEmpty(series.PreviousColor) ? "#004071" : series.PreviousColor,
                            Stack = series.Label,
                            Order = series.Order
                        };
                        for (var i = 0; i < aSeries.Data.Count; i++)
                        {
                            double data = 0;
                            for (var j = 0; j < i; j++)
                            {
                                data += aSeries.Data[j].HasValue ? aSeries.Data[j].Value : 0;
                            }
                            previousSeries.Data.Add(data);
                        }
                        seriesResponse.Add(previousSeries);
                        seriesResponse.Add(aSeries);
                    }
                    else if (seriesType == "multi-stacks" && graphicType == "baraccumulative")
                    {
                        var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = series.Label,
                            Color = series.Color,
                            Order = series.Order
                        };
                        if (comparison)
                        {
                            aSeries.Stack = "KpiActual";
                        }
                        //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                        //{

                        //    foreach (var periode in dateTimePeriodes)
                        //    {
                        //        var targetValue = kpiActuals.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                        //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                        //        if (targetValue == null || !targetValue.HasValue)
                        //        {
                        //            aSeries.Data.Add(null);
                        //        }
                        //        else
                        //        {
                        //            aSeries.Data.Add(targetValue.Value);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        foreach (var periode in dateTimePeriodes)
                        {
                            var target = kpiActuals.Where(x => x.Periode == periode).FirstOrDefault();
                            if (target == null || !target.Value.HasValue)
                            {
                                aSeries.Data.Add(null);
                            }
                            else
                            {
                                aSeries.Data.Add(target.Value.Value);
                            }
                        }
                        //}

                        var previousSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = "Previous Accumulation",
                            Color = string.IsNullOrEmpty(series.PreviousColor) ? "#004071" : series.PreviousColor,
                            Order = series.Order
                        };
                        if (comparison)
                        {
                            previousSeries.Stack = "KpiActual";
                        }
                        for (var i = 0; i < aSeries.Data.Count; i++)
                        {
                            double data = 0;
                            for (var j = 0; j < i; j++)
                            {
                                data += aSeries.Data[j].HasValue ? aSeries.Data[j].Value : 0;
                            }
                            previousSeries.Data.Add(data);
                        }
                        seriesResponse.Add(previousSeries);
                        seriesResponse.Add(aSeries);
                    }
                    else if ((seriesType == "multi-stacks" || seriesType == "multi-stacks-grouped") && graphicType == "barachievement")
                    {
                        var kpiTargets = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType &&
                            x.Periode >= start && x.Periode <= end && x.Kpi.Id == series.KpiId)
                            .OrderBy(x => x.Periode).ToList();
                        if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                      (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                      (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                      (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                        {
                            if (kpiActuals.Count > 0)
                            {
                                var periode = kpiActuals.First().Periode;
                                kpiTargets = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType &&
                              x.Periode == periode && x.Kpi.Id == series.KpiId)
                              .OrderBy(x => x.Periode).ToList();
                            }

                        }
                        var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = series.Label,
                            Color = string.IsNullOrEmpty(series.Color) ? "blue" : series.Color,
                            Order = series.Order
                        };
                        if (comparison)
                        {
                            aSeries.Stack = "KpiActual";
                        }
                        var remainSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = "Remain",
                            Color = "red",
                            Order = series.Order
                        };
                        if (comparison)
                        {
                            remainSeries.Stack = "KpiActual";
                        }
                        var exceedSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = "Exceed",
                            Color = "green",
                            Order = series.Order
                        };
                        if (comparison)
                        {
                            exceedSeries.Stack = "KpiActual";
                        }
                        if (seriesType == "multi-stacks-grouped")
                        {
                            aSeries.Stack = series.Label;
                            remainSeries.Stack = series.Label;
                            exceedSeries.Stack = series.Label;
                        }
                        foreach (var periode in dateTimePeriodes)
                        {
                            //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                            //{
                            //    var actual = kpiActuals.Where(x => x.Periode <= periode)
                            //        .GroupBy(x => x.Kpi)
                            //        .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                            //    var target = kpiTargets.Where(x => x.Periode <= periode)
                            //        .GroupBy(x => x.Kpi)
                            //        .Select(x => x.Sum(y => y.Value)).FirstOrDefault();

                            //    if (!actual.HasValue)
                            //    {
                            //        if (!target.HasValue)
                            //        {
                            //            exceedSeries.Data.Add(0);
                            //            remainSeries.Data.Add(0);
                            //            aSeries.Data.Add(0);
                            //        }
                            //        else
                            //        {
                            //            aSeries.Data.Add(0);
                            //            remainSeries.Data.Add(target.Value);
                            //            exceedSeries.Data.Add(0);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (!target.HasValue)
                            //        {
                            //            aSeries.Data.Add(target.Value);
                            //            remainSeries.Data.Add(0);
                            //            exceedSeries.Data.Add(actual.Value);
                            //        }
                            //        else
                            //        {

                            //            var remain = target.Value - actual.Value;
                            //            if (remain > 0)
                            //            {
                            //                aSeries.Data.Add(actual.Value);
                            //                remainSeries.Data.Add(remain);
                            //                exceedSeries.Data.Add(0);
                            //            }
                            //            else
                            //            {
                            //                aSeries.Data.Add(target.Value);
                            //                exceedSeries.Data.Add(-remain);
                            //                remainSeries.Data.Add(0);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            var actual = kpiActuals.Where(x => x.Periode == periode).FirstOrDefault();
                            var target = kpiTargets.Where(x => x.Periode == periode).FirstOrDefault();
                            if (actual == null || !actual.Value.HasValue)
                            {
                                if (target == null || !target.Value.HasValue)
                                {
                                    exceedSeries.Data.Add(0);
                                    remainSeries.Data.Add(0);
                                    aSeries.Data.Add(0);
                                }
                                else
                                {
                                    aSeries.Data.Add(0);
                                    remainSeries.Data.Add(target.Value.Value);
                                    exceedSeries.Data.Add(0);
                                }
                            }
                            else
                            {
                                if (target == null || !target.Value.HasValue)
                                {
                                    aSeries.Data.Add(0);
                                    remainSeries.Data.Add(0);
                                    exceedSeries.Data.Add(actual.Value.Value);
                                }
                                else
                                {

                                    var remain = target.Value.Value - actual.Value.Value;
                                    if (remain > 0)
                                    {
                                        aSeries.Data.Add(actual.Value.Value);
                                        remainSeries.Data.Add(remain);
                                        exceedSeries.Data.Add(0);
                                    }
                                    else
                                    {
                                        aSeries.Data.Add(target.Value.Value);
                                        exceedSeries.Data.Add(-remain);
                                        remainSeries.Data.Add(0);
                                    }
                                }
                            }
                            //}


                        }
                        seriesResponse.Add(remainSeries);
                        seriesResponse.Add(exceedSeries);
                        seriesResponse.Add(aSeries);
                    }
                    else if (seriesType == "multi-stacks-grouped")
                    {
                        var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = series.Label,
                            Stack = series.Label,
                            Color = series.Color,
                            Order = series.Order,
                            MarkerColor = series.MarkerColor,
                            LineType = series.LineType
                        };
                        if (asNetbackChart)
                        {
                            var sumValue = kpiActuals.Where(x => x.Value.HasValue).Average(x => x.Value);
                            aSeries.BorderColor = "transparent";
                            aSeries.Data.Add(sumValue);
                            if (newTimeInformation == null && newDatetimePeriodes.Count == 0)
                            {
                                if (rangeFilter == RangeFilter.AllExistingYears)
                                {
                                    newTimeInformation = "2011 - 2030";
                                    newDatetimePeriodes = new List<DateTime> { new DateTime(2011, 1, 1, 0, 0, 0), new DateTime(2030, 1, 1, 0, 0, 0) };
                                }
                                else
                                {
                                    switch (periodeType)
                                    {
                                        case PeriodeType.Hourly:
                                            newTimeInformation = start.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture) + " - " + end.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                            break;
                                        case PeriodeType.Daily:
                                            newTimeInformation = start.ToString("dd MMM yy", CultureInfo.InvariantCulture) + " - " + end.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                            break;
                                        case PeriodeType.Monthly:
                                            newTimeInformation = start.ToString("MMM yy", CultureInfo.InvariantCulture) + " - " + end.ToString("MMM yy", CultureInfo.InvariantCulture);
                                            break;
                                        case PeriodeType.Yearly:
                                            newTimeInformation = start.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture) + " - " + end.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                                            break;
                                    }
                                    dateTimePeriodes = new List<DateTime> { start, end };
                                    newDatetimePeriodes = dateTimePeriodes;
                                }
                            }

                        }
                        //else if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                        //{

                        //    foreach (var periode in dateTimePeriodes)
                        //    {
                        //        var economicValue = kpiActuals.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                        //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                        //        if (economicValue == null || !economicValue.HasValue)
                        //        {
                        //            aSeries.Data.Add(null);
                        //        }
                        //        else
                        //        {
                        //            aSeries.Data.Add(economicValue.Value);
                        //        }
                        //    }

                        //}
                        else
                        {
                            foreach (var periode in dateTimePeriodes)
                            {
                                var target = kpiActuals.Where(x => x.Periode == periode).FirstOrDefault();
                                if (target == null || !target.Value.HasValue)
                                {
                                    aSeries.Data.Add(null);
                                }
                                else
                                {
                                    aSeries.Data.Add(target.Value.Value);
                                }
                            }
                        }


                        if (graphicType == "baraccumulative")
                        {
                            var previousSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = "Previous Accumulation",
                                Color = string.IsNullOrEmpty(series.PreviousColor) ? "#004071" : series.PreviousColor,
                                Stack = series.Label,
                                Order = series.Order
                            };
                            for (var i = 0; i < aSeries.Data.Count; i++)
                            {
                                double data = 0;
                                for (var j = 0; j < i; j++)
                                {
                                    data += aSeries.Data[j].HasValue ? aSeries.Data[j].Value : 0;
                                }
                                previousSeries.Data.Add(data);
                            }
                            seriesResponse.Add(previousSeries);
                        }
                        seriesResponse.Add(aSeries);
                        if (asNetbackChart && seriesResponse.Count > 1)
                        {
                            var invicibleSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = "invisible_" + series.Label,
                                Stack = series.Label,
                                Color = "transparent",
                                BorderColor = "transparent",
                                ShowInLegend = false
                            };
                            invicibleSeries.Data.Add(seriesResponse[seriesResponse.Count - 2].Data[0] - seriesResponse[seriesResponse.Count - 1].Data[0]);
                            seriesResponse.Add(invicibleSeries);
                        }
                    }
                    else // multistack groups?
                    {
                        //var kpiTargets = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                        //     x.Periode >= start && x.Periode <= end && x.Kpi.Id == series.KpiId)
                        //     .OrderBy(x => x.Periode).ToList();
                        var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                        {
                            Name = series.Label,
                            Color = series.Color,
                            Order = series.Order,
                            MarkerColor = series.MarkerColor,
                            LineType = series.LineType
                        };
                        if (comparison)
                        {
                            aSeries.Stack = "KpiActual";
                        }
                        //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                        //{

                        //    foreach (var periode in dateTimePeriodes)
                        //    {
                        //        var targetValue = kpiActuals.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                        //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                        //        if (targetValue == null || !targetValue.HasValue)
                        //        {
                        //            aSeries.Data.Add(null);
                        //        }
                        //        else
                        //        {
                        //            aSeries.Data.Add(targetValue.Value);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        foreach (var periode in dateTimePeriodes)
                        {
                            var target = kpiActuals.Where(x => x.Periode == periode).FirstOrDefault();
                            if (target == null || !target.Value.HasValue)
                            {
                                aSeries.Data.Add(null);
                            }
                            else
                            {
                                aSeries.Data.Add(target.Value.Value);
                            }
                        }
                        //}

                        if (graphicType == "baraccumulative")
                        {
                            var previousSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = "Previous Accumulation",
                                Color = string.IsNullOrEmpty(series.PreviousColor) ? "#004071" : series.PreviousColor,
                                Order = series.Order
                            };
                            if (comparison)
                            {
                                previousSeries.Stack = "KpiActual";
                            }
                            for (var i = 0; i < aSeries.Data.Count; i++)
                            {
                                double data = 0;
                                for (var j = 0; j < i; j++)
                                {
                                    data += aSeries.Data[j].HasValue ? aSeries.Data[j].Value : 0;
                                }
                                previousSeries.Data.Add(data);
                            }
                            seriesResponse.Add(previousSeries);
                        }
                        seriesResponse.Add(aSeries);
                    }
                }
                else
                {
                    foreach (var stack in series.Stacks)
                    {
                        var kpiActuals = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                        x.Periode >= start && x.Periode <= end && x.Kpi.Id == stack.KpiId)
                        .OrderBy(x => x.Periode).ToList();

                        if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                     (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                     (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                     (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                        {
                            var kpiActual = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                          x.Periode <= end && x.Kpi.Id == stack.KpiId && (x.Value != null && x.Value.Value != 0))
                          .OrderByDescending(x => x.Periode).FirstOrDefault();
                            if (kpiActual != null)
                            {
                                kpiActuals = new List<KpiAchievement> { kpiActual };
                                switch (periodeType)
                                {
                                    case PeriodeType.Hourly:
                                        newTimeInformation = kpiActual.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                                        break;
                                    case PeriodeType.Daily:
                                        newTimeInformation = kpiActual.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                                        break;
                                    case PeriodeType.Monthly:
                                        newTimeInformation = kpiActual.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                                        break;
                                    case PeriodeType.Yearly:
                                        newTimeInformation = kpiActual.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                                        break;
                                }
                                dateTimePeriodes = new List<DateTime> { kpiActual.Periode };
                                newDatetimePeriodes = dateTimePeriodes;
                            }
                        }

                        if (seriesType == "multi-stacks-grouped")
                        {
                            var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = stack.Label,
                                Stack = series.Label,
                                Color = stack.Color,
                                Order = series.Order,
                                LineType = series.LineType,
                                MarkerColor = series.MarkerColor
                            };
                            if (comparison)
                            {
                                aSeries.Stack = "KpiActual";
                            }
                            //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                            //{

                            //    foreach (var periode in dateTimePeriodes)
                            //    {
                            //        var targetValue = kpiActuals.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                            //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                            //        if (targetValue == null || !targetValue.HasValue)
                            //        {
                            //            aSeries.Data.Add(null);
                            //        }
                            //        else
                            //        {
                            //            aSeries.Data.Add(targetValue.Value);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            foreach (var periode in dateTimePeriodes)
                            {
                                var target = kpiActuals.Where(x => x.Periode == periode).FirstOrDefault();
                                if (target == null || !target.Value.HasValue)
                                {
                                    aSeries.Data.Add(null);
                                }
                                else
                                {
                                    aSeries.Data.Add(target.Value.Value);
                                }
                            }
                            //}
                            seriesResponse.Add(aSeries);
                        }
                        else
                        {

                            var aSeries = new GetCartesianChartDataResponse.SeriesResponse
                            {
                                Name = stack.Label,
                                Color = stack.Color,
                                Order = series.Order,
                                LineType = series.LineType,
                                MarkerColor = series.MarkerColor
                            };
                            if (comparison)
                            {
                                aSeries.Stack = "KpiActual";
                            }
                            //if (rangeFilter == RangeFilter.YTD || rangeFilter == RangeFilter.DTD || rangeFilter == RangeFilter.MTD)
                            //{

                            //    foreach (var periode in dateTimePeriodes)
                            //    {
                            //        var targetValue = kpiActuals.Where(x => x.Periode <= periode).GroupBy(x => x.Kpi)
                            //            .Select(x => x.Sum(y => y.Value)).FirstOrDefault();
                            //        if (targetValue == null || !targetValue.HasValue)
                            //        {
                            //            aSeries.Data.Add(null);
                            //        }
                            //        else
                            //        {
                            //            aSeries.Data.Add(targetValue.Value);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            foreach (var periode in dateTimePeriodes)
                            {
                                var target = kpiActuals.Where(x => x.Periode == periode).FirstOrDefault();
                                if (target == null || !target.Value.HasValue)
                                {
                                    aSeries.Data.Add(null);
                                }
                                else
                                {
                                    aSeries.Data.Add(target.Value.Value);
                                }
                            }
                            //}
                            seriesResponse.Add(aSeries);
                        }
                    }
                }
            }
            return seriesResponse;
        }

        public CreateArtifactResponse Create(CreateArtifactRequest request)
        {
            var artifact = request.MapTo<Artifact>();
            var measurement = new Measurement { Id = request.MeasurementId };
            DataContext.Measurements.Attach(measurement);
            artifact.Measurement = measurement;
            var action = request.MapTo<BaseAction>();
            foreach (var seriesReq in request.Series)
            {
                var series = seriesReq.MapTo<ArtifactSerie>();
                if (seriesReq.KpiId != 0)
                {
                    var kpi = new Kpi { Id = seriesReq.KpiId };
                    if (DataContext.Kpis.Local.Where(x => x.Id == seriesReq.KpiId).FirstOrDefault() == null)
                    {
                        DataContext.Kpis.Attach(kpi);
                    }
                    else
                    {
                        kpi = DataContext.Kpis.Local.Where(x => x.Id == seriesReq.KpiId).FirstOrDefault();
                    }
                    series.Kpi = kpi;
                }
                foreach (var stackReq in seriesReq.Stacks)
                {
                    var stack = stackReq.MapTo<ArtifactStack>();
                    if (stackReq.KpiId != 0)
                    {
                        var kpiInStack = new Kpi { Id = stackReq.KpiId };
                        if (DataContext.Kpis.Local.Where(x => x.Id == stackReq.KpiId).FirstOrDefault() == null)
                        {
                            DataContext.Kpis.Attach(kpiInStack);
                        }
                        else
                        {
                            kpiInStack = DataContext.Kpis.Local.Where(x => x.Id == stackReq.KpiId).FirstOrDefault();
                        }
                        stack.Kpi = kpiInStack;
                    }
                    series.Stacks.Add(stack);
                }
                artifact.Series.Add(series);
            }
            foreach (var chartReq in request.Charts)
            {
                var chart = chartReq.MapTo<ArtifactChart>();
                var localMeasurement = new Measurement { Id = chartReq.MeasurementId };
                if (DataContext.Measurements.Local.Where(x => x.Id == localMeasurement.Id).FirstOrDefault() == null)
                {
                    DataContext.Measurements.Attach(localMeasurement);
                }
                else
                {
                    localMeasurement = DataContext.Measurements.Local.Where(x => x.Id == localMeasurement.Id).FirstOrDefault();
                }
                if (localMeasurement.Id != 0)
                {
                    chart.Measurement = localMeasurement;
                }
                foreach (var seriesReq in chartReq.Series)
                {
                    var series = seriesReq.MapTo<ArtifactSerie>();
                    if (seriesReq.KpiId != 0)
                    {
                        var kpi = new Kpi { Id = seriesReq.KpiId };
                        if (DataContext.Kpis.Local.Where(x => x.Id == seriesReq.KpiId).FirstOrDefault() == null)
                        {
                            DataContext.Kpis.Attach(kpi);
                        }
                        else
                        {
                            kpi = DataContext.Kpis.Local.Where(x => x.Id == seriesReq.KpiId).FirstOrDefault();
                        }
                        series.Kpi = kpi;
                    }
                    foreach (var stackReq in seriesReq.Stacks)
                    {
                        var stack = stackReq.MapTo<ArtifactStack>();
                        if (stackReq.KpiId != 0)
                        {
                            var kpiInStack = new Kpi { Id = stackReq.KpiId };
                            if (DataContext.Kpis.Local.Where(x => x.Id == stackReq.KpiId).FirstOrDefault() == null)
                            {
                                DataContext.Kpis.Attach(kpiInStack);
                            }
                            else
                            {
                                kpiInStack = DataContext.Kpis.Local.Where(x => x.Id == stackReq.KpiId).FirstOrDefault();
                            }
                            stack.Kpi = kpiInStack;
                        }
                        series.Stacks.Add(stack);
                    }
                    chart.Series.Add(series);
                }
                artifact.Charts.Add(chart);
            }
            foreach (var plotReq in request.Plots)
            {
                var plot = plotReq.MapTo<ArtifactPlot>();
                artifact.Plots.Add(plot);
            }
            foreach (var rowReq in request.Rows)
            {
                var row = rowReq.MapTo<ArtifactRow>();
                if (rowReq.KpiId != 0)
                {
                    var kpiInRow = new Kpi { Id = rowReq.KpiId };
                    if (DataContext.Kpis.Local.Where(x => x.Id == rowReq.KpiId).FirstOrDefault() == null)
                    {
                        DataContext.Kpis.Attach(kpiInRow);
                    }
                    else
                    {
                        kpiInRow = DataContext.Kpis.Local.Where(x => x.Id == rowReq.KpiId).FirstOrDefault();
                    }
                    row.Kpi = kpiInRow;
                }
                artifact.Rows.Add(row);
            }
            if (request.Tank != null)
            {
                var tank = new ArtifactTank();
                var volumeInventory = new Kpi { Id = request.Tank.VolumeInventoryId };
                if (DataContext.Kpis.Local.Where(x => x.Id == volumeInventory.Id).FirstOrDefault() == null)
                {
                    DataContext.Kpis.Attach(volumeInventory);
                }
                else
                {
                    volumeInventory = DataContext.Kpis.Local.Where(x => x.Id == request.Tank.VolumeInventoryId).FirstOrDefault();
                }
                tank.VolumeInventory = volumeInventory;
                var daysToTankTop = new Kpi { Id = request.Tank.DaysToTankTopId };
                if (DataContext.Kpis.Local.Where(x => x.Id == daysToTankTop.Id).FirstOrDefault() == null)
                {
                    DataContext.Kpis.Attach(daysToTankTop);
                }
                else
                {
                    daysToTankTop = DataContext.Kpis.Local.Where(x => x.Id == request.Tank.DaysToTankTopId).FirstOrDefault();
                }
                tank.DaysToTankTop = daysToTankTop;
                tank.DaysToTankTopTitle = request.Tank.DaysToTankTopTitle;
                tank.MinCapacity = request.Tank.MinCapacity;
                tank.MaxCapacity = request.Tank.MaxCapacity;
                tank.Color = request.Tank.Color;
                tank.ShowLine = request.Tank.ShowLine;
                artifact.Tank = tank;
            }
            DataContext.Artifacts.Add(artifact);
            //DataContext.SaveChanges();
            DataContext.SaveChanges(action);
            return new CreateArtifactResponse();
        }

        public UpdateArtifactResponse Update(UpdateArtifactRequest request)
        {
            var action = request.MapTo<BaseAction>();
            var artifact = DataContext.Artifacts.Include(x => x.Measurement)
                .Include(x => x.Series)
                .Include(x => x.Series.Select(y => y.Kpi))
                .Include(x => x.Series.Select(y => y.Stacks))
                .Include(x => x.Series.Select(y => y.Stacks.Select(z => z.Kpi)))
                .Include(x => x.Plots)
                .Include(x => x.Rows)
                .Include(x => x.Charts)
                .Include(x => x.Charts.Select(y => y.Series))
                .Include(x => x.Charts.Select(y => y.Series.Select(z => z.Stacks)))
                .Single(x => x.Id == request.Id);

            if (artifact.Measurement.Id != request.MeasurementId)
            {
                var measurement = new Measurement { Id = request.MeasurementId };
                DataContext.Measurements.Attach(measurement);
                artifact.Measurement = measurement;
            }

            foreach (var series in artifact.Series.ToList())
            {
                foreach (var stack in series.Stacks.ToList())
                {
                    DataContext.ArtifactStacks.Remove(stack);
                }
                DataContext.ArtifactSeries.Remove(series);
            }

            foreach (var plot in artifact.Plots.ToList())
            {
                DataContext.ArtifactPlots.Remove(plot);
            }

            foreach (var seriesReq in request.Series)
            {
                var series = seriesReq.MapTo<ArtifactSerie>();
                if (seriesReq.KpiId != 0)
                {
                    var kpi = new Kpi { Id = seriesReq.KpiId };
                    if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == seriesReq.KpiId) == null)
                    {
                        DataContext.Kpis.Attach(kpi);
                    }
                    else
                    {
                        kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == seriesReq.KpiId);
                    }
                    series.Kpi = kpi;
                }
                foreach (var stackReq in seriesReq.Stacks)
                {
                    var stack = stackReq.MapTo<ArtifactStack>();
                    if (stackReq.KpiId != 0)
                    {
                        var kpiInStack = new Kpi { Id = stackReq.KpiId };
                        if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == stackReq.KpiId) == null)
                        {
                            DataContext.Kpis.Attach(kpiInStack);
                        }
                        else
                        {
                            kpiInStack = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == stackReq.KpiId);
                        }
                        stack.Kpi = kpiInStack;
                    }
                    series.Stacks.Add(stack);
                }
                artifact.Series.Add(series);
            }

            foreach (var plotReq in request.Plots)
            {
                var plot = plotReq.MapTo<ArtifactPlot>();
                artifact.Plots.Add(plot);
            }

            foreach (var row in artifact.Rows.ToList())
            {
                DataContext.ArtifactRows.Remove(row);
            }

            foreach (var rowReq in request.Rows)
            {
                var row = rowReq.MapTo<ArtifactRow>();
                if (rowReq.KpiId != 0)
                {
                    var kpiInRow = new Kpi { Id = rowReq.KpiId };
                    if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == rowReq.KpiId) == null)
                    {
                        DataContext.Kpis.Attach(kpiInRow);
                    }
                    else
                    {
                        kpiInRow = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == rowReq.KpiId);
                    }
                    row.Kpi = kpiInRow;
                }
                artifact.Rows.Add(row);
            }

            foreach (var chart in artifact.Charts.ToList())
            {

                foreach (var series in chart.Series.ToList())
                {
                    foreach (var stack in series.Stacks.ToList())
                    {
                        DataContext.ArtifactStacks.Remove(stack);
                    }
                    DataContext.ArtifactSeries.Remove(series);
                }
                DataContext.ArtifactCharts.Remove(chart);
            }

            foreach (var chartReq in request.Charts)
            {
                var chart = chartReq.MapTo<ArtifactChart>();
                var localMeasurement = new Measurement { Id = chartReq.MeasurementId };
                if (DataContext.Measurements.Local.Where(x => x.Id == localMeasurement.Id).FirstOrDefault() == null)
                {
                    DataContext.Measurements.Attach(localMeasurement);
                }
                else
                {
                    localMeasurement = DataContext.Measurements.Local.Where(x => x.Id == localMeasurement.Id).FirstOrDefault();
                }
                if (localMeasurement.Id != 0)
                {
                    chart.Measurement = localMeasurement;
                }
                foreach (var seriesReq in chartReq.Series)
                {
                    var series = seriesReq.MapTo<ArtifactSerie>();
                    if (seriesReq.KpiId != 0)
                    {
                        var kpi = new Kpi { Id = seriesReq.KpiId };
                        if (DataContext.Kpis.Local.Where(x => x.Id == seriesReq.KpiId).FirstOrDefault() == null)
                        {
                            DataContext.Kpis.Attach(kpi);
                        }
                        else
                        {
                            kpi = DataContext.Kpis.Local.Where(x => x.Id == seriesReq.KpiId).FirstOrDefault();
                        }
                        series.Kpi = kpi;
                    }
                    foreach (var stackReq in seriesReq.Stacks)
                    {
                        var stack = stackReq.MapTo<ArtifactStack>();
                        if (stackReq.KpiId != 0)
                        {
                            var kpiInStack = new Kpi { Id = stackReq.KpiId };
                            if (DataContext.Kpis.Local.Where(x => x.Id == stackReq.KpiId).FirstOrDefault() == null)
                            {
                                DataContext.Kpis.Attach(kpiInStack);
                            }
                            else
                            {
                                kpiInStack = DataContext.Kpis.Local.Where(x => x.Id == stackReq.KpiId).FirstOrDefault();
                            }
                            stack.Kpi = kpiInStack;
                        }
                        series.Stacks.Add(stack);
                    }
                    chart.Series.Add(series);
                }
                artifact.Charts.Add(chart);
            }


            if (request.Tank != null)
            {
                var tank = DataContext.ArtifactTanks.Single(x => x.Id == request.Tank.Id);
                var volumeInventory = new Kpi { Id = request.Tank.VolumeInventoryId };
                if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == volumeInventory.Id) == null)
                {
                    DataContext.Kpis.Attach(volumeInventory);
                }
                else
                {
                    volumeInventory = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == request.Tank.VolumeInventoryId);
                }
                tank.VolumeInventory = volumeInventory;
                var daysToTankTop = new Kpi { Id = request.Tank.DaysToTankTopId };
                if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == daysToTankTop.Id) == null)
                {
                    DataContext.Kpis.Attach(daysToTankTop);
                }
                else
                {
                    daysToTankTop = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == request.Tank.DaysToTankTopId);
                }
                tank.DaysToTankTop = daysToTankTop;
                tank.DaysToTankTopTitle = request.Tank.DaysToTankTopTitle;
                tank.MinCapacity = request.Tank.MinCapacity;
                tank.MaxCapacity = request.Tank.MaxCapacity;
                tank.Color = request.Tank.Color;
                tank.ShowLine = request.Tank.ShowLine;
            }

            artifact.GraphicName = request.GraphicName;
            artifact.HeaderTitle = request.HeaderTitle;
            artifact.PeriodeType = request.PeriodeType;
            artifact.RangeFilter = request.RangeFilter;
            artifact.Start = request.Start;
            artifact.End = request.End;
            artifact.ValueAxis = request.ValueAxis;
            artifact.Actual = request.Actual;
            artifact.Target = request.Target;
            artifact.Economic = request.Economic;
            artifact.Fullfillment = request.Fullfillment;
            artifact.Remark = request.Remark;
            artifact.ShowLegend = request.ShowLegend;
            artifact.MaxFractionScale = request.MaxFractionScale;
            artifact.Is3D = request.Is3D;

            artifact.FractionScale = request.FractionScale;
            artifact.SeriesType = request.SeriesType;
            DataContext.SaveChanges(action);
            return new UpdateArtifactResponse();
        }

        public GetArtifactsResponse GetArtifacts(GetArtifactsRequest request)
        {
            int totalRecords;
            var query = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                query = query.Skip(request.Skip).Take(request.Take);
            }
            var artifacts = query.ToList();

            var response = new GetArtifactsResponse();
            response.Artifacts = artifacts.MapTo<GetArtifactsResponse.Artifact>();
            response.TotalRecords = totalRecords;

            return response;
            /*if (request.OnlyCount)
            {
                return new GetArtifactsResponse { Count = DataContext.Artifacts.Count() };
            }
            else
            {
                return new GetArtifactsResponse
                {
                    Artifacts = DataContext.Artifacts.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take)
                                    .ToList().MapTo<GetArtifactsResponse.Artifact>()
                };
            }*/
        }

        private IEnumerable<Artifact> SortData(string search, IDictionary<string, System.Data.SqlClient.SortOrder> sortingDictionary, out int totalRecords)
        {
            var data = DataContext.Artifacts.Include(x => x.LayoutColumns).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.GraphicName.Contains(search) || x.GraphicType.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.GraphicName)
                                   : data.OrderByDescending(x => x.GraphicName);
                        break;
                    case "Order":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.GraphicType)
                                   : data.OrderByDescending(x => x.GraphicType);
                        break;
                    case "Used":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.LayoutColumns.Count > 0 ? 1 : 0).ThenBy(x => x.GraphicName)
                            : data.OrderByDescending(x => x.LayoutColumns.Count > 0 ? 1 : 0).ThenBy(x => x.GraphicName);
                        break;
                }
            }
            totalRecords = data.Count();
            return data;
        }

        public GetArtifactResponse GetArtifact(GetArtifactRequest request)
        {
            return DataContext.Artifacts.Include(x => x.Measurement)
                .Include(x => x.Series)
                .Include(x => x.Series.Select(y => y.Kpi))
                .Include(x => x.Series.Select(y => y.Kpi.Measurement))
                .Include(x => x.Series.Select(y => y.Stacks))
                .Include(x => x.Series.Select(y => y.Stacks.Select(z => z.Kpi)))
                .Include(x => x.Series.Select(y => y.Stacks.Select(z => z.Kpi.Measurement)))
                .Include(x => x.Plots)
                .Include(x => x.Rows)
                .Include(x => x.Rows.Select(y => y.Kpi))
                .Include(x => x.Rows.Select(y => y.Kpi.Measurement))
                .Include(x => x.Charts)
                .Include(x => x.Charts.Select(y => y.Measurement))
                .Include(x => x.Charts.Select(y => y.Series))
                .Include(x => x.Charts.Select(y => y.Series.Select(z => z.Kpi)))
                .Include(x => x.Charts.Select(y => y.Series.Select(z => z.Kpi.Measurement)))
                .Include(x => x.Charts.Select(y => y.Series.Select(z => z.Stacks)))
                .Include(x => x.Charts.Select(y => y.Series.Select(z => z.Stacks.Select(a => a.Kpi))))
                 .Include(x => x.Charts.Select(y => y.Series.Select(z => z.Stacks.Select(a => a.Kpi.Measurement))))
                .Include(x => x.Tank)
                .Include(x => x.Tank.DaysToTankTop)
                .Include(x => x.Tank.DaysToTankTop.Measurement)
                .Include(x => x.Tank.VolumeInventory)
                .Include(x => x.Tank.VolumeInventory.Measurement)
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetArtifactResponse>();
        }

        public GetArtifactsToSelectResponse GetArtifactsToSelect(GetArtifactsToSelectRequest request)
        {
            return new GetArtifactsToSelectResponse
            {
                Artifacts = DataContext.Artifacts.Where(x => x.GraphicName.Contains(request.Term)).Take(20).ToList()
                .MapTo<GetArtifactsToSelectResponse.ArtifactResponse>()
            };
        }

        public DeleteArtifactResponse Delete(DeleteArtifactRequest request)
        {
            try
            {
                var action = request.MapTo<BaseAction>();
                var artifact = DataContext.Artifacts.Include(x => x.Series).Include(x => x.Plots).Include(x => x.LayoutColumns).FirstOrDefault(x => x.Id == request.Id);
                if (artifact.LayoutColumns.Count > 0)
                {
                    return new DeleteArtifactResponse
                    {
                        IsSuccess = false,
                        Message = "The item couldn't be deleted. It is being used by your aplication"
                    };
                }
                foreach (var serie in artifact.Series.ToList())
                {
                    DataContext.ArtifactSeries.Remove(serie);
                }
                foreach (var plot in artifact.Plots.ToList())
                {
                    DataContext.ArtifactPlots.Remove(plot);
                }
                DataContext.Artifacts.Remove(artifact);
                DataContext.SaveChanges(action);
                return new DeleteArtifactResponse
                {
                    IsSuccess = true,
                    Message = "The item has been deleted successfully"
                };
            }
            catch
            {
                return new DeleteArtifactResponse
                {
                    IsSuccess = false,
                    Message = "An error has been occured please contact the administrator for further information"
                };
            }
        }

        private DateTimeValue ChangeFromSpecificToInterval(/*GetPieDataRequest request,*/ DateTime? start, DateTime? end, RangeFilter rangeFilter)
        {
            if (start.HasValue && end.HasValue && rangeFilter.Equals(RangeFilter.SpecificYear))
            {
                rangeFilter = RangeFilter.Interval;
                start = new DateTime(start.Value.Year, 1, 1);
                end = new DateTime(end.Value.Year, 12, 31);
            }
            else if (start.HasValue && end.HasValue &&
                     rangeFilter.Equals(RangeFilter.SpecificMonth))
            {
                rangeFilter = RangeFilter.Interval;
                start = new DateTime(start.Value.Year, start.Value.Month, 1);
                end = new DateTime(end.Value.Year, end.Value.Month,
                                           DateTime.DaysInMonth(end.Value.Year, end.Value.Month));
            }
            else if (start.HasValue && end.HasValue &&
                     rangeFilter.Equals(RangeFilter.SpecificDay))
            {
                rangeFilter = RangeFilter.Interval;
                start = new DateTime(start.Value.Year, start.Value.Month, start.Value.Day);
                end = new DateTime(start.Value.Year, start.Value.Month, start.Value.Day);
            }

            return new DateTimeValue { Start = start, End = end, RangeFilter = rangeFilter };
        }

        private string ChangeTimeInformationFromSpecificToInterval(DateTime? start, DateTime? end, RangeFilter rangeFilter)
        {
            string timeInformation = string.Empty;
            if (start.HasValue && end.HasValue && rangeFilter.Equals(RangeFilter.SpecificYear))
            {
                timeInformation = start.Value.ToString("yyyy", CultureInfo.InvariantCulture);
            }
            else if (start.HasValue && end.HasValue && rangeFilter.Equals(RangeFilter.SpecificMonth))
            {
                timeInformation = start.Value.ToString("MMM yy", CultureInfo.InvariantCulture);
            }
            else if (start.HasValue && end.HasValue && rangeFilter.Equals(RangeFilter.SpecificDay))
            {
                timeInformation = start.Value.ToString("dd MMM yy", CultureInfo.InvariantCulture);
            }

            return timeInformation;
        }

        private DateTime StartOfWeek()
        {
            DateTime dt = DateTime.Now;
            int diff = dt.DayOfWeek - DayOfWeek.Sunday;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public IList<ExportSettingData> GetExportExcelData(Dictionary<string, List<string>> labelDictionaries, DateTime? start, DateTime? end, string periodeType)
        {
            var kpiActuals = labelDictionaries.Where(x => x.Value.ElementAtOrDefault(1) == "KpiActual").Select(x => Int32.Parse(x.Value.ElementAt(0))).ToArray();
            var kpiTargets = labelDictionaries.Where(x => x.Value.ElementAtOrDefault(1) == "KpiTarget").Select(x => Int32.Parse(x.Value.ElementAt(0))).ToArray();
            var kpiEconomics = labelDictionaries.Where(x => x.Value.ElementAtOrDefault(1) == "KpiEconomic").Select(x => Int32.Parse(x.Value.ElementAt(0))).ToArray();
            var dataActuals = _kpiAchievementService.GetKpiAchievements(kpiActuals, start, end, periodeType);
            var dataTargets = _kpiTargetService.GetKpiTargets(kpiTargets, start, end, periodeType);
            var dataEconomics = _kpiAchievementService.GetKpiEconomics(kpiEconomics, start, end, periodeType);

            //var existedTargetKpis = dataTargets.KpiTargets.GroupBy(x => x.KpiId).Select(g => g.First()).ToList();

            Dictionary<DateTime, IList<ExportSettingData>> dictionaries =
                new Dictionary<DateTime, IList<ExportSettingData>>();
            IList<ExportSettingData> exportData = new List<ExportSettingData>();

            foreach (var x in dataActuals.KpiAchievements)
            {
                exportData.Add(new ExportSettingData { Periode = x.Periode, KpiId = x.Kpi.Id, KpiName = x.Kpi.Name, MeasurementName = x.Kpi.KpiMeasurement, Value = x.Value, ValueAxes = ValueAxis.KpiActual.ToString() });
            }

            foreach (var x in dataTargets.KpiTargets)
            {
                exportData.Add(new ExportSettingData { Periode = x.Periode, KpiId = x.KpiId, KpiName = x.KpiName, MeasurementName = x.MeasurementName, Value = x.Value, ValueAxes = ValueAxis.KpiTarget.ToString() });
            }

            foreach (var x in dataEconomics.KpiAchievements)
            {
                exportData.Add(new ExportSettingData { Periode = x.Periode, KpiId = x.Kpi.Id, KpiName = x.Kpi.Name, MeasurementName = x.Kpi.KpiMeasurement, Value = x.Value, ValueAxes = ValueAxis.KpiEconomic.ToString() });
            }


            return exportData;
        }

        public IList<ExportSettingData> GetExportExcelPieData(Dictionary<string, List<string>> labelDictionaries, RangeFilter rangeFilter, DateTime? requestStart, DateTime? requestEnd, PeriodeType periodeType, ArtifactValueInformation valueInformation,
            out IList<DateTime> dateTimePeriodes, out string timeInformation)
        {

            var response = new GetPieDataResponse();
            var oldRangeFilter = rangeFilter;
            if (rangeFilter.Equals(RangeFilter.SpecificYear) || rangeFilter.Equals(RangeFilter.SpecificMonth) || rangeFilter.Equals(RangeFilter.SpecificDay))
            {
                var timeValue = ChangeFromSpecificToInterval(requestStart, requestEnd, rangeFilter);
                requestStart = timeValue.Start;
                requestEnd = timeValue.End;
                rangeFilter = timeValue.RangeFilter;
            }

            this.GetPeriodes(periodeType, rangeFilter, requestStart, requestEnd, out dateTimePeriodes,
                              out timeInformation);

            if (oldRangeFilter.Equals(RangeFilter.SpecificYear) || oldRangeFilter.Equals(RangeFilter.SpecificMonth) || oldRangeFilter.Equals(RangeFilter.SpecificDay))
            {
                timeInformation = ChangeTimeInformationFromSpecificToInterval(requestStart, requestEnd, oldRangeFilter);
            }

            IList<ExportSettingData> list = new List<ExportSettingData>();

            foreach (var label in labelDictionaries)
            {
                var kpiId = Int32.Parse(label.Key.Split('|')[0]);
                var valueAxis = (ValueAxis)Enum.Parse(typeof(ValueAxis), label.Key.Split('|')[1], true);
                var kpi = DataContext.Kpis.Include(x => x.Measurement).First(x => x.Id == kpiId);
                var start = dateTimePeriodes[0];
                var end = dateTimePeriodes[dateTimePeriodes.Count - 1];

                var data = new ExportSettingData
                {
                    KpiId = kpiId,
                    KpiName = kpi.Name,
                    MeasurementName = kpi.Measurement.Name,
                    ValueAxes = valueAxis.ToString(),
                    Periode = start
                };

                switch (kpi.YtdFormula)
                {
                    case YtdFormula.Sum:
                        if (valueAxis == ValueAxis.KpiTarget)
                        {
                            data.Value = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        else if (valueAxis == ValueAxis.KpiActual)
                        {
                            data.Value = SumSeries(valueInformation, periodeType, start, end, kpi.Id);
                        }
                        else if (valueAxis == ValueAxis.KpiEconomic)
                        {
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            data.Value = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == periodeType
                               && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id && x.Scenario.Id == scenarioId)
                               .GroupBy(x => x.Kpi.Id)
                               .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        break;

                    case YtdFormula.Average:
                        if (valueAxis == ValueAxis.KpiTarget)
                        {
                            data.Value = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        else if (valueAxis == ValueAxis.KpiActual)
                        {
                            data.Value = AverageSeries(valueInformation, periodeType, start, end, kpi.Id);
                        }
                        else if (valueAxis == ValueAxis.KpiEconomic)
                        {
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            data.Value = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == periodeType
                               && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id && x.Scenario.Id == scenarioId)
                               .GroupBy(x => x.Kpi.Id)
                               .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        break;

                    default: //nan or custom:
                        if (valueAxis == ValueAxis.KpiTarget)
                        {
                            data.Value = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType
                                && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        else if (valueAxis == ValueAxis.KpiActual)
                        {
                            data.Value = SumSeries(valueInformation, periodeType, start, end, kpi.Id);
                        }
                        else if (valueAxis == ValueAxis.KpiEconomic)
                        {
                            var scenarioId = 0;
                            var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                            if (scenario != null)
                            {
                                scenarioId = scenario.Id;
                            }
                            data.Value = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == periodeType
                               && x.Periode >= start && x.Periode <= end && x.Kpi.Id == kpi.Id && x.Scenario.Id == scenarioId)
                               .GroupBy(x => x.Kpi.Id)
                               .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                        }
                        break;
                }

                KpiAchievement latestActual = null;
                if (valueAxis == ValueAxis.KpiActual)
                {
                    if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                        (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                        (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                        (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                    {
                        var kpiActual = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                                                                               x.Periode <= end &&
                                                                               x.Kpi.Id == kpi.Id && (x.Value != null))
                                                   .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiActual != null && kpiActual.Value.HasValue)
                        {
                            latestActual = kpiActual;
                            data.Value = kpiActual.Value.Value;
                            data.Periode = kpiActual.Periode;
                        }
                    }
                }


                if (valueAxis == ValueAxis.KpiTarget && latestActual != null)
                {
                    if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                       (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                       (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                       (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                    {
                        var kpiTarget = DataContext.KpiTargets.Where(x => x.PeriodeType == periodeType &&
                                                                          x.Periode == latestActual.Periode &&
                                                                          x.Kpi.Id == kpi.Id)
                                                   .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiTarget != null && kpiTarget.Value.HasValue)
                        {
                            data.Value = kpiTarget.Value.Value;
                            data.Periode = kpiTarget.Periode;
                        }
                    }
                }

                if (valueAxis == ValueAxis.KpiEconomic && latestActual != null)
                {
                    if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                       (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                       (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                       (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))

                    {
                        var scenarioId = 0;
                        var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard == true);
                        if (scenario != null)
                        {
                            scenarioId = scenario.Id;
                        }
                        var kpiEconomic = DataContext.KeyOperationDatas.Where(x => x.PeriodeType == periodeType &&
                                                                          x.Periode == latestActual.Periode &&
                                                                          x.Kpi.Id == kpi.Id &&
                                                                          x.Scenario.Id == scenarioId)
                                                   .OrderByDescending(x => x.Periode).FirstOrDefault();
                        if (kpiEconomic != null && kpiEconomic.Value.HasValue)
                        {
                            data.Value = kpiEconomic.Value.Value;
                            data.Periode = kpiEconomic.Periode;

                        }
                    }
                }

                if (latestActual != null)
                {
                    switch (periodeType)
                    {
                        case PeriodeType.Hourly:
                            timeInformation = latestActual.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Daily:
                            timeInformation = latestActual.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Monthly:
                            timeInformation = latestActual.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Yearly:
                            timeInformation = latestActual.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                            break;
                    }

                    if (dateTimePeriodes.Count > 0)
                    {
                        dateTimePeriodes[0] = latestActual.Periode;
                    }
                }

                list.Add(data);
            }

            return list;
        }

        public IList<ExportSettingData> GetExportExcelTankData(Dictionary<string, List<string>> labelDictionaries, RangeFilter rangeFilter, DateTime? requestStart, DateTime? requestEnd, PeriodeType periodeType, ArtifactValueInformation valueInformation, out IList<DateTime> dateTimePeriodes, out string timeInformation)
        {
            IList<ExportSettingData> list = new List<ExportSettingData>();
            var volumeExportData = new ExportSettingData();
            var volumeInventoryKpi = labelDictionaries.FirstOrDefault(x => x.Value.Count > 3 ? x.Value[3] == "volume" : false).Value;
            var volumeInventory = new Kpi();
            if (volumeInventoryKpi != null)
            {
                var id = Int32.Parse(volumeInventoryKpi[0]);
                volumeInventory = DataContext.Kpis.Include(x => x.Measurement).Where(x => x.Id == id).First();
                volumeExportData.KpiId = volumeInventory.Id;
                volumeExportData.KpiName = volumeInventory.Name;
                volumeExportData.MeasurementName = volumeInventory.Measurement.Name;
                volumeExportData.ValueAxes = ValueAxis.KpiActual.ToString();
                volumeExportData.KpiGraphicType = "tank";
            }
            //response.VolumeInventoryUnit = volumeInventory.Measurement.Name;
            var daysToTankExportData = new ExportSettingData();
            var daysToTankTopKpi = labelDictionaries.FirstOrDefault(x => x.Value.Count > 3 ? x.Value[3] == "days" : false).Value;
            var daysToTankTop = new Kpi();
            if (daysToTankTopKpi != null)
            {
                var id = Int32.Parse(daysToTankTopKpi[0]);
                daysToTankTop = DataContext.Kpis.Include(x => x.Measurement).Where(x => x.Id == id).First();
                daysToTankExportData.KpiId = daysToTankTop.Id;
                daysToTankExportData.KpiName = daysToTankTop.Name;
                daysToTankExportData.MeasurementName = daysToTankTop.Measurement.Name;
                daysToTankExportData.ValueAxes = ValueAxis.KpiActual.ToString();
                daysToTankExportData.KpiGraphicType = "tank";
            }

            //var daysToTankTop = DataContext.Kpis.Include(x => x.Measurement).Where(x => x.Id == request.Tank.DaysToTankTopId).First();
            //response.DaysToTankTopUnit = daysToTankTop.Measurement.Name;
            //IList<DateTime> dateTimePeriodes = new List<DateTime>();
            //string timeInformation;
            this.GetPeriodes(periodeType, rangeFilter, requestStart, requestEnd, out dateTimePeriodes, out timeInformation);
            var start = dateTimePeriodes[0];
            var end = dateTimePeriodes[dateTimePeriodes.Count - 1];
            volumeExportData.Periode = start;
            daysToTankExportData.Periode = start;

            switch (volumeInventory.YtdFormula)
            {

                case YtdFormula.Sum:
                default:
                    {
                        volumeExportData.Value = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == volumeInventory.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                    }
                    break;
                case YtdFormula.Average:
                    {
                        volumeExportData.Value = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                                   x.Periode >= start && x.Periode <= end && x.Kpi.Id == volumeInventory.Id)
                                    .GroupBy(x => x.Kpi.Id)
                                    .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                    }
                    break;
            }
            switch (daysToTankTop.YtdFormula)
            {
                case YtdFormula.Sum:
                default:
                    {
                        daysToTankExportData.Value = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                                x.Periode >= start && x.Periode <= end && x.Kpi.Id == daysToTankTop.Id)
                                .GroupBy(x => x.Kpi.Id)
                                .Select(x => x.Sum(y => (double?)y.Value ?? 0)).FirstOrDefault();
                    }
                    break;
                case YtdFormula.Average:
                    {
                        daysToTankExportData.Value = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                                    x.Periode >= start && x.Periode <= end && x.Kpi.Id == daysToTankTop.Id)
                                    .GroupBy(x => x.Kpi.Id)
                                    .Select(x => x.Average(y => (double?)y.Value ?? 0)).FirstOrDefault();
                    }
                    break;
            }
            KpiAchievement latestVolInventory = null;
            if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                       (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                       (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                       (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
            {
                var actual = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
              x.Periode <= end && x.Kpi.Id == volumeInventory.Id && (x.Value != null && x.Value.Value != 0))
              .OrderByDescending(x => x.Periode).FirstOrDefault();
                if (actual != null)
                {
                    latestVolInventory = actual;
                    volumeExportData.Value = actual.Value.Value;
                    volumeExportData.Periode = actual.Periode;
                    if (dateTimePeriodes.Count > 0)
                    {
                        dateTimePeriodes[0] = actual.Periode;
                    }
                }
            }
            if (latestVolInventory != null)
            {
                if ((periodeType == PeriodeType.Hourly && rangeFilter == RangeFilter.CurrentHour) ||
                      (periodeType == PeriodeType.Daily && rangeFilter == RangeFilter.CurrentDay) ||
                      (periodeType == PeriodeType.Monthly && rangeFilter == RangeFilter.CurrentMonth) ||
                      (periodeType == PeriodeType.Yearly && rangeFilter == RangeFilter.CurrentYear))
                {
                    var actual = DataContext.KpiAchievements.Where(x => x.PeriodeType == periodeType &&
                  x.Periode == latestVolInventory.Periode && x.Kpi.Id == daysToTankTop.Id && (x.Value != null && x.Value.Value != 0))
                  .OrderByDescending(x => x.Periode).FirstOrDefault();
                    if (actual != null)
                    {
                        daysToTankExportData.Value = actual.Value.Value;
                        daysToTankExportData.Periode = actual.Periode;
                        if (dateTimePeriodes.Count > 0)
                        {
                            dateTimePeriodes[0] = actual.Periode;
                        }
                    }
                    switch (periodeType)
                    {
                        case PeriodeType.Hourly:
                            timeInformation = latestVolInventory.Periode.ToString(DateFormat.Hourly, CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Daily:
                            timeInformation = latestVolInventory.Periode.ToString("dd MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Monthly:
                            timeInformation = latestVolInventory.Periode.ToString("MMM yy", CultureInfo.InvariantCulture);
                            break;
                        case PeriodeType.Yearly:
                            timeInformation = latestVolInventory.Periode.ToString(DateFormat.Yearly, CultureInfo.InvariantCulture);
                            break;
                    }
                }
            }

            volumeExportData.Periode = dateTimePeriodes[0];
            daysToTankExportData.Periode = dateTimePeriodes[0];
            list.Add(volumeExportData);
            list.Add(daysToTankExportData);
            return list;
        }

        //public string[] GetPeriodes(PeriodeType periodeType, RangeFilter rangeFilter, DateTime? Start, DateTime? End, out IList<DateTime> dateTimePeriodes, out string timeInformation)
        //{
        //    throw new NotImplementedException();
        //}

        class DateTimeValue
        {
            public DateTime? Start { get; set; }
            public DateTime? End { get; set; }
            public RangeFilter RangeFilter { get; set; }
        }
    }
}
