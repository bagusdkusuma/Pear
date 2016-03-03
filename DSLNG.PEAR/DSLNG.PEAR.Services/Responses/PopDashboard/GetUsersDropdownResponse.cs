using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.PopDashboard
{
    public class GetUsersDropdownResponse : BaseResponse
    {
        public IList<User> Users { get; set; }
        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string FullName { get; set; }
        }
    }
}
