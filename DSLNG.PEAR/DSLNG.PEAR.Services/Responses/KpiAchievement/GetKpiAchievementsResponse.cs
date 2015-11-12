using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.KpiAchievement
{
    public class GetKpiAchievementsResponse : BaseResponse
    {
        public GetKpiAchievementsResponse()
        {
            Pillars = new List<Pillar>();
        }

        public IList<Pillar> Pillars { get; set; }
        public int Year { get; set; }

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
                KpiAchievements = new List<KpiAchievement>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Measurement { get; set; }
            public string Remark { get; set; }

            public IList<KpiAchievement> KpiAchievements { get; set; }
        }

        public class KpiAchievement
        {
            public int Id { get; set; }
            public DateTime Periode { get; set; }
            public double? Value { get; set; }
            public string Remark { get; set; }
        }
    }
}
