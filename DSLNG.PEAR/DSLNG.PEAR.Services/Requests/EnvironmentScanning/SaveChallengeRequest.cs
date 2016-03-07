using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.EnvironmentScanning
{
    public class SaveChallengeRequest
    {
        public int Id { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public int Category { get; set; }
        public int EnviId { get; set; }
        public int[] RelationIds { get; set; }
    }
   
}
