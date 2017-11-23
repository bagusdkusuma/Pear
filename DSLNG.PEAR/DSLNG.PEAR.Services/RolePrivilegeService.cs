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
using System.Data.Entity.Infrastructure;

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
                    response.IsSuccess = true;
                    response = data.MapTo<GetPrivilegeResponse>();
                    response.Department = data.RoleGroup.MapTo<GetPrivilegeResponse.RoleGroup>();
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

        /// <summary>
        /// Save Or Update Role Privileges
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SaveRolePrivilegeResponse SaveRolePrivilege(SaveRolePrivilegeRequest request)
        {
            var response = new SaveRolePrivilegeResponse();
            var baseRequest = request.MapTo<BaseAction>();
            try
            {
                var privilege = request.MapTo<RolePrivilege>();
                var user = DataContext.Users.Find(request.UserId);
                if (request.Id > 0)
                {
                    //Update Mode
                    privilege.UpdatedBy = user;
                    privilege.UpdatedDate = DateTime.Now;
                    DataContext.Entry(privilege).State = EntityState.Modified;
                }
                else
                {
                    //Insert mode
                    privilege.CreatedBy = user;
                    privilege.CreatedDate = DateTime.Now;
                    DataContext.RolePrivileges.Add(privilege);
                }
                DataContext.SaveChanges();
                //DataContext.SaveChanges(baseRequest);
                //try to batch update
                if (request.MenuRolePrivileges.Count > 0)
                {
                    foreach (var menuRole in request.MenuRolePrivileges)
                    {
                        menuRole.RolePrivilege_Id = privilege.Id;
                        
                        var existing = DataContext.MenuRolePrivileges.FirstOrDefault(x => x.Menu_Id == menuRole.Menu_Id && x.RolePrivilege_Id == privilege.Id);
                        if (existing != null)
                        {
                            existing.AllowApprove = menuRole.AllowApprove;
                            existing.AllowCreate = menuRole.AllowCreate;
                            existing.AllowDelete = menuRole.AllowDelete;
                            existing.AllowDownload = menuRole.AllowDownload;
                            existing.AllowPublish = menuRole.AllowPublish;
                            existing.AllowUpdate = menuRole.AllowUpdate;
                            existing.AllowUpload = menuRole.AllowUpload;
                            existing.AllowView = menuRole.AllowView;
                            existing.AllowInput = menuRole.AllowInput;
                            DataContext.Entry(existing).State = EntityState.Modified;
                        }
                        else
                        {
                            var menuPrivilege = menuRole.MapTo<MenuRolePrivilege>();
                            DataContext.MenuRolePrivileges.Add(menuPrivilege);
                        }
                    }
                    //DataContext.SaveChanges(baseRequest);
                    DataContext.SaveChanges();
                }
                response.IsSuccess = true;
                response.Id = privilege.Id;
                response.Message = "Privilege Successfully Saved";
            }
            catch (DbUpdateException upd)
            {
                response.IsSuccess = false;
                response.Message = upd.Message;
            }
            catch (InvalidOperationException inv)
            {
                response.IsSuccess = false;
                response.Message = inv.Message;
            }
            
            return response;
        }

        /// <summary>
        /// Get Menu Role Privilege
        /// </summary>
        /// <param name="request:{RoleId}"></param>
        /// <param name="request:{RolePrivilegeId}"></param>
        /// <returns>List Of Menu Role Privileges with TotalRecords and Query Transaction Status</returns>
        public GetMenuRolePrivilegeResponse GetMenuRolePrivileges(GetPrivilegeByRolePrivilegeRequest request)
        {
            var response = new GetMenuRolePrivilegeResponse();
            try
            {
                if(request.RoleId == 0 && request.RolePrivilegeId == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "No Role & Privilege Defined for this query";
                    response.TotalRecords = 0;
                    return response;
                }
                else if(request.RoleId == 0 && request.RolePrivilegeId > 0)
                {
                    var rolePrivilege = DataContext.RolePrivileges.SingleOrDefault(x => x.Id == request.RolePrivilegeId);
                    if (rolePrivilege != null)
                    {
                        request.RoleId = rolePrivilege.RoleGroup_Id;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "Could not found Role for reference";
                        response.TotalRecords = 0;
                        return response;
                    }
                }
                var data = new List<MenuRolePrivilege>();
                #region get role menus
                var role = DataContext.RoleGroups.Include(x => x.Menus).Include(y => y.RolePrivileges).SingleOrDefault(x => x.Id == request.RoleId);
                if (role != null)
                {
                    //cek menus
                    if (role.Menus.Count > 0)
                    {
                        foreach (var menu in role.Menus)
                        {
                            #region jika request.RolePrivilegeId == 0 // create mode
                            if (request.RolePrivilegeId == 0)
                            {
                                var menuRolePrivilege = new MenuRolePrivilege
                                {
                                    Menu = menu,
                                    Menu_Id = menu.Id,
                                    RolePrivilege = new RolePrivilege
                                    {
                                        RoleGroup = role,
                                        RoleGroup_Id = role.Id
                                    },
                                    RolePrivilege_Id = 0,
                                    AllowApprove = false,
                                    AllowPublish = false,
                                    AllowCreate = false,
                                    AllowDelete = false,
                                    AllowDownload = false,
                                    AllowUpdate = false,
                                    AllowView = false,
                                    AllowUpload = false,
                                    AllowInput = false
                                };
                                data.Add(menuRolePrivilege);
                            }
                            #endregion
                            #region jika request.RolePrivilegeId > 0 // edit mode
                            if (request.RolePrivilegeId > 0)
                            {
                                var menuRolePrivilege = DataContext.MenuRolePrivileges.Include(x => x.RolePrivilege).Include(y => y.Menu).SingleOrDefault(z => z.Menu_Id == menu.Id && z.RolePrivilege_Id == request.RolePrivilegeId);
                                if (menuRolePrivilege == null)
                                {
                                    menuRolePrivilege = new MenuRolePrivilege
                                    {
                                        Menu = menu,
                                        Menu_Id = menu.Id,
                                        RolePrivilege = DataContext.RolePrivileges.Single(x => x.Id == request.RolePrivilegeId),
                                        RolePrivilege_Id = request.RolePrivilegeId,
                                        AllowApprove = false,
                                        AllowPublish = false,
                                        AllowCreate = false,
                                        AllowDelete = false,
                                        AllowDownload = false,
                                        AllowUpdate = false,
                                        AllowView = false,
                                        AllowUpload = false,
                                        AllowInput = false
                                    };
                                }
                                data.Add(menuRolePrivilege);
                            }
                            #endregion
                        }
                    }

                }else
                {
                    response.Message = "This Privilege Has No Menu Assigned";
                }
                #endregion
                if (data.Count > 0)
                {
                    response.MenuRolePrivileges = data.ToList().MapTo<GetMenuRolePrivilegeResponse.MenuRolePrivilege>();
                    response.IsSuccess = response.MenuRolePrivileges != null;
                    response.TotalRecords = response.MenuRolePrivileges != null ? response.MenuRolePrivileges.Count() : 0;
                }
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }

        public BaseResponse DeleteRolePrivilege(DeleteRolePrivilegeRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var privilege = new RolePrivilege { Id = request.Id };
                DataContext.RolePrivileges.Attach(privilege);
                DataContext.Entry(privilege).State = EntityState.Deleted;
                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "Role Privilege Deleted Successfully";
            }
            catch (DbUpdateException upd)
            {
                response.IsSuccess = false;
                response.Message = upd.Message;
            }
            return response;
        }
    }
}
