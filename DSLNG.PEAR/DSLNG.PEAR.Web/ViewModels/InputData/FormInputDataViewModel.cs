using DSLNG.PEAR.Web.ViewModels.DerTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.InputData
{
    public class FormInputDataViewModel
    {
        public FormInputDataViewModel()
        {
            GroupInputDatas = new List<GroupInputData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PeriodeType { get; set; }
        public DateTime Date { get; set; }
        public IList<GroupInputData> GroupInputDatas { get; set; }
        public IList<DerValuesViewModel.KpiInformationValuesViewModel> KpiInformationValues { get; set; }

        public class GroupInputData
        {
            public GroupInputData()
            {
                InputDataKpiAndOrders = new List<InputDataKpiAndOrder>();
            }

            public string Name { get; set; }
            public IList<InputDataKpiAndOrder> InputDataKpiAndOrders { get; set; }
            public int Order { get; set; }

            public class InputDataKpiAndOrder
            {
                public int Id { get; set; }
                public int KpiId { get; set; }
                public string KpiName { get; set; }
                public string KpiMeasurement { get; set; }
                public int Order { get; set; }
            }
        }
    }
}