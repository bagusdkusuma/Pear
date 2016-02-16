using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.EnvironmentScanning
{
    public class SaveEnvironmentScanningRequest
    {
        public int Id { get; set; }
        public int EsId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
