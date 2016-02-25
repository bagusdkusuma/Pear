

using DSLNG.PEAR.Services.Requests.MidtermPlanning;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.MidtermPlanning;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IMidtermPlanningService
    {
        GetMidtermPlanningsResponse GetByStageId(int id);
        AddObjectiveResponse AddObejctive(AddObjectiveRequest request);
        AddPlanningKpiResponse AddKpi(AddPlanningKpiRequest request);
        BaseResponse DeleteObjective(int id);
    }
}
