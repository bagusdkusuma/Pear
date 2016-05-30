using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.ProcessBlueprint;
using DSLNG.PEAR.Services.Responses.ProcessBlueprint;
using DSLNG.PEAR.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DSLNG.PEAR.Data.Entities;
using System.Data.Entity.Infrastructure;
using DSLNG.PEAR.Services.Responses;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class ProcessBlueprintService : BaseService, IProcessBlueprintService
    {
        public ProcessBlueprintService(IDataContext dataContext)
            : base(dataContext)
        {

        }
        public GetProcessBlueprintsResponse Gets(GetProcessBlueprintsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetProcessBlueprintsResponse
            {
                TotalRecords = totalRecords,
                ProcessBlueprints = data.ToList().MapTo<GetProcessBlueprintsResponse.ProcessBlueprint>()
            };
        }

        private IEnumerable<ProcessBlueprint> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.ProcessBlueprints.Include(x => x.CreatedBy).AsQueryable();
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
                            ? data.OrderBy(x => x.Name).ThenBy(x => x.ParentId).ThenBy(x => x.LastWriteTime)
                            : data.OrderByDescending(x => x.Name).ThenBy(x => x.LastWriteTime);
                        break;
                    default:
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Id).ThenBy(x => x.ParentId).ThenBy(x => x.LastWriteTime)
                            : data.OrderBy(x => x.ParentId).ThenBy(x => x.Id).ThenBy(x => x.LastWriteTime);
                        break;
                }
            }
            TotalRecords = data.Count();
            return data;

        }

        public GetProcessBlueprintResponse Get(GetProcessBlueprintRequest request)
        {
            var data = DataContext.ProcessBlueprints.Include(x => x.CreatedBy).FirstOrDefault(x => x.Id == request.Id);
            return data.MapTo<GetProcessBlueprintResponse>();
        }

        public SaveProcessBlueprintResponse Save(SaveProcessBlueprintRequest request)
        {
            var response = new SaveProcessBlueprintResponse();
            try
            {
                var proses = new ProcessBlueprint();
                var user = DataContext.Users.Single(x => x.Id == request.UserId);
                if (request.Id > 0)
                {
                    proses = DataContext.ProcessBlueprints.Single(x => x.Id == request.Id);
                    proses.Name = request.Name;
                    proses.ParentId = request.ParentId;
                    proses.IsFolder = request.IsFolder;
                    proses.LastWriteTime = request.LastWriteTime;
                    proses.Data = request.Data;
                    proses.UpdatedBy = user;
                    DataContext.Entry(proses).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    proses.Name = request.Name;
                    proses.IsFolder = request.IsFolder;
                    proses.ParentId = request.ParentId;
                    proses.Data = request.Data;
                    proses.LastWriteTime = request.LastWriteTime;
                    proses.CreatedBy = user;
                    DataContext.ProcessBlueprints.Add(proses);
                }

                DataContext.SaveChanges();
                DataContext.Entry(proses).GetDatabaseValues();
                response.Id = proses.Id;
                response.IsSuccess = true;
                response.Message = "File Manager Successfully Saved";
            }
            catch (DbUpdateException ex)
            {

                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }


        public BaseResponse Delete(int Id)
        {
            var response = new BaseResponse();
            try
            {
                var prosess = DataContext.ProcessBlueprints.Single(x => x.Id == Id);
                DataContext.ProcessBlueprints.Remove(prosess);
                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public GetProcessBlueprintsResponse All()
        {
            var data = DataContext.ProcessBlueprints.Include(x => x.CreatedBy).Include(z => z.FileManagerRolePrivileges).ToList();
            return new GetProcessBlueprintsResponse
            {
                TotalRecords = data.Count(),
                ProcessBlueprints = data.ToList().MapTo<GetProcessBlueprintsResponse.ProcessBlueprint>()
            };
        }


        public GetProcessBlueprintPrivilegesResponse GetPrivileges(GetProcessBlueprintPrivilegeRequest request)
        {
            var response = new GetProcessBlueprintPrivilegesResponse();
            try
            {


                //var data = DataContext.FileManagerRolePrivileges.Include(x => x.ProcessBlueprint).Include(y => y.RoleGroup).ToList();
                var data = new List<FileManagerRolePrivilege>();
                #region request.FileId > 0 && request.RoleGroupId == 0
                if (request.FileId > 0 && request.RoleGroupId == 0)
                {
                    var file = DataContext.ProcessBlueprints.Find(request.FileId);
                    var roles = DataContext.RoleGroups.ToList();
                    foreach (var role in roles)
                    {
                        var privilege = DataContext.FileManagerRolePrivileges.Include(x => x.ProcessBlueprint).Include(y => y.RoleGroup).SingleOrDefault(x => x.ProcessBlueprint_Id == request.FileId && x.RoleGroup_Id == role.Id);
                        if (privilege == null)
                        {
                            privilege = new FileManagerRolePrivilege
                            {
                                ProcessBlueprint = file,
                                ProcessBlueprint_Id = file.Id,
                                RoleGroup = role,
                                RoleGroup_Id = role.Id,
                                AllowBrowse = false,
                                AllowCopy = false,
                                AllowCreate = false,
                                AllowDelete = false,
                                AllowDownload = false,
                                AllowMove = false,
                                AllowRename = false,
                                AllowUpload = false
                            };
                        }
                        data.Add(privilege);
                    }
                    //var file = DataContext.ProcessBlueprints.AsNoTracking().Include(x=>x.FileManagerRolePrivileges).First(x => x.Id == request.FileId);
                    //data = data.Where(x => x.ProcessBlueprint.Id == request.FileId).ToList();
                }
                #endregion
                #region request.RoleGroupId > 0 && request.FileId == 0
                if (request.RoleGroupId > 0 && request.FileId == 0)
                {
                    var files = DataContext.ProcessBlueprints.AsNoTracking().ToDictionary(x => x.Id);
                    var role = DataContext.RoleGroups.Find(request.RoleGroupId);
                    foreach (var file in files)
                    {
                        var privilege = DataContext.FileManagerRolePrivileges.Include(x => x.ProcessBlueprint).Include(y => y.RoleGroup).SingleOrDefault(x => x.ProcessBlueprint_Id == file.Key && x.RoleGroup_Id == request.RoleGroupId);
                        if (privilege == null)
                        {
                            privilege = new FileManagerRolePrivilege
                            {
                                ProcessBlueprint = file.Value,
                                ProcessBlueprint_Id = file.Key,
                                RoleGroup = role,
                                RoleGroup_Id = role.Id,
                                AllowBrowse = false,
                                AllowCopy = false,
                                AllowCreate = false,
                                AllowDelete = false,
                                AllowDownload = false,
                                AllowMove = false,
                                AllowRename = false,
                                AllowUpload = false
                            };
                        }
                        data.Add(privilege);
                    }
                    //data = data.Where(x => x.RoleGroup.Id == request.RoleGroupId).ToList();
                }
                #endregion
                #region if both exist
                if (request.FileId > 0 && request.RoleGroupId > 0)
                {
                    var file = DataContext.ProcessBlueprints.Find(request.FileId);
                    var role = DataContext.RoleGroups.Find(request.RoleGroupId);
                    var privilege = DataContext.FileManagerRolePrivileges.Include(x => x.ProcessBlueprint).Include(y => y.RoleGroup).SingleOrDefault(x => x.ProcessBlueprint_Id == request.FileId && x.RoleGroup_Id == request.RoleGroupId);
                    if (privilege != null)
                    {
                        data.Add(privilege);
                    }
                }
                #endregion
                #region if both 0
                if (request.FileId == 0 && request.RoleGroupId == 0)
                {
                    var files = DataContext.ProcessBlueprints.AsNoTracking().ToDictionary(x => x.Id);
                    var roles = DataContext.RoleGroups.AsNoTracking().ToDictionary(x => x.Id);
                    foreach (var file in files)
                    {
                        foreach (var role in roles)
                        {
                            var privilege = DataContext.FileManagerRolePrivileges.Include(x => x.ProcessBlueprint).Include(y => y.RoleGroup).SingleOrDefault(x => x.ProcessBlueprint_Id == file.Key && x.RoleGroup_Id == role.Key);
                            if (privilege == null)
                            {
                                privilege = new FileManagerRolePrivilege
                                {
                                    ProcessBlueprint = file.Value,
                                    ProcessBlueprint_Id = file.Key,
                                    RoleGroup = role.Value,
                                    RoleGroup_Id = role.Key,
                                    AllowBrowse = false,
                                    AllowCopy = false,
                                    AllowCreate = false,
                                    AllowDelete = false,
                                    AllowDownload = false,
                                    AllowMove = false,
                                    AllowRename = false,
                                    AllowUpload = false
                                };
                            }
                            data.Add(privilege);
                        }
                    }
                }
                #endregion
                if (data.Count() > 0)
                {
                    response.FileManagerRolePrivileges = data.ToList().MapTo<GetProcessBlueprintPrivilegesResponse.FileManagerRolePrivilege>();
                    response.IsSuccess = response.FileManagerRolePrivileges != null;
                    response.TotalRecords = response.FileManagerRolePrivileges != null ? response.FileManagerRolePrivileges.Count() : 0;

                }
            }
            catch (InvalidOperationException e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }

            return response;


        }


        public BaseResponse BatchUpdateFilePrivilege(Requests.FileManagerRolePrivilege.BatchUpdateFilePrivilegeRequest request)
        {
            var response = new BaseResponse();
            try
            {
                int addCounter = 0;
                int updatedCounter = 0;
                foreach (var item in request.BatchUpdateFilePrivilege)
                {
                    if (item.ProcessBlueprint_Id > 0 && item.RoleGroup_Id > 0)
                    {
                        var toUpdate = DataContext.FileManagerRolePrivileges.Find(item.ProcessBlueprint_Id, item.RoleGroup_Id);
                        if (toUpdate != null)
                        {
                            // put update code here
                            toUpdate.AllowBrowse = item.AllowBrowse;
                            toUpdate.AllowCopy = item.AllowCopy;
                            toUpdate.AllowCreate = item.AllowCreate;
                            toUpdate.AllowDelete = item.AllowDelete;
                            toUpdate.AllowDownload = item.AllowDownload;
                            toUpdate.AllowMove = item.AllowMove;
                            toUpdate.AllowRename = item.AllowRename;
                            toUpdate.AllowUpload = item.AllowUpload;
                            DataContext.Entry(toUpdate).State = EntityState.Modified;
                            updatedCounter++;
                        }
                        else
                        {
                            //put insert code here
                            var privilege = item.MapTo<FileManagerRolePrivilege>();
                            DataContext.FileManagerRolePrivileges.Add(privilege);
                            addCounter++;
                        }
                    }
                }
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = string.Format("{0} data has been added, {1} data has been updated", addCounter.ToString()
                   , updatedCounter.ToString());
            }
            catch (InvalidOperationException inval)
            {
                response.Message = inval.Message;
            }
            catch (ArgumentNullException arg)
            {
                response.Message = arg.Message;
            }
            return response;
        }


        public BaseResponse InsertOwnerPrivilege(Requests.FileManagerRolePrivilege.FilePrivilegeRequest request)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                var privilege = request.MapTo<FileManagerRolePrivilege>();
                DataContext.FileManagerRolePrivileges.Add(privilege);
                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (InvalidOperationException inval)
            {
                response.Message = inval.Message;
            }
            catch (ArgumentNullException arg)
            {
                response.Message = arg.Message;
            }
            return response;
        }
    }
}
