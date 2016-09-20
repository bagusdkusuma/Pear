using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.KpiInformation
{
    public class GetKpiTransformationResponse
    {
        public GetKpiTransformationResponse() {
            RoleGroups = new List<RoleGroupResponse>();
            Kpis = new List<KpiResponse>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public ICollection<RoleGroupResponse> RoleGroups { get; set; }
        public ICollection<KpiResponse> Kpis { get; set; }
        public DateTime? LastProcessing { get; set; }

        public class RoleGroupResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class KpiResponse
        {
            public int Id { get; set;}
            public string Name { get; set; }
        }
    }
}
