using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Operation;
using DSLNG.PEAR.Services.Responses.Operation;
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
    public class OperationConfigService : BaseService, IOperationConfigService
    {
        public OperationConfigService(IDataContext context)
            : base(context)
        {
        }


        public GetOperationsResponse GetOperations(GetOperationsRequest request)
        {
            int totalRecords;
            var operationConfigs = DataContext.KeyOperationConfigs
                .Include(x => x.KeyOperationGroup)
                .Include(x => x.Kpi)
                .Include(x => x.Kpi.Measurement)
                .ToList().Select(x => new KeyOperationConfig()
                    {
                        Desc = x.Desc,
                        Id = x.Id,
                        IsActive = x.IsActive,
                        KeyOperationGroup = x.KeyOperationGroup ?? new KeyOperationGroup(),
                        Kpi = x.Kpi ?? new Kpi(),
                        Order = x.Order
                    }).ToList();
            var result = AddEconomicKpi(operationConfigs);
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords, result);
            

            return new GetOperationsResponse
                {
                    TotalRecords = totalRecords,
                    Operations = data.MapTo<GetOperationsResponse.Operation>()
                };
            //if (request.OnlyCount)
            //{
            //    return new GetOperationsResponse { Count = DataContext.KeyOperationConfigs.Count() };
            //}
            //else
            //{
            //    return new GetOperationsResponse
            //    {
            //        Operations = DataContext.KeyOperationConfigs.OrderByDescending(x => x.Id)
            //        .Include(x => x.KeyOperationGroup).Skip(request.Skip).Take(request.Take).ToList().MapTo<GetOperationsResponse.Operation>()
            //    };
            //}
        }


        public OperationGroupsResponse GetOperationGroups()
        {
            return new OperationGroupsResponse
                {
                    OperationGroups =
                        DataContext.KeyOperationGroups.ToList().MapTo<OperationGroupsResponse.OperationGroup>(),
                    Kpis = DataContext.Kpis.ToList().MapTo<OperationGroupsResponse.Kpi>()
                };
        }


        public SaveOperationResponse SaveOperation(SaveOperationRequest request)
        {
            if (request.Id == 0)
            {
                var operation = request.MapTo<KeyOperationConfig>();
                operation.KeyOperationGroup =
                    DataContext.KeyOperationGroups.FirstOrDefault(x => x.Id == request.KeyOperationGroupId);
                operation.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.KpiId);
                DataContext.KeyOperationConfigs.Add(operation);
            }
            else
            {
                var operation = DataContext.KeyOperationConfigs.FirstOrDefault(x => x.Id == request.Id);
                if (operation != null)
                {
                    request.MapPropertiesToInstance<KeyOperationConfig>(operation);
                    operation.KeyOperationGroup =
                        DataContext.KeyOperationGroups.FirstOrDefault(x => x.Id == request.KeyOperationGroupId);
                    operation.Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.KpiId);
                }
            }
            DataContext.SaveChanges();
            return new SaveOperationResponse
            {
                IsSuccess = true,
                Message = "Operation has been Saved"
            };

        }


        public GetOperationResponse GetOperation(GetOperationRequest request)
        {
            return DataContext.KeyOperationConfigs.Where(x => x.Id == request.Id)
                              .Include(x => x.Kpi)
                              .Include(x => x.KeyOperationGroup)
                              .FirstOrDefault()
                              .MapTo<GetOperationResponse>();
        }


        public DeleteOperationResponse DeleteOperation(DeleteOperationRequest request)
        {
            var checkId = DataContext.KeyOperationConfigs.FirstOrDefault(x => x.Id == request.Id);
            if (checkId != null)
            {
                DataContext.KeyOperationConfigs.Attach(checkId);
                DataContext.KeyOperationConfigs.Remove(checkId);
                DataContext.SaveChanges();
            }
            return new DeleteOperationResponse
                {
                    IsSuccess = true,
                    Message = "Operation has been deleted successfully"
                };
        }

        public UpdateOperationResponse UpdateOperation(UpdateOperationRequest request)
        {
            bool isKpiExisted =
                DataContext.KeyOperationConfigs.Include(x => x.Kpi).FirstOrDefault(x => x.Kpi.Id == request.KpiId) != null;
            if (request.Id == 0 && !isKpiExisted)
            {
                var operationConfig = new KeyOperationConfig();
                operationConfig.IsActive = request.IsActive.HasValue && request.IsActive.Value;
                operationConfig.Order = request.Order.HasValue ? request.Order.Value : 0;
                operationConfig.Kpi = DataContext.Kpis.Single(x => x.Id == request.KpiId);
                if (request.KeyOperationGroupId > 0)
                {
                    operationConfig.KeyOperationGroup =
                        DataContext.KeyOperationGroups.Single(x => x.Id == request.KeyOperationGroupId);    
                }
                
                DataContext.KeyOperationConfigs.Add(operationConfig);
                DataContext.SaveChanges();
                return new UpdateOperationResponse
                {
                    IsSuccess = true,
                    Message = "Operation Config has been saved succesfully",
                    Id = operationConfig.Id
                };
            }
            else 
            {
                var operationConfig = DataContext.KeyOperationConfigs
                    .Include(x => x.KeyOperationGroup)
                    .Single(x => x.Id == request.Id);
                if (request.IsActive.HasValue)
                {
                    operationConfig.IsActive = request.IsActive.Value;
                }

                if (request.Order.HasValue)
                {
                    operationConfig.Order = request.Order.Value;
                }

                if (request.KeyOperationGroupId != 0)
                {
                    var group = new KeyOperationGroup { Id = request.KeyOperationGroupId };
                    DataContext.KeyOperationGroups.Attach(group);
                    operationConfig.KeyOperationGroup = group;
                }

                DataContext.SaveChanges();
                return new UpdateOperationResponse
                {
                    IsSuccess = true,
                    Message = "Operation Config has been saved succesfully",
                    Id = operationConfig.Id
                };
            }
            
        }


        public IEnumerable<KeyOperationConfig> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int totalRecords, IEnumerable<KeyOperationConfig> operationConfigs)
        {
            IEnumerable<KeyOperationConfig> data = operationConfigs;
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                //data = data.Where(x => x.KeyOperationGroup != null ? x.KeyOperationGroup.Name.Contains(search) : x.KeyOperationGroup != null || x.Kpi.Name.Contains(search));
                data = data.Where(x => x.Kpi.Name.ToLower().Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Key Operation Group":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.KeyOperationGroup.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.KeyOperationGroup.Name).ThenBy(x => x.Order);
                        break;
                    case "Kpi":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Kpi.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Kpi.Name).ThenBy(x => x.Order);
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

            totalRecords = data.Count();
            return data;
        }



        private IEnumerable<KeyOperationConfig> AddEconomicKpi(List<KeyOperationConfig> operationConfigs)
        {
            IList<int> kpiIds = operationConfigs.Select(x => x.Kpi.Id).ToList();
            var kpis = DataContext.Kpis
                .Include(x => x.Measurement)
                .Where(x => x.IsEconomic && !kpiIds.Contains(x.Id)).ToList();
            foreach (var kpi in kpis)
            {
                operationConfigs.Add(new KeyOperationConfig
                {
                    Id = 0,
                    IsActive = false,
                    Order = 0,
                    Desc = string.Empty,
                    Kpi = new Kpi { Name = kpi.Name, Id = kpi.Id, Measurement = new Measurement { Name = kpi.Measurement.Name } },
                    KeyOperationGroup = new KeyOperationGroup { Id = 0, Name = string.Empty }
                });
            }
            return operationConfigs;
        }


        public GetOperationsInResponse GetOperationIn(GetOperationsInRequest request)
        {
            return new GetOperationsInResponse
            {
                KeyOperations = DataContext.KeyOperationConfigs.Include(x => x.Kpi).Where(x => request.KpiIds.Contains(x.Kpi.Id)).Select(
                    x => new GetOperationsInResponse.KeyOperationResponse { 
                        Id = x.Id,
                        KpiId = x.Kpi.Id
                    }
                ).ToList()
            };
        }
    }
}
