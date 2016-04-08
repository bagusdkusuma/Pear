using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Web.ViewModels.KpiAchievement
{
    public class UpdateKpiAchievementsViewModel 
    {
        public UpdateKpiAchievementsViewModel()
        {
            Pillars = new List<Pillar>();
        }

        public int PmsSummaryId { get; set; }
        public string PeriodeType { get; set; }
        public IList<Pillar> Pillars { get; set; }
        public string ViewName { get { return PeriodeType.ToLowerInvariant() == "yearly" ? "_Yearly" : "_Monthly"; } }
        public IList<SelectListItem> PeriodeTypes { get; set; }
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

        public class KpiAchievementItem
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
    }
}