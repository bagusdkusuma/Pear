using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.OperationalData
{
    public class ViewOperationDataConfigurationResponse
    {
        public IList<Kpi> Kpis { get; set; }
        public class Kpi
        {
            public Kpi()
            {
                OperationDatas = new List<OperationData>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public string PeriodeType { get; set; }
            public string MeasurementName { get; set; }
            public IList<OperationData> OperationDatas { get; set; }
        }

        public class OperationData
        {
            public int Id { get; set; }
            public string Remark { get; set; }
            public double? Value { get; set; }
            public DateTime Periode { get; set; }
            public int ScenarioId { get; set; }
            public int KeyOperationConfigId { get; set; }
        }
    }
}
