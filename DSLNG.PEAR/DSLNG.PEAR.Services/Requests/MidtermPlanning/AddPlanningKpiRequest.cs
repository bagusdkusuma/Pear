

namespace DSLNG.PEAR.Services.Requests.MidtermPlanning
{
    public class AddPlanningKpiRequest
    {
        public int MidtermPlanningId { get; set; }
        public int KpiId { get; set; }
        public int OldKpiId { get; set; }
    }
}
