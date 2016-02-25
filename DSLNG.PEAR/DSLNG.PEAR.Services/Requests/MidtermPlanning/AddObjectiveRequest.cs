

namespace DSLNG.PEAR.Services.Requests.MidtermPlanning
{
    public class AddObjectiveRequest
    {
        public int Id { get; set; }
        public int MidtermPlanningId { get; set; }
        public string Value { get; set; }
    }
}
