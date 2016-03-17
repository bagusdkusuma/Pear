

using DSLNG.PEAR.Services.Requests.PlanningBlueprint;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.MidtermFormulation;
using DSLNG.PEAR.Services.Responses.PlanningBlueprint;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IPlanningBlueprintService
    {
        GetPlanningBlueprintsResponse GetPlanningBlueprints(GetPlanningBlueprintsRequest request);
        GetPlanningBlueprintsResponse GetPlanningBlueprints();
        GetPlanningBlueprintResponse GetPlanningBlueprint(int id);
        SavePlanningBlueprintResponse SavePlanningBlueprint(SavePlanningBlueprintRequest request);
        GetVoyagePlanResponse GetVoyagePlan();
        ApproveVoyagePlanResponse ApproveVoyagePlan(int id);
        BaseResponse RejectVoyagePlan(RejectVoyagePlanRequest request);
        ApproveMidtermStrategyResponse ApproveMidtermStrategy(int id);
        BaseResponse RejectMidtermStrategy(RejectMidtermStrategyRequest request);
        GetMidtermFormulationResponse GetMidtermStrategy();
        BaseResponse KpiTargetInput(KpiTargetInputRequest request);
        BaseResponse KpiEconomicInput(KpiEconomicInputRequest request);
        GetESCategoriesResponse GetESCategories(GetESCategoriesRequest request);
        SaveESCategoryResponse SaveESCategory(SaveESCategoryRequest request);
        GetESCategoryResponse GetESCategory(GetESCategoryRequest request);
        DeleteESCategoryResponse DeleteESCategory(DeleteESCategoryRequest request);
        BaseResponse Delete(int id);
        BaseResponse ResetWorkflow(int id);
    }
}
