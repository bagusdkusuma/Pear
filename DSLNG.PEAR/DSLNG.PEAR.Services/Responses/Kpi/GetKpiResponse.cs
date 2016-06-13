using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Kpi
{
    public class GetKpiResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? PillarId { get; set; } //to make this nullable we need to include it
        public int LevelId { get; set; }
        public Level Level { get; set; }
        public RoleGroup RoleGroup { get; set; }
        public Type Type { get; set; }
        public Group Group { get; set; }
        public string IsEconomic { get; set; }
        public int Order { get; set; }
        public YtdFormula YtdFormula { get; set; }
        public Measurement Measurement { get; set; }
        public Method Method { get; set; }
        public int? ConversionId { get; set; }
        public Conversion Conversion { get; set; }
        public FormatInput FormatInput { get; set; }
        public PeriodeType Periode { get; set; }
        public string Remark { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public string CustomFormula { get; set; }
        public ICollection<KpiRelationModel> RelationModels { get; set; }
    }
}
