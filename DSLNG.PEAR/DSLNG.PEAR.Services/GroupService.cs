using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Group;
using DSLNG.PEAR.Services.Responses.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class GroupService : BaseService, IGroupService
    {
        public GroupService(IDataContext dataContext)
            : base(dataContext)
        {

        }

        public GetGroupResponse GetGroup(GetGroupRequest request)
        {
            try
            {
                var group = DataContext.Groups.First(x => x.Id == request.Id);
                var response = group.MapTo<GetGroupResponse>();

                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetGroupResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }


        public GetGroupsResponse GetGroups(GetGroupsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetGroupsResponse
            {
                TotalRecords = totalRecords,
                Groups = data.ToList().MapTo<GetGroupsResponse.Group>()
            };
            //var response = new GetGroupsResponse();
            //var groups = DataContext.Groups.ToList();
            //response.Groups = groups.MapTo<GetGroupsResponse.Group>();
            //return response;
        }

        public CreateGroupResponse Create(CreateGroupRequest request)
        {
            var response = new CreateGroupResponse();
            try
            {
                var group = request.MapTo<Group>();
                DataContext.Groups.Add(group);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Group item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.IsSuccess = false;
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public UpdateGroupResponse Update(UpdateGroupRequest request)
        {
            var response = new UpdateGroupResponse();
            try
            {
                var group = request.MapTo<Group>();
                DataContext.Groups.Attach(group);
                DataContext.Entry(group).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Group item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.IsSuccess = false;
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public DeleteGroupResponse Delete(int id)
        {
            var response = new DeleteGroupResponse();
            try
            {
                var group = new Group { Id = id };
                DataContext.Groups.Attach(group);
                DataContext.Entry(group).State = EntityState.Deleted;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Group item has been Deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.IsSuccess = false;
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public IEnumerable<Group> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Groups.AsQueryable();
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
