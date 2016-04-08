


using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Requests.OperationalData
{
    public class GetOperationDataConfigurationRequest
    {
        public int RoleGroupId { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public int ScenarioId { get; set; }
        public int Year { get; set; }
        public bool IsPartial { get; set; }
    }
}
