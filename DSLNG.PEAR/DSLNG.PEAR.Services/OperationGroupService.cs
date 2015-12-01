using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OperationGroup;
using DSLNG.PEAR.Services.Responses.OperationGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class OperationGroupService : BaseService, IOperationGroupService 
    {
        public OperationGroupService(IDataContext context) : base(context) { }

        public GetOperationGroupsResponse GetOperationGroups(GetOperationGroupsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetOperationGroupsResponse
            {
                TotalRecords = totalRecords,
                OperationGroups = data.ToList().MapTo<GetOperationGroupsResponse.OperationGroup>()
            }; 
        }


        public SaveOperationGroupResponse SaveOperationGroup(SaveOperationGroupRequest request)
        {
            if (request.Id == 0)
            {
                var operationGroup = request.MapTo<KeyOperationGroup>();
                DataContext.KeyOperationGroups.Add(operationGroup);
            }
            else
            {
                var operationGroup = DataContext.KeyOperationGroups.FirstOrDefault(x => x.Id == request.Id);
                if (operationGroup != null)
                {
                    request.MapPropertiesToInstance<KeyOperationGroup>(operationGroup);
                }
            }
            DataContext.SaveChanges();
            return new SaveOperationGroupResponse
            {
                IsSuccess = true,
                Message = "Operation Group has been saved"
            };
        }


        public GetOperationGroupResponse GetOperationGroup(GetOperationGroupRequest request)
        {
            return DataContext.KeyOperationGroups.FirstOrDefault(x => x.Id == request.Id).MapTo<GetOperationGroupResponse>();
        }


        public DeleteOperationGroupResponse DeleteOperationGroup(DeleteOperationGroupRequest request)
        {
            try
            {
                var OperationGroup = new KeyOperationGroup { Id = request.Id };
                DataContext.KeyOperationGroups.Attach(OperationGroup);
                DataContext.KeyOperationGroups.Remove(OperationGroup);
                DataContext.SaveChanges();

                return new DeleteOperationGroupResponse
                {
                    IsSuccess = true,
                    Message = "The Operation Group has been deleted successfully"
                };
            }
            catch(DbUpdateException exception)
            {
                return new DeleteOperationGroupResponse
                {
                    IsSuccess = false,
                    Message = exception.Message
                };
            }
        }



        public IEnumerable<KeyOperationGroup> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KeyOperationGroups.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search));
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
    }
}
