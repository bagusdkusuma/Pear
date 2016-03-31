using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.MirDataTable
{
    public class SaveMirDataTableRequest
    {
        public int MirDataId { get; set; }
        public int MirConfigurationId { get; set; }
        public int[] KpiIds { get; set; }
        public MirTableType Type { get; set; }
    }
}
