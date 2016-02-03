using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Kpi
{
    public class GetKpiDetailResponse : BaseResponse
    {
       
        public int Id { get; set; }
        
        public string Code { get; set; }

        public string Name { get; set; }

        public string Level { get; set; }

        public string RoleGroup { get; set; }

        public string Type { get; set; }

        public string Group { get; set; }

        public string Order { get; set; }

        public string IsEconomic { get; set; }

        public string Measurement { get; set; }

        public string Method { get; set; }

        public string Periode { get; set; }

        public string Remark { get; set; }

        public ICollection<KpiRelationModel> RelationModels { get; set; }

        public string Icon { get; set; }

        public string IsActive { get; set; }

        public string Pillar { get; set; }

        public string YtdFormula { get; set; }
    }
}
