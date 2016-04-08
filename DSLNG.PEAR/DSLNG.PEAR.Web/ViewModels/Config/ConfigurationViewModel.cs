using DSLNG.PEAR.Web.ViewModels.Kpi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Config
{
    public class ConfigurationViewModel
    {
        public IList<Kpi> Kpis { get; set; }

        public IEnumerable<SelectListItem> Years { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }
        public IEnumerable<SelectListItem> PeriodeTypes { get; set; }
        public IEnumerable<SelectListItem> ConfigTypes { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string PeriodeType { get; set; }
        public string ConfigType { get; set; }
        public string FileName { get; set; }

        public class Kpi
        {
            public Kpi()
            {
                KpiAchievements = new List<KpiAchievement>();
                KpiTargets = new List<KpiTarget>();
                OperationData = new List<OperationData>();
                Items = new List<Item>();

            }

            public int Id { get; set; }
            public string Name { get; set; }
            public string PeriodeType { get; set; }
            public string Measurement { get; set; }
            public IList<KpiAchievement> KpiAchievements { get; set; }
            public IList<KpiTarget> KpiTargets { get; set; }
            public IList<OperationData> OperationData { get; set; }
            public IList<Item> Items { get; set; }
        }

        public class KpiAchievement
        {
            public int Id { get; set; }
            public string Remark { get; set; }
            public double? Value { get; set; }
            public DateTime Periode { get; set; }
        }

        public class KpiTarget
        {
            public int Id { get; set; }
            public string Remark { get; set; }
            public double? Value { get; set; }
            public DateTime Periode { get; set; }
        }

        public class OperationData
        {
            public int Id { get; set; }
            public string Remark { get; set; }
            public double? Value { get; set; }
            public DateTime Periode { get; set; }
        }

        public class Item
        {
            public int Id { get; set; }
            public int KpiId { get; set; }
            public DateTime Periode { get; set; }
            public string Value { get; set; }
            public double? RealValue
            {
                get
                {
                    double realValue;
                    var isParsed = double.TryParse(Value, out realValue);
                    return isParsed ? realValue : default(double?);
                }
            }
            public string Remark { get; set; }
            public Data.Enums.PeriodeType PeriodeType { get; set; }
            public int ScenarioId { get; set; }
            public int OperationId { get; set; }
        }
    }
}