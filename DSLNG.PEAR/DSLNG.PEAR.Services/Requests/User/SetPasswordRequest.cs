using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.User
{
    public class SetPasswordRequest
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
        
    }
}
