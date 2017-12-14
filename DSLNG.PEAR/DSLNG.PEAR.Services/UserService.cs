using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.User;
using DSLNG.PEAR.Services.Responses.User;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using SimpleCrypto;
using System;
//using Microsoft.AspNet.Identity;


namespace DSLNG.PEAR.Services
{
    public class UserService : BaseService, IUserService
    {
        //private PasswordHasher _pass = new PasswordHasher();
        private PBKDF2 crypto = new PBKDF2();


        public UserService(IDataContext dataContext)
            : base(dataContext)
        {

        }

        public GetUsersResponse GetUsers(GetUsersRequest request)
        {
            int totalRecords;
            var users = SortData(request.Search, request.SortingDictionary, out totalRecords).Skip(request.Skip).Take(request.Take);

            var response = new GetUsersResponse() { Users = users.MapTo<GetUsersResponse.User>(), TotalRecords = totalRecords };
            /*var users = DataContext.Users.Include(u => u.Role).ToList();
            var response = new GetUsersResponse();

            response.Users = users.MapTo<GetUsersResponse.User>();*/

            return response;
        }

        public GetUserResponse GetUser(GetUserRequest request)
        {
            try
            {
                var user = DataContext.Users.Include(x => x.Role).Include(y => y.RolePrivileges).FirstOrDefault(x => x.Id == request.Id);
                //var user = DataContext.Users.Include(u => u.Role).First(x => x.Id == request.Id);
                var response = user.MapTo<GetUserResponse>(); //Mapper.Map<GetUserResponse>(user);
                //response.RoleName = DataContext.RoleGroups.FirstOrDefault(x => x.Id == user.RoleId).Name.ToString();
                response.IsSuccess = true;
                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetUserResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        public CreateUserResponse Create(CreateUserRequest request)
        {
            var response = new CreateUserResponse();
            try
            {
                crypto.HashIterations = 10;
                crypto.SaltSize = 12;

                var user = request.MapTo<User>();
                user.Role = DataContext.RoleGroups.First(x => x.Id == request.RoleId);
                user.PasswordSalt = crypto.GenerateSalt(crypto.HashIterations, crypto.SaltSize);
                user.Password = crypto.Compute(request.Password, user.PasswordSalt);
                                
                //user.Password = _pass.HashPassword(request.Password);
                DataContext.Users.Add(user);
                DataContext.SaveChanges();


                if (request.RolePrivilegeIds.Count > 0)
                {
                    user = DataContext.Users.Include(u => u.Role).Include(r => r.RolePrivileges).First(x => x.Id == user.Id).MapTo<User>();
                    user.RolePrivileges.Clear();
                    foreach (var role in request.RolePrivilegeIds)
                    {
                        var rolePrivilege = DataContext.RolePrivileges.Find(role).MapTo<RolePrivilege>();
                        user.RolePrivileges.Add(rolePrivilege);
                    }
                    DataContext.SaveChanges();
                }
                response.IsSuccess = true;
                response.Message = "User item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public UpdateUserResponse Update(UpdateUserRequest request)
        {
            crypto.HashIterations = 10;
            crypto.SaltSize = 12;
            var response = new UpdateUserResponse();
            try
            {
                //var user = request.MapTo<User>();
                var user = DataContext.Users.Include(u => u.Role).Include(r => r.RolePrivileges).First(x => x.Id == request.Id).MapTo<User>();
                user.Role = DataContext.RoleGroups.First(x => x.Id == request.RoleId);
                user.FullName = request.FullName;
                user.RolePrivileges.Clear();
                if (request.RolePrivilegeIds.Count > 0)
                {
                    foreach (var role in request.RolePrivilegeIds)
                    {
                        var rolePrivilege = DataContext.RolePrivileges.Find(role);
                        user.RolePrivileges.Add(rolePrivilege);
                    }
                }
                user.SignatureImage = request.SignatureImage;
                user.Position = request.Position;
                user.Username = request.Username;
                user.IsActive = request.IsActive;
                user.ChangeModel = request.ChangeModel;
                user.Email = request.Email;
                if (request.ChangePassword && request.Password != null)
                {
                    user.PasswordSalt = crypto.GenerateSalt(crypto.HashIterations, crypto.SaltSize);
                    user.Password = crypto.Compute(request.Password, user.PasswordSalt);
                }
                DataContext.Users.Attach(user);
                DataContext.Entry(user).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "User item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public DeleteUserResponse Delete(int id)
        {
            var response = new DeleteUserResponse();
            try
            {
                var user = new User { Id = id };
                DataContext.Users.Attach(user);
                DataContext.Entry(user).State = EntityState.Deleted;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "User item has been deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public LoginUserResponse Login(LoginUserRequest request)
        {
            var response = new LoginUserResponse();

            try
            {
                //var user = DataContext.Users.Where(x => x.Username == request.Username).Include(x => x.Role).First();
                var user = DataContext.Users.Where(x => x.Email == request.Email || x.Username == request.Email).Include(x => x.Role).Include(y => y.RolePrivileges).First();
                if (user != null && user.Password == crypto.Compute(request.Password, user.PasswordSalt))
                {
                    //Add For Update Password
                    int HashIteration = int.Parse(user.PasswordSalt.Substring(0, user.PasswordSalt.IndexOf('.')), System.Globalization.NumberStyles.Number);
                    if (HashIteration > 10)
                    {
                        ChangePassword(new ChangePasswordRequest
                        {
                            Id = user.Id,
                            Old_Password = request.Password,
                            New_Password = request.Password
                        });
                    }
                    //Include(x => x.Role).
                    response = user.MapTo<LoginUserResponse>();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = string.Format("Failed login using email <{0}> and password <{1}>", request.Email, request.Password);
                }
                if (response.IsSuccess)
                {
                    var userlogin = new UserLogin
                    {
                        User = user,
                        IpAddress = request.IpAddress,
                        Browser = request.Browser,
                        HostName = request.HostName,
                        LastLogin = DateTime.Now
                    };
                    DataContext.UserLogins.Add(userlogin);
                    DataContext.SaveChanges();
                    response.UserLogin = userlogin.MapTo<LoginUserResponse.Login>();
                    response.UserLogin.Id = userlogin.Id;
                }
            }
            catch (System.InvalidOperationException x)
            {

                response.IsSuccess = false;
                response.Message = string.Format("Failed login using email <{0}> and password <{1}> {2}", request.Email, request.Password, x.Message);
            }

            return response;
        }


        public GetUserResponse GetUserByName(GetUserByNameRequest request)
        {
            try
            {
                var user = DataContext.Users.Include(u => u.Role).Include(y => y.RolePrivileges).First(x => x.Username == request.Name);
                var response = user.MapTo<GetUserResponse>(); //Mapper.Map<GetUserResponse>(user);
                //response.RoleName = DataContext.RoleGroups.FirstOrDefault(x => x.Id == user.RoleId).Name.ToString();
                response.IsSuccess = true;
                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetUserResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        public UpdateUserResponse ChangePassword(ChangePasswordRequest request)
        {
            var response = new UpdateUserResponse { IsSuccess = false, Message = "Unknown Error" };

            if (request.New_Password == null)
            {
                response.Message = "New Password Could not be null!";
                return response;
            }
            var user = DataContext.Users.First(x => x.Id == request.Id).MapTo<User>();
            if (user != null)
            {

                if (user.Password != crypto.Compute(request.Old_Password, user.PasswordSalt))
                {
                    response.Message = "Current Password isn't correct!";
                    return response;
                }

                // change to lesser crypto
                crypto.HashIterations = 10;
                crypto.SaltSize = 12;

                //user.PasswordSalt = crypto.Salt != null ? crypto.Salt : crypto.GenerateSalt(crypto.HashIterations, crypto.SaltSize);
                user.PasswordSalt = crypto.GenerateSalt(crypto.HashIterations, crypto.SaltSize);
                user.Password = crypto.Compute(request.New_Password, user.PasswordSalt);

                DataContext.Users.Attach(user);
                DataContext.Entry(user).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Password Successfully Changed!";
            }

            return response;
        }


        public UpdateUserResponse CheckPassword(CheckPasswordRequest request)
        {
            var response = new UpdateUserResponse();
            var user = DataContext.Users.First(x => x.Username == request.Name);
            if (user != null && request.Password != null && user.Password == crypto.Compute(request.Password, user.PasswordSalt))
            {
                //Include(x => x.Role).
                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = string.Format("Your current password isn't correct");
            }
            return response;
        }


        public ResetPasswordResponse ResetPassword(ResetPasswordRequest request)
        {
            var response = new ResetPasswordResponse();
            response = this.GenerateToken(request);
            return response;
        }

        private ResetPasswordResponse GenerateToken(ResetPasswordRequest request)
        {
            var response = new ResetPasswordResponse();


            ///Try to save token to database
            try
            {
                response.Salt = crypto.Salt != null ? crypto.Salt : crypto.GenerateSalt(crypto.HashIterations, crypto.SaltSize);

                response.Email = request.Email;
                response.ExpireDate = DateTime.Now.AddDays(3);
                response.Token = crypto.Compute(request.Email, response.Salt);
                //var entity = new ResetPassword { Email = response.Email, Token = response.Token, Salt = response.Salt, ExpireDate = response.ExpireDate };
                var entity = response.MapTo<ResetPassword>();
                DataContext.ResetPasswords.Add(entity);
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Password Token Successfully Created";
            }
            catch (System.InvalidOperationException x)
            {
                return new ResetPasswordResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
            return response;
        }


        public ResetPasswordResponse GetUserByToken(ResetPasswordTokenRequest request)
        {
            var response = new ResetPasswordResponse();
            response = this.GetResetPasswordDetail(new ResetPasswordTokenRequest { Token = request.Token });
            return response;
        }

        private ResetPasswordResponse GetResetPasswordDetail(ResetPasswordTokenRequest resetPasswordTokenRequest)
        {
            var response = new ResetPasswordResponse();
            try
            {
                response = DataContext.ResetPasswords.First(x => x.Token == resetPasswordTokenRequest.Token).MapTo<ResetPasswordResponse>();

                if (response.ExpireDate < DateTime.Now)
                {
                    response.IsSuccess = false;
                    response.Message = "Token Already Expired!";
                    return response;
                }
                if (response.Status)
                {
                    response.IsSuccess = false;
                    response.Message = "Token Already Used!";
                    return response;
                }

                response.Profile = GetUserByEmail(new GetUserRequest { Email = response.Email }).MapTo<ResetPasswordResponse.User>();
                response.IsSuccess = true;
            }
            catch (System.InvalidOperationException x)
            {
                return new ResetPasswordResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
            return response;
        }

        public GetUserResponse GetUserByEmail(GetUserRequest request)
        {
            try
            {
                var user = DataContext.Users.Include(u => u.Role).First(x => x.Email == request.Email);
                var response = user.MapTo<GetUserResponse>(); //Mapper.Map<GetUserResponse>(user);
                response.IsSuccess = true;
                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetUserResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        private IEnumerable<User> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int totalRecords)
        {
            var data = DataContext.Users.Include(x => x.Role).Include(x => x.RolePrivileges).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Username.Contains(search) || x.Email.Contains(search)
                    || x.Role.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Username":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Username)
                                   : data.OrderByDescending(x => x.Username);
                        break;
                    case "Email":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Email)
                                   : data.OrderByDescending(x => x.Email);
                        break;
                    case "isActive":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.IsActive)
                                   : data.OrderByDescending(x => x.IsActive);
                        break;
                    case "IsSuperAdmin":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.IsSuperAdmin)
                                   : data.OrderByDescending(x => x.IsSuperAdmin);
                        break;
                    case "RoleName":
                        data = sortOrder.Value == SortOrder.Ascending
                                   ? data.OrderBy(x => x.Role.Name)
                                   : data.OrderByDescending(x => x.Role.Name);
                        break;
                }
            }
            totalRecords = data.Count();
            return data;
        }
    }
}
