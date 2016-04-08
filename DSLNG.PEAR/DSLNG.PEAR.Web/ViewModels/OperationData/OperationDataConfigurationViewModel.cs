using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Web.ViewModels.OperationData
{
    public class OperationDataConfigurationViewModel
    {
        public OperationDataConfigurationViewModel()
        {
            Kpis = new List<Kpi>();
        }
        public int ScenarioId { get; set; }
        public string ConfigType { get; set; }
        public IList<SelectListItem> Scenarios { get; set; }
        public string PeriodeType { get; set; }
        public int RoleGroupId { get; set; }
        public string GroupName { get; set; }
        public IList<Kpi> Kpis { get; set; }
        public IList<SelectListItem> Years { get; set; }
        public int Year { get; set; }
        public string FileName { get; set; }

        public class Kpi
        {
            public Kpi()
            {
                OperationData = new List<OperationData>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public string PeriodeType { get; set; }
            public string MeasurementName { get; set; }
            public IList<OperationData> OperationData { get; set; }
            public string GroupName { get; set; }
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

        public class Item
        {
            public int Id { get; set; }
            public int KpiId { get; set; }
            public int ScenarioId { get; set; }
            public int OperationId { get; set; }
            public DateTime Periode { get; set; }
            public double? Value { get; set; }
            public string Remark { get; set; }
            public PeriodeType PeriodeType { get; set; }
        }
    }
}