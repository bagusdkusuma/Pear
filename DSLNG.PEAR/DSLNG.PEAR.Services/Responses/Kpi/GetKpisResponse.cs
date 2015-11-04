using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Kpi
{
    public class GetKpisResponse : BaseResponse
    {
        public IList<Kpi> Kpis { get; set; }
        public int TotalRecords { get; set; }

        public class Kpi
        {
            public int Id { get; set; }

            public string Code { get; set; }
            public string Name { get; set; }
            public int? PilarId { get; set; } //to make this nullable we need to include it
            public string PillarName { get; set; }
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
            public int Id { get; set; }
            public string Method { get; set; }
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
}
