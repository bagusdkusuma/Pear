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
        public IList<Environmental> Relation { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public int EnviId { get; set; }
    }

    public class Environmental
    {
        public int Id { get; set; }
        public string Desc { get; set; }
    }
}
