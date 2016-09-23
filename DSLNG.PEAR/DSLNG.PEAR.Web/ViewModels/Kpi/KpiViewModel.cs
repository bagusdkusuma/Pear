using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Kpi
{
    public class KpiViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? PillarId { get; set; } //to make this nullable we need to include it
        public Pillar Pillar { get; set; }
        public Level Level { get; set; }
        public RoleGroup RoleGroup { get; set; }
        public Type Type { get; set; }
        public Group Group { get; set; }
        public bool IsEconomic { get; set; }
        public int Order { get; set; }
        public YtdFormula YtdFormula { get; set; }
        public Measurement Measurement { get; set; }
        public Method Method { get; set; }
        public int? ConversionId { get; set; }
        public Conversion Conversion { get; set; }
        public FormatInput FormatInput { get; set; }
        public Periode Periode { get; set; }
        public string Remark { get; set; }
        public ICollection<KpiRelationModel> RelationModels { get; set; }
        public DateTime? Value { get; set; }
        public ICollection<KpiTarget> KpiTargets { get; set; }
        public ICollection<KpiAchievement> KpiAchievements { get; set; }
        public bool IsActive { get; set; }
        
    }

    

        public enum YtdFormula
        {
            Sum,
            Average,
            Custom,
            NaN
        }

        public enum FormatInput
        {
            Sum = 1,
        }

        public class Periode
        {
            public int Id { get; set; }

            public PeriodeType Name { get; set; }
            public DateTime? Value { get; set; }
            public string Remark { get; set; }

            public bool IsActive { get; set; }
        }

        public class Method
        {
            public int Id { get; set; }

            public string Name { get; set; }
            public string Remark { get; set; }

            public bool IsActive { get; set; }
        }

        public class Pillar
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public int Order { get; set; }
            public string Color { get; set; }
            public string Icon { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }

        public class RoleGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Level Level { get; set; }
            public string Icon { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }

        public class Level
        {
            public int Id { get; set; }

            public string Code { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
            public string Remark { get; set; }

            public bool IsActive { get; set; }
        }

        public class Group
        {
            public int Id { get; set; }

            public string Name { get; set; }
            public int Order { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }

        public class Type
        {
            public int Id { get; set; }

            public string Name { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }

        public class Measurement
        {
            public int Id { get; set; }

            public string Name { get; set; }
            public string Remark { get; set; }

            public bool IsActive { get; set; }
        }

        public class Conversion
        {
            public int Id { get; set; }
            public Measurement From { get; set; }
            public Measurement To { get; set; }
            public float Value { get; set; }
            public string Name { get; set; }
            public bool IsReverse { get; set; }
            public bool IsActive { get; set; }
        }

        public class KpiRelationModel
        {
            public KpiRelationModel()
            {
                Methods = new List<SelectListItem>();
                Methods.Add(new SelectListItem { Text = "Quantitative", Value = "Quantitative" });
                Methods.Add(new SelectListItem { Text = "Qualitative", Value = "Qualitative" });
            }
            public int Id { get; set; }
            public int KpiId { get; set; }
            public string Method { get; set; }
            public List<SelectListItem> Methods { get; set; }
        }

        public class KpiTarget
        {
            public int Id { get; set; }
            public decimal? Value { get; set; }
            public DateTime Periode { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public string Remark { get; set; }

            public bool IsActive { get; set; }
        }

        public enum PeriodeType
        {
            Hourly,
            Daily,
            Weekly,
            Monthly,
            Yearly
        }

        public class KpiAchievement
        {
            public int Id { get; set; }
            public decimal? Value { get; set; }
            public DateTime Periode { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public string Remark { get; set; }

            public bool IsActive { get; set; }
        }
}