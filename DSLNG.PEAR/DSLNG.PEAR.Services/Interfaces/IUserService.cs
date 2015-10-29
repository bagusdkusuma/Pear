using DSLNG.PEAR.Services.Requests.User;
using DSLNG.PEAR.Services.Responses.User;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IUserService
    {
        GetUserResponse GetUser(GetUserRequest request);
        GetUsersResponse GetUsers(GetUsersRequest request);
        CreateUserResponse Create(CreateUserRequest request);
        UpdateUserResponse Update(UpdateUserRequest request);
        DeleteUserResponse Delete(int id);
        LoginUserResponse Login(LoginUserRequest request);
        GetUserResponse GetUserByName(GetUserByNameRequest request);
        UpdateUserResponse ChangePassword(ChangePasswordRequest request);
        UpdateUserResponse CheckPassword(CheckPasswordRequest request);
        ResetPasswordResponse ResetPassword(ResetPasswordRequest request);

        ResetPasswordResponse GetUserByToken(ResetPasswordTokenRequest request);
        GetUserResponse GetUserByEmail(GetUserRequest request);
    }
}
