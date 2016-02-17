using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EnvironmentScanning
{
    public class SaveChallengeResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public int[] RelationIds { get; set; }
    }

}
