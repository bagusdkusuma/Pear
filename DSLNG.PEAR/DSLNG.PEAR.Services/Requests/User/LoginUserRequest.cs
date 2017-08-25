using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.User
{
    public class LoginUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public string Browser { get; set; }
        public string HostName { get; set; }
    }
}
