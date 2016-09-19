using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.KpiInformation
{
    public class GetKpiTransformationsResponse
    {
        public IList<KpiTransformationResponse> KpiTransformations { get; set; }
        public int TotalRecords { get; set; }
        public class KpiTransformationResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public string PeriodeTypeName { get { return Enum.GetName(typeof(PeriodeType), this.PeriodeType); } }
            public ICollection<RoleGroupResponse> RoleGroups { get; set; }
            public DateTime? LastProcessing { get; set; }
        }

        public class RoleGroupResponse {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
