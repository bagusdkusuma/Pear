
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Requests.KpiTransformation
{
    public class SaveKpiTransformationRequest : BaseRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] RoleGroupIds { get; set; }
        public int[] KpiIds { get; set; }
        public PeriodeType PeriodeType { get; set; }
    }
}
