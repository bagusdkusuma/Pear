using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.Privilege;
using DSLNG.PEAR.Services.Responses.Privilege;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Data.Entities;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class RolePrivilegeService : BaseService, IRolePrivilegeService
    {
        public RolePrivilegeService(IDataContext contex):base(contex)
        {

        }

        public GetPrivilegeResponse GetRolePrivilege(GetPrivilegeRequest request)
        {
            var response = new GetPrivilegeResponse();
            try
            {
                var data = DataContext.RolePrivileges.Include(x => x.RoleGroup).FirstOrDefault(y => y.Id == request.Id);
                if (data == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Data Not Exist";
                }
                else
                {
                    response = data.MapTo<GetPrivilegeResponse>();
                }
            }
            catch (InvalidOperationException e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            
            return response;
        }

        public GetPrivilegesResponse GetRolePrivileges(GetPrivilegesRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            if (request.RoleId > 0)
            {
                data = data.Where(x => x.RoleGroup_Id == request.RoleId);
            }
            return new GetPrivilegesResponse
            {
                TotalRecords = totalRecords,
                Privileges = data.ToList().MapTo<GetPrivilegesResponse.RolePrivilege>()
            };
        }

        public IEnumerable<RolePrivilege> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.RolePrivileges.Include(x=>x.RoleGroup).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.Descriptions.Contains(search));
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
                    case "RoleGroup":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.RoleGroup.Name)
                            : data.OrderByDescending(x => x.RoleGroup.Name);
                        break;
                }
            }
            TotalRecords = data.Count();
            return data;
        }

        public GetPrivilegesResponse GetRolePrivileges(GetPrivilegeByRoleRequest request)
        {
            var response = new GetPrivilegesResponse();
            response.Privileges = DataContext.RolePrivileges.Where(x => x.RoleGroup_Id == request.RoleId).MapTo<GetPrivilegesResponse.RolePrivilege>();
            response.TotalRecords = response.Privileges.Count();
            return response;
        }

        public BaseResponse SaveRolePrivilege(SaveRolePrivilegeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
