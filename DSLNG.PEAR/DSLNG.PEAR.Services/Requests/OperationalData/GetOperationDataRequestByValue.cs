using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.OperationalData
{
    public class GetOperationDataRequestByValue
    {
        public int Kpi_Id { get; set; }
        public string PeriodeType { get; set; }
        public DateTime periode { get; set; }
        public int Scenario_Id { get; set; }
        public int Group_Id { get; set; }
    }
}
