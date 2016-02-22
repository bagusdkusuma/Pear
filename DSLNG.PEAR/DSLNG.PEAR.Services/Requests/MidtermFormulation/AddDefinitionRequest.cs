using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.MidtermFormulation
{
    public class AddDefinitionRequest
    {
        public int Id { get; set; }
        public int MidtermPhaseStageId { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
