using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EnvironmentScanning
{
    public class SaveEnvironmentScanningResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
