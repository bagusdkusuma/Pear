using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.RoleGroup;
using DSLNG.PEAR.Services.Responses.RoleGroup;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace DSLNG.PEAR.Services
{
    public class RoleGroupService : BaseService, IRoleGroupService
    {
        public RoleGroupService(IDataContext DataContext) : base(DataContext)
        {

        }

        public GetRoleGroupsResponse GetRoleGroups(GetRoleGroupsRequest request)
        {

            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetRoleGroupsResponse
            {
                TotalRecords = totalRecords,
                RoleGroups = data.ToList().MapTo<GetRoleGroupsResponse.RoleGroup>()
            };

            //var roleGroups = new List<RoleGroup>();
            //if (request.Take != 0)
            //{
            //    roleGroups = DataContext.RoleGroups.Include(x => x.Level).OrderBy(x => x.Id).Skip(request.Skip).Take(request.Take).ToList();
            //}
            //else
            //{
            //    roleGroups = DataContext.RoleGroups.Include(x => x.Level).ToList();
            //}
            //var response = new GetRoleGroupsResponse();
            //response.RoleGroups = roleGroups.MapTo<GetRoleGroupsResponse.RoleGroup>();
            //return response;
        }

        public GetRoleGroupResponse GetRoleGroup(GetRoleGroupRequest request)
        {
            try
            {
                var roleGroup = DataContext.RoleGroups.Include(x => x.Level).Include(y => y.RolePrivileges).FirstOrDefault(x => x.Id == request.Id);
                var response = roleGroup.MapTo<GetRoleGroupResponse>();
                response.Privileges = roleGroup.RolePrivileges.MapTo<GetRoleGroupResponse.RolePrivilege>();
                response.IsSuccess = true;
                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetRoleGroupResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        public CreateRoleGroupResponse Create(CreateRoleGroupRequest request)
        {
            var response = new CreateRoleGroupResponse();
            var action = request.MapTo<BaseAction>();
            try
            {
                var roleGroup = request.MapTo<RoleGroup>();
                roleGroup.Level = DataContext.Levels.FirstOrDefault(x => x.Id == request.LevelId);
                DataContext.RoleGroups.Add(roleGroup);
                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "RoleGroup type item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public UpdateRoleGroupResponse Update(UpdateRoleGroupRequest request)
        {
            var response = new UpdateRoleGroupResponse();
            try
            {
                var roleGroup = request.MapTo<RoleGroup>();
                roleGroup.Level = DataContext.Levels.FirstOrDefault(x => x.Id == request.LevelId);
                DataContext.RoleGroups.Attach(roleGroup);
                DataContext.Entry(roleGroup).State = EntityState.Modified;
                DataContext.SaveChanges();

                response.IsSuccess = true;
                response.Message = "User RoleGroup item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public DeleteRoleGroupResponse Delete(int id)
        {
            var response = new DeleteRoleGroupResponse();
            try
            {
                var roleGroup = new Data.Entities.RoleGroup { Id = id };
                DataContext.RoleGroups.Attach(roleGroup);
                DataContext.Entry(roleGroup).State = EntityState.Deleted;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "User RoleGroup item has been deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public IEnumerable<RoleGroup> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.RoleGroups.Include(x => x.Level).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.Level.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name)
                            : data.OrderByDescending(x => x.Name);
                        break;
                    case "Level":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Level.Name)
                            : data.OrderByDescending(x => x.Level.Name);
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

        public GetRoleGroupsResponse All()
        {
            var data = DataContext.RoleGroups.MapTo<GetRoleGroupsResponse.RoleGroup>();
            return new GetRoleGroupsResponse
            {
                IsSuccess = true,
                RoleGroups = data,
                TotalRecords = data.Count
            };
        }
    }
}
