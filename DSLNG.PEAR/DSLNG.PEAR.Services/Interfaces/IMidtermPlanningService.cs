

using DSLNG.PEAR.Services.Requests.MidtermPlanning;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.MidtermPlanning;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IMidtermPlanningService
    {
        GetMidtermPlanningsResponse GetByStageId(int id);
        AddMidtermPlanningResponse Add(AddMidtermPlanningRequest request);
        AddObjectiveResponse AddObejctive(AddObjectiveRequest request);
        AddPlanningKpiResponse AddKpi(AddPlanningKpiRequest request);
        BaseResponse DeleteObjective(int id);
        BaseResponse Delete(int id);
        BaseResponse DeleteKpi(int id, int midTermId);
        SubmitMidtermPlanningResponse SubmitMidtermPlanning(int id);
        bool IsValid(int id);
    }
}
