using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.InputData
{
    public class CreateInputDataViewModel
    {
        public CreateInputDataViewModel()
        {
            Accountabilities = new List<SelectListItem>();
            PeriodeTypes = new List<SelectListItem>();
            GroupInputDatas = new List<GroupInputData>();
            Kpis = new List<Kpi>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PeriodeType { get; set; }
        [Display(Name="Accountability")]
        public int AccountabilityId { get; set; }
        public int  Order { get; set; }
        public IList<SelectListItem> Accountabilities { get; set; }        
        public IList<SelectListItem> PeriodeTypes { get; set; }
        public IList<GroupInputData> GroupInputDatas { get; set; }
        public IList<Kpi> Kpis { get; set; }
        
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
            public int Id { get; set; }
            public int KpiId { get; set; }
            public string KpiName { get; set; }
            public string KpiMeasurement { get; set; }
            public int Order { get; set; }
        }

        public class Kpi
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Measurement { get; set; }
        }


    }
}