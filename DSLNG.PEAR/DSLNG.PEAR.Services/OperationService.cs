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
    public class OperationService : BaseService, IOperationService
    {
        public OperationService(IDataContext context) : base(context) { }


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
            //    return new GetOperationsResponse { Count = DataContext.KeyOperations.Count() };
            //}
            //else
            //{
            //    return new GetOperationsResponse
            //    {
            //        Operations = DataContext.KeyOperations.OrderByDescending(x => x.Id)
            //        .Include(x => x.KeyOperationGroup).Skip(request.Skip).Take(request.Take).ToList().MapTo<GetOperationsResponse.Operation>()
            //    };
            //}
        }


        public OperationGroupsResponse GetOperationGroups()
        {
            return new OperationGroupsResponse
            {
                OperationGroups = DataContext.KeyOperationGroups.ToList().MapTo<OperationGroupsResponse.OperationGroup>(),
                KPIS = DataContext.Kpis.ToList().MapTo<OperationGroupsResponse.KPI>()
            };
        }


        public SaveOperationResponse SaveOperation(SaveOperationRequest request)
        {
            if (request.Id == 0)
            {
                var Operation = request.MapTo<KeyOperationConfig>();
                Operation.KeyOperationGroup = DataContext.KeyOperationGroups.Where(x => x.Id == request.IdKeyOperationGroup).FirstOrDefault();
                Operation.Kpi = DataContext.Kpis.Where(x => x.Id == request.IdKPI).FirstOrDefault();
                DataContext.KeyOperations.Add(Operation);
            }
            else
            {
                var Operation = DataContext.KeyOperations.Where(x => x.Id == request.Id).FirstOrDefault();
                if (Operation != null)
                {
                    request.MapPropertiesToInstance<KeyOperationConfig>(Operation);
                    Operation.KeyOperationGroup = DataContext.KeyOperationGroups.Where(x => x.Id == request.IdKeyOperationGroup).FirstOrDefault();
                    Operation.Kpi = DataContext.Kpis.Where(x => x.Id == request.IdKPI).FirstOrDefault();
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
            return DataContext.KeyOperations.Where(x => x.Id == request.Id)
                .Include(x => x.Kpi).Include(x => x.KeyOperationGroup).FirstOrDefault().MapTo<GetOperationResponse>();
        }


        public DeleteOperationResponse DeleteOperation(DeleteOperationRequest request)
        {
            var checkId = DataContext.KeyOperations.Where(x => x.Id == request.Id).FirstOrDefault();
            if (checkId != null)
            {
                DataContext.KeyOperations.Attach(checkId);
                DataContext.KeyOperations.Remove(checkId);
                DataContext.SaveChanges();
            }
            return new DeleteOperationResponse
            {
                IsSuccess = true,
                Message = "Operation has been deleted successfully"
            };
        }


        public IEnumerable<KeyOperationConfig> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KeyOperations.Include(x => x.KeyOperationGroup).Include(x => x.Kpi).AsQueryable();
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
