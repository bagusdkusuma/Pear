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

namespace DSLNG.PEAR.Services
{
    public class OperationGroupService : BaseService, IOperationGroupService 
    {
        public OperationGroupService(IDataContext context) : base(context) { }

        public GetOperationGroupsResponse GetOperationGroups(GetOperationGroupsRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetOperationGroupsResponse { Count = DataContext.KeyOperationGroups.Count()};
            }
            else
            {
                return new GetOperationGroupsResponse
                {
                    OperationGroups = DataContext.KeyOperationGroups.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take).ToList().MapTo<GetOperationGroupsResponse.OperationGroup>()
                };
            }
        }


        public SaveOperationGroupResponse SaveOperationGroup(SaveOperationGroupRequest request)
        {
            if (request.Id == 0)
            {
                var OperationGroup = request.MapTo<KeyOperationGroup>();
                DataContext.KeyOperationGroups.Add(OperationGroup);
            }
            else
            {
                var OperationGroup = DataContext.KeyOperationGroups.FirstOrDefault(x => x.Id == request.Id);
                if (OperationGroup != null)
                {
                    request.MapPropertiesToInstance<KeyOperationGroup>(OperationGroup);
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
    }
}
