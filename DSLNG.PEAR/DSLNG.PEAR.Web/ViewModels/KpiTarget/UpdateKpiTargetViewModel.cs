using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Data.Enums;


namespace DSLNG.PEAR.Web.ViewModels.KpiTarget
{
    public class UpdateKpiTargetViewModel 
    {
        public UpdateKpiTargetViewModel()
        {
            Pillars = new List<Pillar>();
        }

        public int Year { get; set; }
        public int PmsSummaryId { get; set; }
        public string PeriodeType { get; set; }
        public IList<Pillar> Pillars { get; set; }
        public string ViewName { get { return PeriodeType.ToLowerInvariant() == "yearly" ? "_Yearly" : "_Monthly"; } }
        public IList<SelectListItem> PeriodeTypes { get; set; }

        public class Pillar
        {
            public Pillar()
            {
                Kpis = new List<Kpi>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public IList<Kpi> Kpis { get; set; }
        }

        public class Kpi
        {
            public Kpi()
            {
                KpiTargets = new List<KpiTarget>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Measurement { get; set; }
            public string Remark { get; set; }
            public IList<KpiTarget> KpiTargets { get; set; }
        }

        public class KpiTarget
        {
            public int Id { get; set; }
            public DateTime Periode { get; set; }
            public double? Value { get; set; }
            public string Remark { get; set; }
        }

        public class KpiTargetItem
        {
            public int Id { get; set; }
            public int KpiId { get; set; }
            public DateTime Periode { get; set; }
            public double? Value { get; set; }
            public string Remark { get; set; }
            public PeriodeType PeriodeType { get; set; }
        }
    }
}