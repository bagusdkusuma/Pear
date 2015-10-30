using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.User
{
    public class ResetPasswordResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Salt { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool Status { get; set; }

        public User Profile { get; set; }
        public class User {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        }
    }
}
