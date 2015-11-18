using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Type
{
    public class GetTypeRequest
    {
        public int Id { get; set; }
    }

    public class GetTypesRequest : GridBaseRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
