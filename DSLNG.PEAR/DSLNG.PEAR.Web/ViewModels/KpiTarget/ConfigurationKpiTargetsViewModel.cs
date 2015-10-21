using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.KpiTarget
{
    public class ConfigurationKpiTargetsViewModel
    {
        public ConfigurationKpiTargetsViewModel()
        {
            Kpis = new List<Kpi>();
        }
        public IList<Kpi> Kpis { get; set; }
        public string RoleGroupName { get; set; }
        public int RoleGroupId { get; set; }

        public IEnumerable<SelectListItem> Years { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string PeriodeType { get; set; }

        public string FileName { get; set; }

        public class Kpi
        {
            public Kpi()
            {
                KpiTargets = new List<KpiTarget>();
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public string PeriodeType { get; set; }
            public string Measurement { get; set; }
            public IList<KpiTarget> KpiTargets { get; set; }
        }
        
        public class KpiTarget
        {
            public int Id { get; set; }
            public string Remark { get; set; }
            public double? Value { get; set; }
            public DateTime Periode { get; set; }
        }
    }
}