using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.MirDataTable
{
    public class DeleteKpiMirDataTableRequest
    {
        public int KpiId { get; set; }
        public int MirDataTableId { get; set; }
        public int MirConfigureId { get; set; }
    }
}
