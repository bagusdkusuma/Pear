
using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.MidtermPlanning
{
    public class GetMidtermPlanningsResponse
    {
        public GetMidtermPlanningsResponse() {
            MidtermPlannings = new List<MidtermPlanning>();
            KpiTargets = new List<KpiData>();
            KpiActuals = new List<KpiData>();
        }
        public IList<MidtermPlanning> MidtermPlannings { get; set; }
        public IList<KpiData> KpiTargets { get; set; }
        public IList<KpiData> KpiActuals { get; set; }
        public class MidtermPlanning
        {
            public MidtermPlanning()
            {
                Objectives = new List<MidtermPlanningObjective>();
                Kpis = new List<Kpi>();
                KpiDatas = new List<KpiData>();
            }
            public int Id { get; set; }
            public string Title { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public IList<MidtermPlanningObjective> Objectives { get; set; }
            public IList<Kpi> Kpis { get; set; }
            public IList<KpiData> KpiDatas { get; set; }
        }
        public class MidtermPlanningObjective
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
        public class Kpi
        {
            public int Id { get; set; }
            public string Name {get; set; }
            public string Measurement { get; set; }
            public YtdFormula YtdFormula { get; set; }
            //public double? Target { get; set; }
            //public double? Economic { get; set; }
        }
        public class KpiData {
            public Kpi Kpi { get; set; }
            public double? Target { get; set; }
            public double? Economic { get; set; }
        }
    }
}
