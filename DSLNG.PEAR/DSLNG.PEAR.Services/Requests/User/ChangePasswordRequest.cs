using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.User
{
    public class ChangePasswordRequest
    {
        public int Id { get; set; }
        public string New_Password { get; set; }
        public string Old_Password { get; set; }
    }
}
