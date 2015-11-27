

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Services.Responses.OutputConfig;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System.Collections;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Collections.Generic;
using DSLNG.PEAR.Data.Enums;
using System;
using System.Globalization;
using DSLNG.PEAR.Common.Calculator;

namespace DSLNG.PEAR.Services
{
    public class OutputConfigService : BaseService, IOutputConfigService
    {

        public OutputConfigService(IDataContext dataContext) : base(dataContext) { }

        public GetKpisResponse GetKpis(GetKpisRequest request)
        {
            return new GetKpisResponse
            {
                KpiList = DataContext.Kpis.Where(x => x.Name.Contains(request.Term) && x.IsEconomic == true).Take(20).ToList().MapTo<GetKpisResponse.Kpi>()
            };
        }


        public GetKeyAssumptionsResponse GetKeyAssumptions(GetKeyAssumptionsRequest request)
        {
            return new GetKeyAssumptionsResponse
            {
                KeyAssumptions = DataContext.KeyAssumptionConfigs.Where(x => x.Name.Contains(request.Term)).Take(20).ToList().MapTo<GetKeyAssumptionsResponse.KeyAssumption>()
            };
        }

        public SaveOutputConfigResponse Save(SaveOutputConfigRequest request)
        {
            try
            {
                var outputConfig = request.MapTo<KeyOutputConfiguration>();
                if (request.Id != 0)
                {
                    outputConfig = DataContext.KeyOutputConfigs.Include(x => x.Measurement)
                        .Include(x => x.Category)
                       .Include(x => x.Kpis)
                       .Include(x => x.KeyAssumptions)
                       .First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<KeyOutputConfiguration>(outputConfig);
                    foreach (var kpi in outputConfig.Kpis.ToList())
                    {
                        outputConfig.Kpis.Remove(kpi);
                    }
                    foreach (var assumption in outputConfig.KeyAssumptions.ToList())
                    {
                        outputConfig.KeyAssumptions.Remove(assumption);
                    }
                    if (request.MeasurementId != outputConfig.Measurement.Id)
                    {
                        var measurement = new Measurement { Id = request.MeasurementId };
                        DataContext.Measurements.Attach(measurement);
                        outputConfig.Measurement = measurement;
                    }
                    if (request.CategoryId != outputConfig.Category.Id)
                    {
                        var category = new KeyOutputCategory { Id = request.CategoryId };
                        DataContext.KeyOutputCategories.Attach(category);
                        outputConfig.Category = category;
                    }

                }
                else
                {
                    var measurement = new Measurement { Id = request.MeasurementId };
                    DataContext.Measurements.Attach(measurement);
                    outputConfig.Measurement = measurement;
                    var category = new KeyOutputCategory { Id = request.CategoryId };
                    DataContext.KeyOutputCategories.Attach(category);
                    outputConfig.Category = category;

                    DataContext.KeyOutputConfigs.Add(outputConfig);
                }
                foreach (var kpiId in request.KpiIds)
                {
                    var kpi = new Kpi { Id = kpiId };
                    if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiId) == null)
                    {
                        DataContext.Kpis.Attach(kpi);
                    }
                    else
                    {
                        kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiId);
                    }
                    outputConfig.Kpis.Add(kpi);
                }
                foreach (var assumptionId in request.KeyAssumptionIds)
                {
                    var assumption = new KeyAssumptionConfig { Id = assumptionId };
                    if (DataContext.KeyAssumptionConfigs.Local.FirstOrDefault(x => x.Id == assumptionId) == null)
                    {
                        DataContext.KeyAssumptionConfigs.Attach(assumption);
                    }
                    else
                    {
                        assumption = DataContext.KeyAssumptionConfigs.Local.FirstOrDefault(x => x.Id == assumptionId);
                    }
                    outputConfig.KeyAssumptions.Add(assumption);
                }
                DataContext.SaveChanges();
                return new SaveOutputConfigResponse
                {
                    IsSuccess = true,
                    Message = "The item has been saved successfully"
                };
            }
            catch
            {
                return new SaveOutputConfigResponse
                {
                    IsSuccess = false,
                    Message = "An Error Occured please contact the administrator for further information"
                };
            }
        }


        public GetOutputConfigResponse Get(GetOutputConfigRequest request)
        {
            return DataContext.KeyOutputConfigs.Include(x => x.Measurement)
                .Include(x => x.Category)
                .Include(x => x.Kpis)
                .Include(x => x.KeyAssumptions)
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetOutputConfigResponse>();
        }
        public GetOutputConfigsResponse GetOutputConfigs(GetOutputConfigsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetOutputConfigsResponse
            {
                TotalRecords = totalRecords,
                OutputConfigs = data.ToList().MapTo<GetOutputConfigsResponse.OutputConfig>()
            };
        }


        public IEnumerable<KeyOutputConfiguration> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KeyOutputConfigs.Include(x => x.Category).Include(x => x.Measurement).Include(x => x.KeyAssumptions).Include(x => x.Kpis).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.Category.Name.Contains(search) || x.Measurement.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Name).ThenBy(x => x.Order);
                        break;
                    case "Category":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Category.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Category.Name).ThenBy(x => x.Order);
                        break;
                    case "Measurement":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Measurement.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Measurement.Name).ThenBy(x => x.Order);
                        break;
                    case "Formula":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Formula).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Formula).ThenBy(x => x.Order);
                        break;
                    case "Order":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Order)
                            : data.OrderByDescending(x => x.Order);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.IsActive).ThenBy(x => x.Order);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }




        public CalculateOutputResponse CalculateOputput(CalculateOutputRequest request)
        {
            var resp = new CalculateOutputResponse();
            var categories = DataContext.KeyOutputCategories
                 .Include(x => x.KeyOutputs)
                 .Include(x => x.KeyOutputs.Select(y => y.Measurement))
                 .Include(x => x.KeyOutputs.Select(y => y.Kpis))
                 .Include(x => x.KeyOutputs.Select(y => y.KeyAssumptions))
                 .Where(x => x.IsActive == true).OrderBy(x => x.Order).ToList();
            foreach (var category in categories)
            {
                var categoryResp = category.MapTo<CalculateOutputResponse.OutputCategoryResponse>();
                foreach (var keyOutput in category.KeyOutputs.ToList())
                {
                    var keyOutputResp = this.CalculateFormula(keyOutput, request.ScenarioId);
                    categoryResp.KeyOutputs.Add(keyOutputResp);
                }
                resp.OutputCategories.Add(categoryResp);
            }

            return resp;
        }

        private CalculateOutputResponse.KeyOutputResponse CalculateFormula(KeyOutputConfiguration keyOutput, int scenarioId)
        {
            var resp = keyOutput.MapTo<CalculateOutputResponse.KeyOutputResponse>();
            switch (keyOutput.Formula)
            {
                case Formula.SUM:
                    {
                        var result = this.Sum(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
                case Formula.AVERAGE:
                    {
                        var result = this.Average(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
                case Formula.MIN:
                    {
                        var result = this.Min(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
                //is it daily
                case Formula.MINDATE:
                    {
                        var result = this.MinDate(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
                case Formula.BREAKEVENTYEAR:
                    {
                        var result = this.BreakEventYear(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
                case Formula.PAYBACK:
                    {
                        var result = this.Payback(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
                case Formula.PROFITINVESTMENTRATIO:
                    {
                        var result = this.ProfitInvestmentRatio(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
                case Formula.COMPLETIONDATE:
                    {
                        var result = this.CompletionDate(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
                case Formula.GROSSPROFIT:
                case Formula.NETBACKVALUE:
                    {
                        var result = this.HeaderSubstruction(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
                case Formula.PROJECTIRR:
                case Formula.EQUITYIRR:
                    {
                        var result = this.Irr(keyOutput, scenarioId);
                        resp.Actual = result.Actual;
                        resp.Forecast = result.Forecast;
                    }
                    break;
            }
            return resp;
        }

        private OutputResult Sum(KeyOutputConfiguration keyOutput, int scenarioId)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var result = new OutputResult();
            var kpiId = keyOutput.Kpis[0].Id;
            var startId = keyOutput.KeyAssumptions[0].Id;
            var endId = keyOutput.KeyAssumptions[1].Id;
            var startAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == startId);
            var endAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == endId);
            var startForecast = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endForecast = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var startActual = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endActual = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var forecastValue = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == keyOutput.Id && x.Scenario.Id == scenarioId
                && x.Periode.Year >= startForecast.Year && x.Periode.Year <= endForecast.Year && x.PeriodeType == PeriodeType.Yearly)
                .Sum(x => x.Value);
            if (forecastValue.HasValue) result.Forecast = forecastValue.ToString();

            var pastValue = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year >= startActual.Year && x.Periode.Year < currentYear && x.PeriodeType == PeriodeType.Yearly).Sum(x => x.Value);
            var futureValue = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year <= endActual.Year && x.Periode.Year > currentYear && x.PeriodeType == PeriodeType.Yearly && x.Scenario.Id == scenarioId).Sum(x => x.Value);
            var untilNowThisYearValue = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId && x.Periode.Year == currentYear
                && x.Periode.Month < currentMonth && x.PeriodeType == PeriodeType.Monthly).Sum(x => x.Value);
            var thisYearForecastValue = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId && x.Periode.Year == currentYear
                && x.Periode.Month >= currentMonth && x.PeriodeType == PeriodeType.Monthly && x.Scenario.Id == scenarioId).Sum(x => x.Value);
            var actualValue = pastValue + futureValue + untilNowThisYearValue + thisYearForecastValue;
            if (actualValue.HasValue) result.Actual = actualValue.ToString();

            return result;
        }

        private OutputResult Average(KeyOutputConfiguration keyOutput, int scenarioId)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var result = new OutputResult();
            var kpiId = keyOutput.Kpis[0].Id;
            var startId = keyOutput.KeyAssumptions[0].Id;
            var endId = keyOutput.KeyAssumptions[1].Id;
            var startAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == startId);
            var endAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == endId);
            var startForecast = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endForecast = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var startActual = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endActual = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var forecastValue = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == keyOutput.Id && x.Scenario.Id == scenarioId
                && x.Periode.Year >= startForecast.Year && x.Periode.Year <= endForecast.Year && x.PeriodeType == PeriodeType.Yearly)
                .Average(x => x.Value);
            if (forecastValue.HasValue) result.Forecast = forecastValue.ToString();

            var pastValues = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId && x.Periode.Year >= startActual.Year
                && x.Periode.Year < currentYear && x.PeriodeType == PeriodeType.Yearly).Select(x => x.Value).ToList();
            var futureValues = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year <= endActual.Year && x.Periode.Year > currentYear && x.PeriodeType == PeriodeType.Yearly && x.Scenario.Id == scenarioId).Select(x => x.Value).ToList();

            var untilNowThisYearValues = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId && x.Periode.Year == currentYear
                && x.Periode.Month < currentMonth && x.PeriodeType == PeriodeType.Monthly).Select(x => x.Value).ToList();
            var thisYearForecastValues = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId && x.Periode.Year == currentYear
                && x.Periode.Month >= currentMonth && x.PeriodeType == PeriodeType.Monthly && x.Scenario.Id == scenarioId).Select(x => x.Value).ToList();
            var currentYearValue = untilNowThisYearValues.Concat(thisYearForecastValues).Sum();
            pastValues.Add(currentYearValue);

            var actualValue = pastValues.Concat(futureValues).Average();
            if (actualValue.HasValue) result.Actual = actualValue.ToString();

            return result;
        }

        private OutputResult Min(KeyOutputConfiguration keyOutput, int scenarioId)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDate = DateTime.Now;
            var result = new OutputResult();
            var kpiId = keyOutput.Kpis[0].Id;
            var startId = keyOutput.KeyAssumptions[0].Id;
            var endId = keyOutput.KeyAssumptions[1].Id;
            var startAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == startId);
            var endAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == endId);
            var startForecast = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endForecast = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var startActual = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endActual = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var forecastValue = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == keyOutput.Id && x.Scenario.Id == scenarioId
                && x.Periode >= startForecast && x.Periode <= endForecast && x.PeriodeType == PeriodeType.Monthly)
                .Min(x => x.Value);
            if (forecastValue.HasValue) result.Forecast = forecastValue.Value.ToString();

            var pastValues = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
               && x.Periode >= startActual && x.Periode < currentDate && x.PeriodeType == PeriodeType.Monthly).Select(x => x.Value).ToList();
            var futureValues = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId
                && x.Periode >= currentDate && x.Periode <= endForecast && x.PeriodeType == PeriodeType.Monthly && x.Scenario.Id == scenarioId).Select(x => x.Value).ToList();
            var minActual = pastValues.Concat(futureValues).Min();
            if (minActual != null) result.Actual = minActual.ToString();
            return result;
        }

        private OutputResult MinDate(KeyOutputConfiguration keyOutput, int scenarioId)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDate = DateTime.Now;
            var result = new OutputResult();
            var kpiId = keyOutput.Kpis[0].Id;
            var startId = keyOutput.KeyAssumptions[0].Id;
            var endId = keyOutput.KeyAssumptions[1].Id;
            var startAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == startId);
            var endAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == endId);
            var startForecast = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endForecast = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var startActual = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endActual = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var minForecast = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == keyOutput.Id && x.Scenario.Id == scenarioId
                && x.Periode >= startForecast && x.Periode <= endForecast && x.PeriodeType == PeriodeType.Monthly)
                .OrderBy(x => x.Value).FirstOrDefault();
            if (minForecast != null) result.Forecast = minForecast.Periode.AddMonths(1).AddDays(-1).ToString();
            var pastValues = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
                && x.Periode >= startActual && x.Periode < currentDate && x.PeriodeType == PeriodeType.Monthly).Select(x => new
                {
                    Periode = x.Periode,
                    Value = x.Value
                });
            var futureValues = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId
                && x.Periode >= currentDate && x.Periode <= endForecast && x.PeriodeType == PeriodeType.Monthly && x.Scenario.Id == scenarioId).Select(x => new
                {
                    Periode = x.Periode,
                    Value = x.Value
                });

            var minActual = pastValues.Concat(futureValues).OrderBy(x => x.Value).FirstOrDefault();
            if (minActual != null) result.Actual = minActual.Periode.AddMonths(1).AddDays(-1).ToString();
            return result;
        }

        private OutputResult BreakEventYear(KeyOutputConfiguration keyOutput, int scenarioId)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDate = DateTime.Now;
            var result = new OutputResult();
            var kpiId = keyOutput.Kpis[0].Id;
            var startId = keyOutput.KeyAssumptions[0].Id;
            var endId = keyOutput.KeyAssumptions[1].Id;
            var startAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == startId);
            var endAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == endId);
            var startForecast = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endForecast = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var startActual = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endActual = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var forecast = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId && x.Scenario.Id == scenarioId
                && x.Periode.Year >= startForecast.Year && x.Periode.Year <= endForecast.Year
                && x.PeriodeType == PeriodeType.Yearly).OrderBy(x => x.Value).FirstOrDefault(x => x.Value > 0);
            if (forecast != null) result.Forecast = forecast.Periode.Year.ToString();

            var pastValues = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
               && x.Periode.Year >= startActual.Year && x.Periode.Year < currentYear
               && x.PeriodeType == PeriodeType.Yearly).Select(x => new { Value = x.Value, Periode = x.Periode }).ToList();
            var futureValues = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year <= endActual.Year && x.Periode.Year > currentYear
                && x.PeriodeType == PeriodeType.Yearly && x.Scenario.Id == scenarioId).Select(x => new { Value = x.Value, Periode = x.Periode }).ToList();
            var untilNowThisYearValue = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year == currentYear && x.Periode.Month < currentMonth
                && x.PeriodeType == PeriodeType.Monthly).Sum(x => x.Value);
            var thisYearForecastValue = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year == currentYear && x.Periode.Month >= currentMonth
                && x.PeriodeType == PeriodeType.Monthly && x.Scenario.Id == scenarioId).Sum(x => x.Value);
            var currentYearValue = new
            {
                Value = thisYearForecastValue + untilNowThisYearValue,
                Periode = DateTime.Now
            };
            pastValues.Add(currentYearValue);
            var actual = pastValues.Concat(futureValues).OrderBy(x => x.Value).FirstOrDefault(x => x.Value > 0);
            if (actual != null) result.Actual = actual.Periode.Year.ToString();

            return result;
        }

        private OutputResult Payback(KeyOutputConfiguration keyOutput, int scenarioId)
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDate = DateTime.Now;
            var result = new OutputResult();
            var kpiId = keyOutput.Kpis[0].Id;
            var startId = keyOutput.KeyAssumptions[0].Id;
            var endId = keyOutput.KeyAssumptions[1].Id;
            var startAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == startId);
            var endAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == endId);
            var startForecast = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endForecast = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var startActual = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endActual = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);

            var forecastList = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId && x.Scenario.Id == scenarioId
               && x.Periode.Year >= startForecast.Year && x.Periode.Year <= endForecast.Year
               && x.PeriodeType == PeriodeType.Yearly).ToList();
            var forecast = forecastList.OrderBy(x => x.Value).FirstOrDefault(x => x.Value > 0);
            var breakEventYearWeight = 1;
            if (forecast != null)
            {
                var prev = forecastList.FirstOrDefault(x => x.Periode.Year == (forecast.Periode.Year - 1));
                if (prev.Value - forecast.Value != 0)
                {
                    breakEventYearWeight = int.Parse(string.Format("{0:0.0}", (prev.Value / (prev.Value - forecast.Value))));
                }
                result.Forecast = (forecast.Periode.Year - startForecast.Year + breakEventYearWeight).ToString();
            }

            var pastValues = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
               && x.Periode.Year >= startActual.Year && x.Periode.Year < currentYear
               && x.PeriodeType == PeriodeType.Yearly).Select(x => new { Value = x.Value, Periode = x.Periode }).ToList();
            var futureValues = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year <= endActual.Year && x.Periode.Year > currentYear
                && x.PeriodeType == PeriodeType.Yearly && x.Scenario.Id == scenarioId).Select(x => new { Value = x.Value, Periode = x.Periode }).ToList();
            var untilNowThisYearValue = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year == currentYear && x.Periode.Month < currentMonth
                && x.PeriodeType == PeriodeType.Monthly).Sum(x => x.Value);
            var thisYearForecastValue = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year == currentYear && x.Periode.Month >= currentMonth
                && x.PeriodeType == PeriodeType.Monthly && x.Scenario.Id == scenarioId).Sum(x => x.Value);
            var currentYearValue = new
            {
                Value = thisYearForecastValue + untilNowThisYearValue,
                Periode = DateTime.Now
            };
            pastValues.Add(currentYearValue);
            var actualList = pastValues.Concat(futureValues);
            var actual = actualList.OrderBy(x => x.Value).FirstOrDefault(x => x.Value > 0);

            var actualBreakEventYearWeight = 1;
            if (actual != null)
            {
                var prev = actualList.FirstOrDefault(x => x.Periode.Year == (forecast.Periode.Year - 1));
                if (prev.Value - actual.Value != 0)
                {
                    actualBreakEventYearWeight = int.Parse(string.Format("{0:0.0}", (prev.Value / (prev.Value - actual.Value))));
                }
                result.Actual = (actual.Periode.Year - startForecast.Year + actualBreakEventYearWeight).ToString();
            }

            return result;

        }

        private OutputResult ProfitInvestmentRatio(KeyOutputConfiguration keyOutput, int scenarioId)
        {
            var sumResult = this.Sum(keyOutput, scenarioId);
            var result = new OutputResult();
            result.Actual = String.Format("{0:0.0}", double.Parse(sumResult.Actual) /
                double.Parse(DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == keyOutput.KeyAssumptions[2].Id)
                .ActualValue));
            result.Forecast = String.Format("{0:0.0}", double.Parse(sumResult.Forecast) /
                double.Parse(DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == keyOutput.KeyAssumptions[2].Id)
                .ForecastValue));
            
            return result;
        }

        private OutputResult CompletionDate(KeyOutputConfiguration keyOutput, int scenarioId) 
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDate = DateTime.Now;
            var result = new OutputResult();
            var kpiId = keyOutput.Kpis[0].Id;
            var startId = keyOutput.KeyAssumptions[0].Id;
            var endId = keyOutput.KeyAssumptions[1].Id;
            var completionId = keyOutput.KeyAssumptions[2].Id;
            var startAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == startId);
            var endAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == endId);
            var startForecast = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endForecast = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var startActual = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endActual = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var completionAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == completionId);
            var completionForecast =  DateTime.ParseExact(completionAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var completionActual =  DateTime.ParseExact(completionAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var forecast = DataContext.KeyOperationDatas.FirstOrDefault(x => x.Kpi.Id == kpiId && x.Scenario.Id == scenarioId
                && startForecast <= x.Periode && x.Periode <= endForecast
                && x.PeriodeType == PeriodeType.Yearly
                && x.Periode.Year == completionForecast.Year && x.Periode.Month == completionForecast.Month);
            if (forecast != null) result.Forecast = forecast.Value.ToString();

            if (currentYear == completionActual.Year)
            {
                var pastMonthsThisYear = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
                && startForecast <= x.Periode && x.Periode <= endForecast
                && x.PeriodeType == PeriodeType.Monthly
                && x.Periode.Year == completionForecast.Year && x.Periode.Month < currentMonth).Sum(x => x.Value);

                var nextMonthThisYear = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId && scenarioId == x.Scenario.Id
                && startForecast <= x.Periode && x.Periode <= endForecast
                && x.PeriodeType == PeriodeType.Monthly
                && x.Periode.Year == completionForecast.Year && x.Periode.Month > currentMonth).Sum(x => x.Value);

                if (pastMonthsThisYear.HasValue || nextMonthThisYear.HasValue) {
                    result.Actual = (pastMonthsThisYear + nextMonthThisYear).ToString();
                }
            }
            else {
                var actual = DataContext.KpiAchievements.FirstOrDefault(x => x.Kpi.Id == kpiId
                && startForecast <= x.Periode && x.Periode <= endForecast
                && x.PeriodeType == PeriodeType.Yearly
                && x.Periode.Year == completionForecast.Year && x.Periode.Month == completionForecast.Month);
                if (actual != null) result.Actual = actual.ToString();
            }
            return result;
        }

        private OutputResult HeaderSubstruction(KeyOutputConfiguration keyOutput, int scenarioId) {
            var result = new OutputResult();
            var kpiIds = keyOutput.Kpis.Select(x => x.Id).ToList();
            var headId = kpiIds[0];
            var forecastValues = DataContext.KeyOperationDatas.Include(x => x.Kpi).Where(x => x.Scenario.Id == scenarioId
                && kpiIds.Contains(x.Kpi.Id)).ToList();
            var forecastValue = forecastValues.First(x => x.Kpi.Id == headId).Value;
            foreach (var forecastData in forecastValues) {
                if (forecastData.Kpi.Id != headId) {
                    forecastValue = forecastValue - forecastData.Value;
                }
            }
            if (forecastValue.HasValue) result.Forecast = forecastValue.ToString();

            var actualValues = DataContext.KpiAchievements.Include(x => x.Kpi).Where(x => kpiIds.Contains(x.Kpi.Id)).ToList();
            var actualValue = actualValues.First(x => x.Kpi.Id == headId).Value;
            foreach (var actualData in actualValues)
            {
                if (actualData.Kpi.Id != headId)
                {
                    actualValue = actualValue - actualData.Value;
                }
            }
            if (actualValue.HasValue) result.Actual = actualValue.ToString();
            return result;
        }

        private OutputResult Irr(KeyOutputConfiguration keyOutput, int scenarioId) {
             var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDate = DateTime.Now;
            var result = new OutputResult();
            var kpiId = keyOutput.Kpis[0].Id;
            var startId = keyOutput.KeyAssumptions[0].Id;
            var endId = keyOutput.KeyAssumptions[1].Id;
            var completionId = keyOutput.KeyAssumptions[2].Id;
            var startAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == startId);
            var endAssumption = DataContext.KeyAssumptionDatas.First(x => x.KeyAssumptionConfig.Id == endId);
            var startForecast = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endForecast = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ForecastValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ForecastValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var startActual = startAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(startAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(startAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);
            var endActual = endAssumption.ForecastValue.Length == 4 ? DateTime.ParseExact(endAssumption.ActualValue, "YYYY", CultureInfo.InvariantCulture) : DateTime.ParseExact(endAssumption.ActualValue, "MM-YYYY", CultureInfo.InvariantCulture);

            var forecast = DataContext.KeyOperationDatas.Where(x => x.Scenario.Id == scenarioId && x.Kpi.Id == kpiId
                    && startForecast <= x.Periode && x.Periode <= endForecast
                && x.PeriodeType == PeriodeType.Yearly).OrderBy(x => x.Periode).Select(x => x.Value.Value).ToList().ToArray();
            var forecastIrrCalculator = new NewtonRaphsonIRRCalculator(forecast);
            result.Forecast = forecastIrrCalculator.Execute().ToString();

            var past = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
                && startActual <= x.Periode && x.Periode.Year < currentYear
                && x.PeriodeType == PeriodeType.Yearly).OrderBy(x => x.Periode).Select(x => x.Value.Value).ToList();
            var future = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId && x.Scenario.Id == scenarioId
                && x.Periode <= endActual && x.Periode.Year > currentYear
                && x.PeriodeType == PeriodeType.Yearly).OrderBy(x => x.Periode).Select(x => x.Value.Value).ToList();
            var currentPastMonths = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpiId
                && x.Periode.Year == currentYear && x.Periode.Month < currentMonth
                && x.PeriodeType == PeriodeType.Monthly).OrderBy(x => x.Periode).Sum(x => x.Value.Value);
            var currentNextMonths = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpiId && x.Scenario.Id == scenarioId
                && x.Periode.Year == currentYear && x.Periode.Month >= currentMonth
                && x.PeriodeType == PeriodeType.Monthly).OrderBy(x => x.Periode).Sum(x => x.Value.Value);
             past.Add(currentPastMonths + currentNextMonths);
             var actualArray = past.Concat(future).ToArray();
             var actualIrrCalculator = new NewtonRaphsonIRRCalculator(actualArray);
             result.Actual = actualIrrCalculator.Execute().ToString();

             return result;

        }


        public DeleteOutputConfigResponse DeleteOutput(DeleteOutputConfigRequest request)
        {
            var output = DataContext.KeyOutputConfigs.FirstOrDefault(x => x.Id == request.Id);
            if (output != null)
            {
                DataContext.KeyOutputConfigs.Attach(output);
                DataContext.KeyOutputConfigs.Remove(output);
                DataContext.SaveChanges();
            }
            return new DeleteOutputConfigResponse
            {
                IsSuccess = true,
                Message = "The Output Config has been deleted successfully"
            };
        }
    }


    public class OutputResult
    {
        public string Actual { get; set; }
        public string Forecast { get; set; }
    }
}
