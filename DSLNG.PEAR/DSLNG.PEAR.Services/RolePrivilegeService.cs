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
            throw new NotImplementedException();
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
