

using DSLNG.PEAR.Services.Requests.PlanningBlueprint;
using DSLNG.PEAR.Services.Responses.MidtermFormulation;
using DSLNG.PEAR.Services.Responses.PlanningBlueprint;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IPlanningBlueprintService
    {
        GetPlanningBlueprintsResponse GetPlanningBlueprints(GetPlanningBlueprintsRequest request);
        SavePlanningBlueprintResponse SavePlanningBlueprint(SavePlanningBlueprintRequest request);
        GetVoyagePlanResponse GetVoyagePlan();
        ApproveVoyagePlanResponse ApproveVoyagePlan(int id);
        ApproveMidtermStrategyResponse ApproveMidtermStrategy(int id);
        GetMidtermFormulationResponse GetMidtermStrategy();
    }
}
