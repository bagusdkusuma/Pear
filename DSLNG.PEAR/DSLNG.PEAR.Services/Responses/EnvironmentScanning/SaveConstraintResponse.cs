using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EnvironmentScanning
{
    public class SaveConstraintResponse : BaseResponse
    {
        public int Id { get; set; }
        public int[] RelationIds { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int[] ThreatIds { get; set; }
        public int[] OpportunityIds { get; set; }
        public int[] WeaknessIds { get; set; }
        public int[] StrengthIds { get; set; }

    }    
}
