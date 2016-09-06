
namespace DSLNG.PEAR.Services.Requests.KpiTransformation
{
    public class SaveKpiTransformationRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] RoleGroupIds { get; set; }
        public int[] KpiIds { get; set; }
    }
}
