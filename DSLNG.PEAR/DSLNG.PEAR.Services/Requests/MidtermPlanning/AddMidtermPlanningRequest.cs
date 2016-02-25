

using System;
namespace DSLNG.PEAR.Services.Requests.MidtermPlanning
{
    public class AddMidtermPlanningRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MidtermStageId { get; set; }
    }
}
