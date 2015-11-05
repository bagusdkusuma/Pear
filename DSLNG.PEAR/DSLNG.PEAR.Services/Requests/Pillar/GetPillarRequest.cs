using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Pillar
{
    public class GetPillarRequest
    {
        public int Id { get; set; }
    }

    public class GetPillarsRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
    }
}
