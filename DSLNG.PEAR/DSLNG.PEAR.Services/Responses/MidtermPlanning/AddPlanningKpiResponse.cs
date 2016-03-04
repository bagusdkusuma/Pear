

namespace DSLNG.PEAR.Services.Responses.MidtermPlanning
{
    public class AddPlanningKpiResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Measurement { get; set; }
        public double? Target { get; set; }
        public double? Economic { get; set; }
    }
}
