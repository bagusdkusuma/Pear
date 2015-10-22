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
            var users = DataContext.Users.Include(u => u.Role).ToList();
            var response = new GetUsersResponse();

            response.Users = users.MapTo<GetUsersResponse.User>();

            return response;
        }

        public GetUserResponse GetUser(GetUserRequest request)
        {
            try
            {
                var user = DataContext.Users.Include(u => u.Role).First(x => x.Id == request.Id);
                var response = user.MapTo<GetUserResponse>(); //Mapper.Map<GetUserResponse>(user);
                //response.RoleName = DataContext.RoleGroups.FirstOrDefault(x => x.Id == user.RoleId).Name.ToString();

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

                var user = request.MapTo<User>();
                user.Role = DataContext.RoleGroups.First(x => x.Id == request.RoleId);
                user.PasswordSalt = crypto.GenerateSalt(crypto.HashIterations,crypto.SaltSize);
                user.Password = crypto.Compute(request.Password, user.PasswordSalt);
                //user.Password = _pass.HashPassword(request.Password);
                DataContext.Users.Add(user);
                DataContext.SaveChanges();
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
            var response = new UpdateUserResponse();
            try
            {
                //var user = request.MapTo<User>();
                var user = DataContext.Users.Include(u => u.Role).First(x => x.Id == request.Id).MapTo<User>();
                user.Role = DataContext.RoleGroups.First(x => x.Id == request.RoleId);
                user.Username = request.Username;
                user.IsActive = request.IsActive;
                user.ChangeModel = request.ChangeModel;
                if (request.ChangePassword && request.Password != null)
                {
                    user.PasswordSalt = crypto.Salt != null ? crypto.Salt : crypto.GenerateSalt(crypto.HashIterations, crypto.SaltSize);
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
                var user = DataContext.Users.Where(x => x.Email == request.Email).Include(x => x.Role).First();
                if (user != null && user.Password == crypto.Compute(request.Password, user.PasswordSalt))
                {
                    //Include(x => x.Role).
                    response = user.MapTo<LoginUserResponse>();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = string.Format("Failed login using email <{0}> and password <{1}>", request.Email, request.Password);
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
                var user = DataContext.Users.Include(u => u.Role).First(x => x.Username == request.Name);
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

        public UpdateUserResponse ChangePassword(ChangePasswordRequest request) {
            var response = new UpdateUserResponse { IsSuccess = false, Message = "Unknown Error"};
            
            if (request.New_Password == null)
            {
                response.Message = "New Password Could not be null!";
                return response;
            }
            var user = DataContext.Users.First(x => x.Id == request.Id).MapTo<User>();
            if (user != null) {
                
                if (user.Password != crypto.Compute(request.Old_Password, user.PasswordSalt)) {
                    response.Message = "Current Password isn't correct!";
                    return response;
                }

                user.PasswordSalt = crypto.Salt != null ? crypto.Salt : crypto.GenerateSalt(crypto.HashIterations, crypto.SaltSize);
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
    }
}
