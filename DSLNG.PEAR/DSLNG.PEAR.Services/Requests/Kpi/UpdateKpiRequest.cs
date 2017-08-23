using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Responses.Kpi;

namespace DSLNG.PEAR.Services.Requests.Kpi
{
    public class UpdateKpiRequest : BaseRequest
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? PillarId { get; set; } //to make this nullable we need to include it
        public int LevelId { get; set; }
        public int? RoleGroupId { get; set; }
        public int TypeId { get; set; }
        public int? GroupId { get; set; }
        public string IsEconomic { get; set; }
        public int Order { get; set; }
        public YtdFormula YtdFormula { get; set; }
        public string CustomFormula { get; set; }
        public int MeasurementId { get; set; }
        public int MethodId { get; set; }
        public int? ConversionId { get; set; }
        public Conversion Conversion { get; set; }
        public FormatInput FormatInput { get; set; }
        public PeriodeType Periode { get; set; }
        public string Remark { get; set; }
        public string Icon { get; set; }

        public bool IsActive { get; set; }
        public ICollection<KpiRelationModel> RelationModels { get; set; }
    }

    public class DeleteKpiRequest : BaseRequest
    {
        public int Id { get; set; }
    }
}
