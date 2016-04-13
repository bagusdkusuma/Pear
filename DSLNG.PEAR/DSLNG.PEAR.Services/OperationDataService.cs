using System.Globalization;
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
using DSLNG.PEAR.Services.Responses;

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
            //    return new GetOperationalDatasResponse { Count = DataContext.KeyOperasionalDatas.Count() };
            //}
            //else
            //{
            //    return new GetOperationalDatasResponse
            //    {
            //        OperationalDatas = DataContext.KeyOperasionalDatas.OrderByDescending(x => x.Id)
            //        .Include(x => x.KeyOperation).Include(x => x.Kpi)
            //        .Skip(request.Skip).Take(request.Take).ToList().MapTo<GetOperationalDatasResponse.OperationalData>()
            //    };
            //}
        }


        public GetOperationalSelectListResponse GetOperationalSelectList()
        {
            return new GetOperationalSelectListResponse
            {
                Operations = DataContext.KeyOperationDatas.ToList().MapTo<GetOperationalSelectListResponse.Operation>(),
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
            //    DataContext.KeyOperasionalDatas.Add(OperationalData);

            //}
            //else
            //{
            //    var OperationalData = DataContext.KeyOperasionalDatas.FirstOrDefault(x => x.Id == request.Id);
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
            //return DataContext.KeyOperasionalDatas
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
                    .Where(x => x.IsActive && x.KeyOperationGroup != null)
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
            response.RoleGroupId = request.RoleGroupId;
            response.ScenarioId = request.ScenarioId;
            if (request.RoleGroupId > 0)
            {
                response.GroupName =
                DataContext.KeyOperationGroups.Single(x => x.Id == request.RoleGroupId).Name;
            }

            var periodeType = request.PeriodeType;
            List<KeyOperationConfig> keyOperationConfigs;
            if (request.RoleGroupId > 0)
            {
                keyOperationConfigs = DataContext.KeyOperationConfigs
                                                     .Include(x => x.Kpi)
                                                     .Include(x => x.Kpi.Measurement)
                                                     .Where(x => x.KeyOperationGroup != null && x.KeyOperationGroup.Id == request.RoleGroupId && x.IsActive)
                                                     .OrderBy(x => x.KeyOperationGroup.Order)
                                                     .ThenBy(x => x.Order).ToList();
            }
            else
            {
                keyOperationConfigs = DataContext.KeyOperationConfigs
                .Include(x => x.Kpi)
                .Include(x => x.Kpi.Measurement)
                .Include(x => x.KeyOperationGroup)
                .Where(x => x.IsActive && x.KeyOperationGroup != null)
                .OrderBy(x => x.KeyOperationGroup.Order)
                .ThenBy(x => x.Order).ToList();
            }


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
                        kpiDto.GroupName = keyOperationConfig.KeyOperationGroup.Name;
                        foreach (var number in YearlyNumbersForOperationData)
                        {
                            var operation = operationDataYearly.FirstOrDefault(x => x.Kpi != null && x.Kpi.Id == keyOperationConfig.Kpi.Id && x.Periode.Year == number);

                            if (operation != null)
                            {
                                var operationtDataDto =
                                    operation.MapTo<GetOperationDataConfigurationResponse.OperationData>();
                                operationtDataDto.ScenarioId = request.ScenarioId;
                                operationtDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                kpiDto.OperationData.Add(operationtDataDto);
                            }
                            else
                            {
                                var operationtDataDto = new GetOperationDataConfigurationResponse.OperationData();
                                operationtDataDto.Periode = new DateTime(number, 1, 1);
                                operationtDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                kpiDto.OperationData.Add(operationtDataDto);
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
                                    .Where(x => x.PeriodeType == PeriodeType.Monthly && x.Periode.Year == request.Year && x.Scenario.Id == request.ScenarioId).ToList();

                    foreach (var keyOperationConfig in keyOperationConfigs)
                    {
                        var kpiDto = keyOperationConfig.Kpi.MapTo<GetOperationDataConfigurationResponse.Kpi>();
                        kpiDto.GroupName = keyOperationConfig.KeyOperationGroup.Name;
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
                                kpiDto.OperationData.Add(operationDataDto);
                            }
                            else
                            {
                                var operationDataDto = new GetOperationDataConfigurationResponse.OperationData();
                                operationDataDto.Periode = new DateTime(request.Year, i, 1);
                                operationDataDto.ScenarioId = request.ScenarioId;
                                operationDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                kpiDto.OperationData.Add(operationDataDto);
                            }
                        }
                        response.Kpis.Add(kpiDto);
                    }
                    break;

            }

            response.IsSuccess = true;


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
                response.Id = operationData.Id;
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

        /*public ViewOperationDataConfigurationResponse ViewOperationDataConfiguration(int scenarioId, PeriodeType periodeType)
        {
            var keyOperationConfigs = DataContext.
                    .Include(x => x.Kpi)
                    .Include(x => x.Kpi.Measurement)
                    .Where(x => x.).ToList();
            switch (periodeType)
            {
                case PeriodeType.Yearly:
                    var operationDataYearly =
                            DataContext.KeyOperationDatas
                            .Include(x => x.Kpi)
                            .Include(x => x.Scenario)
                            .Include(x => x.KeyOperationConfig)
                            .Where(x => x.PeriodeType == PeriodeType.Yearly && x.Scenario.Id == scenarioId).ToList();
                    break;
                case PeriodeType.Monthly:
                    break;
            }
        }*/

        private IEnumerable<KeyOperationData> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KeyOperationDatas.Include(x => x.KeyOperationConfig).Include(x => x.Kpi)
                .Include(x => x.KeyOperationConfig.Kpi).Include(x => x.Scenario).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Kpi.Name.Contains(search) || x.Scenario.Name.Contains(search) || x.KeyOperationConfig.Kpi.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Scenario":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Scenario.Name)
                            : data.OrderByDescending(x => x.Scenario.Name);
                        break;
                    case "KeyOperation":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.KeyOperationConfig.Kpi.Name)
                            : data.OrderByDescending(x => x.KeyOperationConfig.Kpi.Name);
                        break;
                    case "Kpi":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Kpi.Name)
                            : data.OrderByDescending(x => x.Kpi.Name);
                        break;
                    case "Value":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Value)
                            : data.OrderByDescending(x => x.Value);
                        break;
                    case "Periode":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Periode)
                            : data.OrderByDescending(x => x.Periode);
                        break;
                    case "PeriodeType":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.PeriodeType)
                            : data.OrderByDescending(x => x.PeriodeType);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
            //throw new NotImplementedException();


        }



        public GetOperationIdResponse GetOperationId(List<int> list_Kpi)
        {
            return new GetOperationIdResponse
            {
                OperationDatas = DataContext.KeyOperationDatas.Include(x => x.Kpi).Include(x => x.KeyOperationConfig).Include(x => x.Scenario).Where(x => list_Kpi.Contains(x.Kpi.Id)).ToList().MapTo<GetOperationIdResponse.OperationData>()
            };
        }


        public BaseResponse BatchUpdateOperationDatas(BatchUpdateOperationDataRequest request)
        {
            //var response = new BaseResponse();
            //try
            //{
            //    int i = 0;
            //    foreach (var item in request.BatchUpdateOperationDataItemRequest)
            //    {
            //        var operationData = item.MapTo<KeyOperationData>();
            //        operationData.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == item.KpiId);
            //        operationData.Scenario = DataContext.Scenarios.FirstOrDefault(x => x.Id == item.ScenarioId);
            //        operationData.KeyOperationConfig = DataContext.KeyOperationConfigs.FirstOrDefault(x => x.Id == item.KeyOperationConfigId);
            //        var exist = DataContext.KeyOperationDatas.FirstOrDefault(x => x.Kpi.Id == item.KpiId && x.PeriodeType == item.PeriodeType && x.Periode == item.Periode && x.Value == item.Value && x.Remark == item.Remark && x.Scenario.Id == item.ScenarioId && x.KeyOperationConfig.Id == item.KeyOperationConfigId);
            //        //skip no change value
            //        if (exist != null)
            //        {
            //            continue;
            //        }
            //        var attachedEntity = DataContext.KeyOperationDatas.FirstOrDefault(x => x.Kpi.Id == item.KpiId && x.PeriodeType == item.PeriodeType && x.Periode == item.Periode && x.Scenario.Id == item.ScenarioId && x.KeyOperationConfig.Id == item.KeyOperationConfigId);
            //        if (attachedEntity != null)
            //        {
            //            operationData.Id = attachedEntity.Id;
            //        }
            //        //jika tidak ada perubahan di skip aja
            //        //if (existing.Value.Equals(item.Value) && existing.Periode.Equals(item.Periode) && existing.Kpi.Id.Equals(item.KpiId) && existing.PeriodeType.Equals(item.PeriodeType)) {
            //        //    break;
            //        //}
            //        if (operationData.Id != 0)
            //        {
            //            //var attachedEntity = DataContext.KpiAchievements.Find(item.Id);
            //            if (attachedEntity != null && DataContext.Entry(attachedEntity).State != EntityState.Detached)
            //            {
            //                DataContext.Entry(attachedEntity).State = EntityState.Detached;
            //            }
            //            DataContext.KeyOperationDatas.Attach(operationData);
            //            DataContext.Entry(operationData).State = EntityState.Modified;
            //        }
            //        else
            //        {
            //            operationData.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == item.KpiId);
            //            DataContext.KeyOperationDatas.Add(operationData);
            //        }
            //        i++;
            //    }
            //    DataContext.SaveChanges();
            //    response.IsSuccess = true;
            //    if (i > 0)
            //    {
            //        response.Message = string.Format("{0}  Operation Data items has been updated successfully", i.ToString());
            //    }
            //    else
            //    {
            //        response.Message = "File Successfully Parsed, but no data changed!";
            //    }


            //}
            //catch (InvalidOperationException invalidOperationException)
            //{
            //    response.Message = invalidOperationException.Message;
            //}
            //catch (ArgumentNullException argumentNullException)
            //{
            //    response.Message = argumentNullException.Message;
            //}
            //return response;
            var response = new BaseResponse();
            try
            {
                int deletedCounter = 0;
                int updatedCounter = 0;
                int addedCounter = 0;
                int skippedCounter = 0;
                foreach (var item in request.BatchUpdateOperationDataItemRequest)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        var existedOperationDatum = DataContext.KeyOperationDatas
                            .Include(x => x.KeyOperationConfig)
                            .Include(x => x.Scenario)
                            .FirstOrDefault(x =>x.Kpi.Id == item.KpiId 
                            && x.PeriodeType == item.PeriodeType && x.Periode == item.Periode && x.Scenario.Id == item.ScenarioId);


                        if (existedOperationDatum != null)
                        {
                            if (item.Value.Equals("-") || item.Value.ToLowerInvariant().Equals("null"))
                            {
                                DataContext.KeyOperationDatas.Remove(existedOperationDatum);
                                deletedCounter++;
                            }
                            else
                            {
                                string oldValue = !existedOperationDatum.Value.HasValue ? string.Empty : existedOperationDatum.Value.Value.ToString(CultureInfo.InvariantCulture);
                                string newValue = !item.RealValue.HasValue ? string.Empty : item.RealValue.Value.ToString(CultureInfo.InvariantCulture);

                                if (oldValue.Equals(newValue))
                                {
                                    skippedCounter++;
                                }
                                else
                                {
                                    existedOperationDatum.Value = item.RealValue;
                                    DataContext.Entry(existedOperationDatum).State = EntityState.Modified;
                                    updatedCounter++;    
                                }
                            }
                        }
                        else
                        {
                            var operationDatum = item.MapTo<KeyOperationData>();
                            if (operationDatum.Value.HasValue)
                            {
                                operationDatum.Kpi = DataContext.Kpis.Single(x => x.Id == item.KpiId);
                                operationDatum.KeyOperationConfig = DataContext.KeyOperationConfigs.Single(x => x.Id == item.KeyOperationConfigId);
                                operationDatum.Scenario = DataContext.Scenarios.Single(x => x.Id == item.ScenarioId);
                                DataContext.KeyOperationDatas.Add(operationDatum);
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


        public GetOperationDataConfigurationResponse GetOperationDataConfigurationForAllGroup(GetOperationDataConfigurationRequest request)
        {
            var response = new GetOperationDataConfigurationResponse();
            response.RoleGroupId = request.RoleGroupId;
            response.ScenarioId = request.ScenarioId;
           
            var periodeType = request.PeriodeType;
            
            List<KeyOperationConfig> keyOperationConfigs = DataContext.KeyOperationConfigs
            .Include(x => x.Kpi)
            .Include(x => x.Kpi.Measurement)
            .Include(x => x.KeyOperationGroup)
            .Where(x => x.IsActive && x.KeyOperationGroup != null).ToList();
            
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
                        kpiDto.GroupName = keyOperationConfig.KeyOperationGroup.Name;
                        foreach (var number in YearlyNumbersForOperationData)
                        {
                            var operation = operationDataYearly.FirstOrDefault(x => x.Kpi != null && x.Kpi.Id == keyOperationConfig.Kpi.Id && x.Periode.Year == number);

                            if (operation != null)
                            {
                                var operationtDataDto =
                                    operation.MapTo<GetOperationDataConfigurationResponse.OperationData>();
                                operationtDataDto.ScenarioId = request.ScenarioId;
                                operationtDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                kpiDto.OperationData.Add(operationtDataDto);
                            }
                            else
                            {
                                var operationtDataDto = new GetOperationDataConfigurationResponse.OperationData();
                                operationtDataDto.Periode = new DateTime(number, 1, 1);
                                operationtDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                kpiDto.OperationData.Add(operationtDataDto);
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
                                    .Where(x => x.PeriodeType == PeriodeType.Monthly && x.Periode.Year == request.Year && x.Scenario.Id == request.ScenarioId).ToList();

                    foreach (var keyOperationConfig in keyOperationConfigs)
                    {
                        var kpiDto = keyOperationConfig.Kpi.MapTo<GetOperationDataConfigurationResponse.Kpi>();
                        kpiDto.GroupName = keyOperationConfig.KeyOperationGroup.Name;
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
                                kpiDto.OperationData.Add(operationDataDto);
                            }
                            else
                            {
                                var operationDataDto = new GetOperationDataConfigurationResponse.OperationData();
                                operationDataDto.Periode = new DateTime(request.Year, i, 1);
                                operationDataDto.ScenarioId = request.ScenarioId;
                                operationDataDto.KeyOperationConfigId = keyOperationConfig.Id;
                                kpiDto.OperationData.Add(operationDataDto);
                            }
                        }
                        response.Kpis.Add(kpiDto);
                    }
                    break;

            }

            response.IsSuccess = true;


            return response;
        }
    }
}
