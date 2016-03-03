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
        public List<Environmental> Relations { get; set; }
        public List<Environmental> ThreatIds { get; set; }
        public List<Environmental> Opportunitys { get; set; }
        public List<Environmental> WeaknessIds { get; set; }
        public List<Environmental> StrengthIds { get; set; }
        
        public class Environmental
        {
            public int Id { get; set; }
            public string Desc { get; set; }
        }

        
    }
}
