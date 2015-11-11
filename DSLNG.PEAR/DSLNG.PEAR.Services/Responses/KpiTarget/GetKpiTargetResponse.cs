using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Responses.KpiTarget
{
    public class GetKpiTargetResponse : BaseResponse
    {
        public GetKpiTargetResponse()
        {
            Pillars = new List<Pillar>();
        }

        public int Id { get; set; }
        public int Year { get; set; }
        public IList<Pillar> Pillars { get; set; }

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
        public PeriodeType PeriodeType { get; set; }
    }
}
