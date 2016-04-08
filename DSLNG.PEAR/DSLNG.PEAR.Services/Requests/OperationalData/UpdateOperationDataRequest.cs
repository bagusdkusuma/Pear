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
        public string Value { get; set; }
        public double? RealValue
        {
            get
            {
                double realValue;
                var isParsed = double.TryParse(Value, out realValue);
                return isParsed ? realValue : default(double?);
            }
        }
        public string Remark { get; set; }
        public DateTime Periode { get; set; }
        public Data.Enums.PeriodeType PeriodeType { get; set; }
    }

    public class BatchUpdateOperationDataRequest
    {
        public BatchUpdateOperationDataRequest()
        {
            BatchUpdateOperationDataItemRequest = new List<UpdateOperationDataRequest>();
        }
        public List<UpdateOperationDataRequest> BatchUpdateOperationDataItemRequest { get; set; }
    }
}
