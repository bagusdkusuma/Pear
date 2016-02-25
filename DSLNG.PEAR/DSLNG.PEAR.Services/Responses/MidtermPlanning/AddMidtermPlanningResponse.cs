
using System;
namespace DSLNG.PEAR.Services.Responses.MidtermPlanning
{
    public class AddMidtermPlanningResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
