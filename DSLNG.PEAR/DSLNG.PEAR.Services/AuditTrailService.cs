using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DSLNG.PEAR.Services.Requests.AuditTrail;
using DSLNG.PEAR.Services.Responses.AuditTrail;
using DSLNG.PEAR.Data.Entities;
using System.Data.SqlClient;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Services
{
    public class AuditTrailService : BaseService, IAuditTrailService
    {
        public AuditTrailService(IDataContext dataContext) : base(dataContext)
        {
        }

        public void CreateAuditUserRequest(CreateAuditUserRequest request)
        {
            var data = request.MapTo<AuditUser>();
            var user = DataContext.Users.First(x => x.Id == request.UserId);
            var userlogin = DataContext.UserLogins.First(x => x.Id == request.Login_Id);
            try
            {
                userlogin.User = user;
                data.UserLogin = userlogin;
                data.TimeAccessed = DateTime.Now;
                DataContext.AuditUsers.Add(data);
                DataContext.SaveChanges();
            }
            catch (InvalidOperationException e)
            {

                throw new Exception(e.Message);
            }
        }

        public AuditTrailsResponse GetAuditTrail(GetAuditTrailRequest request)
        {
            var data = DataContext.AuditTrails.Where(x => x.RecordId == request.RecordId);
            return new AuditTrailsResponse
            {
                TotalRecords = data.Count(),
                AuditTrails = data.ToList().MapTo<AuditTrailsResponse.AuditTrail>()
            };
        }

        public AuditTrailsResponse GetAuditTrailDetails(int recordId)
        {
            var response = new AuditTrailsResponse();
            try
            {
                var auditDetails = DataContext.AuditTrails.Include(x=>x.User).Where(x => x.RecordId == recordId).OrderByDescending(x => x.UpdateDate).ToList();
                response.AuditTrails = auditDetails.MapTo<AuditTrailsResponse.AuditTrail>();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public AuditTrailsResponse GetAuditTrails(GetAuditTrailsRequest request)
        {
            int totalRecords;
            var data = SortData(request, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new AuditTrailsResponse
            {
                TotalRecords = totalRecords,
                AuditTrails = data.ToList().MapTo<AuditTrailsResponse.AuditTrail>()
            };
        }

        public AuditUsersResponse GetAuditUser(GetAuditUserRequest request)
        {
            var response = new AuditUsersResponse();
            try
            {
                var data = DataContext.AuditUsers.Where(x => x.UserLogin.Id == request.LoginId).OrderByDescending(x => x.TimeAccessed).ToList();
                response.AuditUsers = data.MapTo<AuditUsersResponse.AuditUser>();
                response.IsSuccess = true;
                response.Count = data.Count();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public AuditUsersResponse GetAuditUsers(GetAuditUsersRequest request)
        {
            int totalRecords;
            var data = GetAuditUserData(request, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }
            return new AuditUsersResponse
            {
                TotalRecords = totalRecords,
                AuditUsers = data.ToList().MapTo<AuditUsersResponse.AuditUser>()
            };
        }

        public AuditUserLoginResponse GetUserLogin(GetAuditUserLoginRequest request)
        {
            var response = new AuditUserLoginResponse();
            try
            {
                var data = DataContext.UserLogins.Include(x => x.User).Include(x => x.AuditUsers).FirstOrDefault(x => x.Id == request.Id);
                response = data.MapTo<AuditUserLoginResponse>();
                response.UserLoggedIn = data.User.MapTo<AuditUserLoginResponse.User>();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        public AuditUserLoginsResponse GetUserLogins(GetAuditUserLoginsRequest request)
        {
            int totalRecord = 0;
            var data = SortUserData(request, request.SortingDictionary, out totalRecord);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }
            return new AuditUserLoginsResponse
            {
                UserLogins = data.ToList().MapTo<AuditUserLoginsResponse.UserLogin>(),
                TotalRecords = totalRecord
            };
        }

        private IEnumerable<UserLogin> SortUserData(GetAuditUserLoginsRequest request, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.UserLogins.Include(x => x.AuditUsers).Include(x => x.User).AsQueryable();
            if (!string.IsNullOrEmpty(request.Search) && !string.IsNullOrWhiteSpace(request.Search))
            {
                data = data.Where(x => x.User.Username.Contains(request.Search) || x.HostName.Contains(request.Search) || x.IpAddress.Contains(request.Search));
            }
            if (request.StartDate != null)
            {
                data = data.Where(x => DbFunctions.TruncateTime(x.LastLogin) >= request.StartDate.Date);
            }
            if (request.EndDate != null)
            {
                data = data.Where(x => DbFunctions.TruncateTime(x.LastLogin) <= request.EndDate.Date);
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Username":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.LastLogin).ThenBy(x => x.User.Username)
                            : data.OrderByDescending(x => x.LastLogin).ThenByDescending(x => x.User.Username);
                        break;
                    case "HostName":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.LastLogin).ThenBy(x => x.HostName)
                            : data.OrderByDescending(x => x.LastLogin).ThenByDescending(x => x.HostName);
                        break;
                    case "Browser":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.LastLogin).ThenBy(x => x.Browser)
                            : data.OrderByDescending(x => x.LastLogin).ThenByDescending(x => x.Browser);
                        break;
                    case "IpAddress":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.LastLogin).ThenBy(x => x.IpAddress)
                            : data.OrderByDescending(x => x.LastLogin).ThenByDescending(x => x.IpAddress);
                        break;
                    default:
                        data = data.OrderByDescending(x => x.LastLogin);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }

        private IEnumerable<AuditUser> GetAuditUserData(GetAuditUsersRequest request, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.AuditUsers.AsQueryable();
            if(request.LoginId.HasValue && request.LoginId != default(int))
            {
                data = data.Where(x => x.UserLogin.Id == request.LoginId);
            }
            if (!string.IsNullOrEmpty(request.Search) && !string.IsNullOrWhiteSpace(request.Search))
            {
                data = data.Where(x => x.Url.Contains(request.Search) || x.ActionName.Contains(request.Search) || x.ControllerName.Contains(request.Search) || x.UserLogin.User.Username.Contains(request.Search));
            }
            //if (request.StartDate != null)
            //{
            //    data = data.Where(x => x.TimeAccessed >= request.StartDate);
            //}
            //if (request.EndDate != null)
            //{
            //    data = data.Where(x => x.TimeAccessed <= request.EndDate);
            //}

            //data = data.GroupBy(x => x.UserLogin.Id).Select(y => y.OrderByDescending(x => x.TimeAccessed).FirstOrDefault())
            //    .OrderByDescending(x => x.TimeAccessed).Include(x => x.UserLogin).Include(x => x.UserLogin.User);

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "User":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.TimeAccessed).ThenBy(x => x.UserLogin.User.Username)
                            : data.OrderByDescending(x => x.TimeAccessed).ThenByDescending(x => x.UserLogin.User.Username);
                        break;
                    case "ActionName":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.TimeAccessed).ThenBy(x => x.ActionName)
                            : data.OrderByDescending(x => x.TimeAccessed).ThenByDescending(x => x.ActionName);
                        break;
                    case "Controller":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.TimeAccessed).ThenBy(x => x.ControllerName)
                            : data.OrderByDescending(x => x.TimeAccessed).ThenByDescending(x => x.ControllerName);
                        break;
                    default:
                        data = data.OrderByDescending(x => x.TimeAccessed);
                        break;
                }
            }
            TotalRecords = data.Count();
            return data;
        }

        private IEnumerable<AuditTrail> SortData(GetAuditTrailsRequest request, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.AuditTrails.Include(x=>x.User).AsQueryable();
            if (!string.IsNullOrEmpty(request.Search) && !string.IsNullOrWhiteSpace(request.Search))
            {
                data = data.Where(x => x.TableName.Contains(request.Search) || x.Action.Contains(request.Search) || x.ActionName.Contains(request.Search)
                || x.ControllerName.Contains(request.Search) || x.NewValue.Contains(request.Search) || x.OldValue.Contains(request.Search));
            }

            if (request.StartDate != null)
            {
                data = data.Where(x => DbFunctions.TruncateTime(x.UpdateDate) >= request.StartDate.Date);
            }

            if (request.EndDate != null)
            {
                data = data.Where(x => DbFunctions.TruncateTime(x.UpdateDate) <= request.EndDate.Date);
            }

            //data = data.GroupBy(x => x.RecordId).Select(y => y.OrderByDescending(x => x.UpdateDate).FirstOrDefault())
            //    .OrderByDescending(x => x.UpdateDate).Include(x => x.User);

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "User":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.UpdateDate).ThenBy(x => x.User.Username)
                            : data.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.User.Username);
                        break;
                    case "TableName":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.UpdateDate).ThenBy(x => x.TableName)
                            : data.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.TableName);
                        break;
                    case "Controller":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderByDescending(x => x.UpdateDate).ThenBy(x => x.ControllerName)
                            : data.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.ControllerName);
                        break;
                    default:
                        data = data.OrderByDescending(x => x.UpdateDate);
                        break;
                }
            }
            TotalRecords = data.Count();
            return data;
        }
    }
}
