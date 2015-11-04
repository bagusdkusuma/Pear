using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Menu
{
    public class GetMenuRequest
    {
        public int Id { get; set; }
    }

    public class GetMenusRequest : GridBaseRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class GetMenuRequestByUrl {
        public string Url { get; set; }
        public int RoleId { get; set; }
    }

    public class GetParentMenuRequest {
        public int ParentId { get; set; }
    }
}
