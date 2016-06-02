using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Files
{
    public class GetFilesRequest : GridBaseRequest
    {
        public int Year { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
