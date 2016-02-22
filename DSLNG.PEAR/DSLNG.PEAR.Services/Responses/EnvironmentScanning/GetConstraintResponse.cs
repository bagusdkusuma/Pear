using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EnvironmentScanning
{
    public class GetConstraintResponse : BaseResponse
    {
        public int Id { get; set; }
        public List<environment> Relation { get; set; }
                


        public class environment
        {
            public int Id { get; set; }
            public string Desc { get; set; }
        }
    }
}
