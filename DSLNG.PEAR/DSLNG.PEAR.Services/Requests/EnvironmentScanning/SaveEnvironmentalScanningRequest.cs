using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.EnvironmentScanning
{
    public class SaveEnvironmentalScanningRequest
    {
        public int Id { get; set; }
        public int Esid { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
