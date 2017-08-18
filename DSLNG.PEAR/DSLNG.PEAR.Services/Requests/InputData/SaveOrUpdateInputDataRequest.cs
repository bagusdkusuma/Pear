using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.InputData
{
    public class SaveOrUpdateInputDataRequest : BaseRequest
    {
        public SaveOrUpdateInputDataRequest()
        {
            GroupInputDatas = new List<GroupInputData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PeriodeType { get; set; }
        public int UpdatedById { get; set; }

        public int AccountabilityId { get; set; }
        public int Order { get; set; }
        public IList<GroupInputData> GroupInputDatas { get; set; }

        public class GroupInputData
        {
            public GroupInputData()
            {
                InputDataKpiAndOrders = new List<InputDataKpiAndOrder>();
            }

            public string Name { get; set; }
            public IList<InputDataKpiAndOrder> InputDataKpiAndOrders { get; set; }
            public int Order { get; set; }
        }

        public class InputDataKpiAndOrder
        {
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public int Order { get; set; }
        }
    }
}
