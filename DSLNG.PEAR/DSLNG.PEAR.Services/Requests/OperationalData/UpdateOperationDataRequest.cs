using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.OperationalData
{
    public class UpdateOperationDataRequest
    {
        public int Id { get; set; }
        public int ScenarioId { get; set; }
        public int KeyOperationConfigId { get; set; }
        public int KpiId { get; set; }
        public double? Value { get; set; }
        public string Remark { get; set; }
        public DateTime Periode { get; set; }
        public string PeriodeType { get; set; }
    }
}
