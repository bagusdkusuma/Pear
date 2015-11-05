using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.AssumptionConfig
{
    public class GetAssumptionConfigResponse
    {

        public string Name { get; set; }
        public int IdCategory { get; set; }
        public int IdMeasurement { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }

    }
}
