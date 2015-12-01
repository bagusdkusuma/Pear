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
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetOperationsResponse
                {
                    TotalRecords = totalRecords,
                    Operations = data.ToList().MapTo<GetOperationsResponse.Operation>()
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
                    Message = "Operation has been Save"
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
            var operationConfig = DataContext.KeyOperationConfigs.Single(x => x.Id == request.Id);
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
                var group = new KeyOperationGroup {Id = request.KeyOperationGroupId};
                DataContext.KeyOperationGroups.Attach(group);
                operationConfig.KeyOperationGroup = group;
            }

            DataContext.SaveChanges();
            return new UpdateOperationResponse
            {
                IsSuccess = true,
                Message = "Operation Config has been saved succesfully"
            };
        }


        public IEnumerable<KeyOperationConfig> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KeyOperationConfigs.Include(x => x.KeyOperationGroup).Include(x => x.Kpi).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.KeyOperationGroup.Name.Contains(search) || x.Kpi.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "OperationGroup":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.KeyOperationGroup.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.KeyOperationGroup.Name).ThenBy(x => x.Order);
                        break;
                    case "KPI":
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
                            ? data.OrderBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.IsActive);
                        break;
                }
            }


            TotalRecords = data.Count();
            return data;
        }
    }
}
