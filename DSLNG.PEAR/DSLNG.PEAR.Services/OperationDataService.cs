using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OperationalData;
using DSLNG.PEAR.Services.Responses.OperationalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class OperationDataService : BaseService, IOperationDataService
    {
        public OperationDataService(IDataContext context) : base(context) { }




        public GetOperationalDatasResponse GetOperationalDatas(GetOperationalDatasRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetOperationalDatasResponse
            {
                TotalRecords = totalRecords,
                OperationalDatas = data.ToList().MapTo<GetOperationalDatasResponse.OperationalData>()
            };
            //if (request.OnlyCount)
            //{
            //    return new GetOperationalDatasResponse { Count = DataContext.KeyOperationDatas.Count() };
            //}
            //else
            //{
            //    return new GetOperationalDatasResponse
            //    {
            //        OperationalDatas = DataContext.KeyOperationDatas.OrderByDescending(x => x.Id)
            //        .Include(x => x.KeyOperation).Include(x => x.Kpi)
            //        .Skip(request.Skip).Take(request.Take).ToList().MapTo<GetOperationalDatasResponse.OperationalData>()
            //    };
            //}
        }


        public GetOperationalSelectListResponse GetOperationalSelectList()
        {
            return new GetOperationalSelectListResponse
            {
                Operations = DataContext.KeyOperationConfigs.ToList().MapTo<GetOperationalSelectListResponse.Operation>(),
                KPIS = DataContext.Kpis.ToList().MapTo<GetOperationalSelectListResponse.KPI>()
            };
        }


        public SaveOperationalDataResponse SaveOperationalData(SaveOperationalDataRequest request)
        {
            //if (request.Id == 0)
            //{
            //    var OperationalData = request.MapTo<KeyOperationData>();
            //    OperationalData.KeyOperation = DataContext.KeyOperations.FirstOrDefault(x => x.Id == request.IdKeyOperation);
            //    OperationalData.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.IdKPI);
            //    DataContext.KeyOperationDatas.Add(OperationalData);

            //}
            //else
            //{
            //    var OperationalData = DataContext.KeyOperationDatas.FirstOrDefault(x => x.Id == request.Id);
            //    if (OperationalData != null)
            //    {
            //        var operational = request.MapPropertiesToInstance<KeyOperationData>(OperationalData);
            //        operational.KeyOperation = DataContext.KeyOperations.FirstOrDefault(x => x.Id == request.IdKeyOperation);
            //        operational.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.IdKPI);
            //    }
            //}
            //DataContext.SaveChanges();
            //return new SaveOperationalDataResponse
            //{
            //    IsSuccess = true,
            //    Message = "Operational Data has been Save"
            //};
            throw new NotImplementedException();
        }


        public GetOperationalDataResponse GetOperationalData(GetOperationalDataRequest request)
        {
            //return DataContext.KeyOperationDatas
            //    .Include(x => x.KeyOperation).Include(x => x.Kpi)
            //    .FirstOrDefault(x => x.Id == request.Id).MapTo<GetOperationalDataResponse>();
            throw new NotImplementedException();
        }


        public DeleteOperationalDataResponse DeleteOperationalData(DeleteOperationalDataRequest request)
        {
            var checkId = DataContext.KeyOperationDatas.FirstOrDefault(x => x.Id == request.Id);
            if (checkId != null)
            {
                DataContext.KeyOperationDatas.Attach(checkId);
                DataContext.KeyOperationDatas.Remove(checkId);
                DataContext.SaveChanges();
            }
            return new DeleteOperationalDataResponse
            {
                IsSuccess = true,
                Message = "Operational Data has been deleted successfully"
            };
        }

        public GetOperationalDataDetailResponse GetOperationalDataDetail(GetOperationalDataDetailRequest request)
        {
            var response = new GetOperationalDataDetailResponse();
            //var a = DataContext.KeyOperationConfigs.Include(x => x.Kpi).ToList();
            var configs = DataContext.KeyOperationConfigs
                    .Include(x => x.Kpi)
                    .Include(x => x.Kpi.Measurement)
                    .Include(x => x.KeyOperationGroup)
                    .AsEnumerable()
                    .OrderBy(x => x.KeyOperationGroup.Order).ThenBy(x => x.Order)
                    .GroupBy(x => x.KeyOperationGroup)
                    .ToDictionary(x => x.Key);

            foreach (var config in configs)
            {
                var configResponse = new List<GetOperationalDataDetailResponse.KeyOperationConfig>();
                foreach (var item in config.Value)
                {
                    configResponse.Add(new GetOperationalDataDetailResponse.KeyOperationConfig
                        {
                            Desc = item.Desc,
                            Id = item.Id,
                            IsActive = item.IsActive,
                            Kpi = new GetOperationalDataDetailResponse.Kpi { Id = item.Kpi.Id, Name = item.Kpi.Name, MeasurementName = item.Kpi.Measurement.Name },
                            Order = item.Order
                        });
                }

                response.KeyOperationGroups.Add(new GetOperationalDataDetailResponse.KeyOperationGroup
                    {
                        Id = config.Key.Id,
                        IsActive = config.Key.IsActive,
                        Name = config.Key.Name,
                        Order = config.Key.Order,
                        Remark = config.Key.Remark,
                        KeyOperationConfigs = configResponse
                    });
            }

            return response;
        }

        public GetOperationDataConfigurationResponse GetOperationDataConfiguration(GetOperationDataConfigurationRequest request)
        {
            var response = new GetOperationDataConfigurationResponse();
            response.GroupId = request.GroupId;
            response.ScenarioId = request.ScenarioId;
            try
            {
                var periodeType = request.PeriodeType;

                var keyOperationConfigs = DataContext.KeyOperationConfigs
                    .Include(x => x.Kpi)
                    .Include(x => x.Kpi.Measurement)
                    .Where(x => x.KeyOperationGroup.Id == request.GroupId).ToList();

                switch (periodeType)
                {
                    case PeriodeType.Yearly:
                        var operationDataYearly =
                            DataContext.KeyOperationDatas
                            .Include(x => x.Kpi)
                            .Include(x => x.Scenario)
                            .Include(x => x.KeyOperationConfig)
                            .Where(x => x.PeriodeType == PeriodeType.Yearly && x.Scenario.Id == request.ScenarioId).ToList();

                        foreach (var keyOperationConfig in keyOperationConfigs)
                        {
                            var kpiDto = keyOperationConfig.Kpi.MapTo<GetOperationDataConfigurationResponse.Kpi>();
                            foreach (var number in YearlyNumbers)
                            {
                                var operation = operationDataYearly.SingleOrDefault(x => x.Kpi.Id == keyOperationConfig.Kpi.Id && x.Periode.Year == number);
                                
                                if (operation != null)
                                {
                                    var operationtDataDto =
                                        operation.MapTo<GetOperationDataConfigurationResponse.OperationData>();
                                    operationtDataDto.ScenarioId = request.ScenarioId;
                                    operationtDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                    kpiDto.OperationDatas.Add(operationtDataDto);
                                }
                                else
                                {
                                    var operationtDataDto = new GetOperationDataConfigurationResponse.OperationData();
                                    operationtDataDto.Periode = new DateTime(number, 1, 1);
                                    operationtDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                    kpiDto.OperationDatas.Add(operationtDataDto);
                                }
                            }

                            response.Kpis.Add(kpiDto);
                        }
                        break;
                    case PeriodeType.Monthly:
                        var operationDataMonthly = DataContext.KeyOperationDatas
                                        .Include(x => x.Kpi)
                                        .Include(x => x.Scenario)
                                        .Include(x => x.KeyOperationConfig)
                                        .Where(x => x.PeriodeType == PeriodeType.Monthly && x.Periode.Year == request.Year).ToList();

                        foreach (var keyOperationConfig in keyOperationConfigs)
                        {
                            var kpiDto = keyOperationConfig.Kpi.MapTo<GetOperationDataConfigurationResponse.Kpi>();
                            KeyOperationConfig config = keyOperationConfig;
                            var operationDatas = operationDataMonthly.Where(x => x.Kpi.Id == config.Kpi.Id).ToList();
                            for (int i = 1; i <= 12; i++)
                            {
                                var operationData = operationDatas.FirstOrDefault(x => x.Periode.Month == i);
                                if (operationData != null)
                                {
                                    var operationDataDto =
                                        operationData.MapTo<GetOperationDataConfigurationResponse.OperationData>();
                                    operationDataDto.ScenarioId = request.ScenarioId;
                                    operationDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                    kpiDto.OperationDatas.Add(operationDataDto);
                                }
                                else
                                {
                                    var operationDataDto = new GetOperationDataConfigurationResponse.OperationData();
                                    operationDataDto.Periode = new DateTime(request.Year, i, 1);
                                    operationDataDto.ScenarioId = request.ScenarioId;
                                    operationDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                    kpiDto.OperationDatas.Add(operationDataDto);
                                }
                            }
                            response.Kpis.Add(kpiDto);
                        }
                        break;

                }

                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public UpdateOperationDataResponse Update(UpdateOperationDataRequest request)
        {
            var response = new UpdateOperationDataResponse();
            try
            {
                var operationData = request.MapTo<KeyOperationData>();
                
                if (operationData.Id > 0)
                {
                    operationData = DataContext.KeyOperationDatas.Single(x => x.Id == operationData.Id);
                    request.MapPropertiesToInstance(operationData);
                }
                else
                {
                    DataContext.KeyOperationDatas.Add(operationData);
                }
                operationData.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                operationData.Scenario = DataContext.Scenarios.Single(x => x.Id == request.ScenarioId);
                operationData.KeyOperationConfig = DataContext.KeyOperationConfigs.Single(x => x.Id == request.KeyOperationConfigId);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Key Operation Data has been updated successfully";
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

        public IEnumerable<KeyOperationData> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            //var data = DataContext.KeyOperationDatas.Include(x => x.KeyOperation).Include(x => x.Kpi).AsQueryable();
            //if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            //{
            //    data = data.Where(x => x.KeyOperation.Name.Contains(search) || x.Kpi.Name.Contains(search));
            //}

            //foreach (var sortOrder in sortingDictionary)
            //{
            //    switch(sortOrder.Key)
            //    {
            //        case "KeyOperation":
            //            data = sortOrder.Value == SortOrder.Ascending
            //                ? data.OrderBy(x => x.KeyOperation.Name)
            //                : data.OrderByDescending(x => x.KeyOperation.Name);
            //            break;
            //        case "Kpi" :
            //            data = sortOrder.Value == SortOrder.Ascending
            //                ? data.OrderBy(x => x.Kpi.Name)
            //                : data.OrderByDescending(x => x.Kpi.Name);
            //            break;
            //        case "Value":
            //            data = sortOrder.Value == SortOrder.Ascending
            //                ? data.OrderBy(x => x.Value)
            //                : data.OrderByDescending(x => x.ActualValue);
            //            break;
            //        case "ForecastValue":
            //            data = sortOrder.Value == SortOrder.Ascending
            //                ? data.OrderBy(x => x.ForecastValue)
            //                : data.OrderByDescending(x => x.ForecastValue);
            //            break;
            //    }
            //}

            //TotalRecords = data.Count();
            //return data;
            throw new NotImplementedException();


        }
    }
}
