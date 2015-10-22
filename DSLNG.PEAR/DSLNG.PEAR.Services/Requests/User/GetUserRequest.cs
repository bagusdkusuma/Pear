


namespace DSLNG.PEAR.Services.Requests.User
{
    public class GetUserRequest
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }

    public class GetUserByNameRequest {
        public string Name { get; set; }
    }

    public class CheckPasswordRequest {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
