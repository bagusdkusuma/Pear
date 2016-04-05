using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSLNG.PEAR.Data.Enums;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.KpiTarget
{
    public class CreateKpiTargetViewModel
    {
        public CreateKpiTargetViewModel()
        {
            PillarKpiTarget = new List<PillarTarget>();
        }
        public List<PillarTarget> PillarKpiTarget { get; set; }

        //public List<KpiTarget> KpiTargets { get; set; }
    }

    public class KpiTarget
    {
        public KpiTarget()
        {
            KpiList = new List<SelectListItem>();
        }
        public List<SelectListItem> KpiList {get; set;}
        public Kpi Kpi { get; set; }

        public int KpiId { get; set; }
        //public List<double?> ValueList { get; set; }
        public double? Value { get; set; }
        public DateTime Periode { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }

    public class KpiTargetItem
    {
        public int Id { get; set; }
        public int KpiId { get; set; }
        public DateTime Periode { get; set; }
        public string Value { get; set; }
        public string Remark { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public double? RealValue
        {
            get
            {
                double realValue;
                var isParsed = double.TryParse(Value, out realValue);
                return isParsed ? realValue : default(double?);
            }
        }
    }

    public class PillarTarget
    {
        public PillarTarget()
        {
            PillarList = new List<SelectListItem>();
            KpiTargetList = new List<KpiTarget>();
        }
        public int PillarId { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> PillarList { get; set; }
        public List<KpiTarget> KpiTargetList { get; set; }
    }
}