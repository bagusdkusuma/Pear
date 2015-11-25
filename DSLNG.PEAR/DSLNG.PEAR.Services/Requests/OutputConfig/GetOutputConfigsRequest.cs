using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.OutputConfig
{
    public class GetOutputConfigsRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
    }
}
