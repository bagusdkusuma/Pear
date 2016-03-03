using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.BusinessPosture
{
    public class GetPostureConstraintResponse
    {
        public int Id { get; set; }
        public List<DesiredState> DesiredStates { get; set; }

        public class DesiredState
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}
