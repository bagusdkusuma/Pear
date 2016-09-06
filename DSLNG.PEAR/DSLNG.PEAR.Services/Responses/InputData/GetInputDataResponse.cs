using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.InputData
{
    public class GetInputDataResponse : BaseResponse
    {
        public GetInputDataResponse()
        {
            GroupInputDatas = new List<GroupInputData>();
        }

        public int Id { get; set; }
        public string PeriodeType { get; set; }
        public int AccountabilityId { get; set; }
        public string Name { get; set; }
        public DateTime LastInput { get; set; }
        public IList<GroupInputData> GroupInputDatas { get; set; }

        public class GroupInputData
        {
            public GroupInputData()
            {
                InputDataKpiAndOrders = new List<InputDataKpiAndOrder>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public IList<InputDataKpiAndOrder> InputDataKpiAndOrders { get; set; }
            public int Order { get; set; }
        }

        public class InputDataKpiAndOrder
        {
            public int Id { get; set; }
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string KpiMeasurement{ get; set; }
            public string Val { get; set; }
            public int Order { get; set; }
        }
        
    }
}
