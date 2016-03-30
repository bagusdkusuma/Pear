using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.ProcessBlueprint
{
    public class GetProcessBlueprintRequest
    {
        public int Id { get; set; }
    }

    public class GetProcessBlueprintsRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
    }
}
