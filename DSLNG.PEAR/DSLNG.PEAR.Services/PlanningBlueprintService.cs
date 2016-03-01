
using DSLNG.PEAR.Data.Entities.Blueprint;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PlanningBlueprint;
using DSLNG.PEAR.Services.Responses.PlanningBlueprint;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Responses.MidtermFormulation;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using DSLNG.PEAR.Common.Helpers;
using DSLNG.PEAR.Services.Responses.OutputConfig;
using DSLNG.PEAR.Services.Requests.OutputConfig;

namespace DSLNG.PEAR.Services
{
    public class PlanningBlueprintService : BaseService,IPlanningBlueprintService
    {
        private readonly IOutputConfigService _outputConfigService;
        public PlanningBlueprintService(IDataContext dataContext, IOutputConfigService outputConfigService) : base(dataContext) {
            _outputConfigService = outputConfigService;
        }
        public GetPlanningBlueprintsResponse GetPlanningBlueprints(GetPlanningBlueprintsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetPlanningBlueprintsResponse
            {
                TotalRecords = totalRecords,
                PlanningBlueprints = data.ToList().MapTo<GetPlanningBlueprintsResponse.PlanningBlueprint>()
            };
        }
        public IEnumerable<PlanningBlueprint> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.PlanningBlueprints.AsQueryable();
            data = data.Include(x => x.EnvironmentsScanning)
                .Include(x => x.BusinessPostureIdentification)
                .Include(x => x.MidtermPhaseFormulation)
                .Include(x => x.MidtermStragetyPlanning);
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Title.Contains(search) || x.Description.Contains(search));
            }
           
            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Title":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Title).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.Title).ThenBy(x => x.IsActive);
                        break;
                    case "Description":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Description).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.Description).ThenBy(x => x.IsActive);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }


        public SavePlanningBlueprintResponse SavePlanningBlueprint(SavePlanningBlueprintRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var planningBluePrint = request.MapTo<PlanningBlueprint>();
                    var environmentsScanning = new EnvironmentsScanning();
                    var businessPostureIdentification = new BusinessPostureIdentification { IsLocked = true };
                    var midtermPhaseFormulation = new MidtermPhaseFormulation { IsLocked = true };
                    var midtermStrategyPlanning = new MidtermStrategyPlanning { IsLocked = true };
                    var constructionPosture = new Posture { Type = PostureType.Construction };
                    var operationPosture = new Posture { Type = PostureType.Operation };
                    var decommissioningPosture = new Posture { Type = PostureType.Decommissioning };
                    businessPostureIdentification.Postures.Add(constructionPosture);
                    businessPostureIdentification.Postures.Add(operationPosture);
                    businessPostureIdentification.Postures.Add(decommissioningPosture);
                    planningBluePrint.EnvironmentsScanning = environmentsScanning;
                    planningBluePrint.BusinessPostureIdentification = businessPostureIdentification;
                    planningBluePrint.MidtermPhaseFormulation = midtermPhaseFormulation;
                    planningBluePrint.MidtermStragetyPlanning = midtermStrategyPlanning;
                    foreach (var keyOutputId in request.KeyOutputIds) {
                        var keyOutputConfig = new KeyOutputConfiguration { Id = keyOutputId };
                        DataContext.KeyOutputConfigs.Attach(keyOutputConfig);
                        planningBluePrint.KeyOutput.Add(keyOutputConfig);
                    }
                    DataContext.PlanningBlueprints.Add(planningBluePrint);
                }
                else
                {
                    var planningBlueprint = DataContext.PlanningBlueprints
                        .Include(x => x.KeyOutput).First(x => x.Id == request.Id);
                    planningBlueprint.KeyOutput = new List<KeyOutputConfiguration>();
                    foreach (var keyOutputId in request.KeyOutputIds)
                    {
                        var keyOutputConfig = DataContext.KeyOutputConfigs.Local.FirstOrDefault(x => x.Id == keyOutputId);
                        if (keyOutputConfig == null)
                        {
                            keyOutputConfig = new KeyOutputConfiguration{Id = keyOutputId};
                            DataContext.KeyOutputConfigs.Attach(keyOutputConfig);
                        }
                        planningBlueprint.KeyOutput.Add(keyOutputConfig);
                    }
                    request.MapPropertiesToInstance<PlanningBlueprint>(planningBlueprint);
                }
                DataContext.SaveChanges();
                return new SavePlanningBlueprintResponse
                {
                    IsSuccess = true,
                    Message = "The item has been successfully saved"
                };
            }
            catch {
                return new SavePlanningBlueprintResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact administrator for further information"
                };
            }
        }


        public GetVoyagePlanResponse GetVoyagePlan()
        {
            var planningBluePrint = DataContext.PlanningBlueprints
                .Include(x => x.KeyOutput)
                .Include(x => x.EnvironmentsScanning)
                .Include(x => x.EnvironmentsScanning.ConstructionPhase)
                .Include(x => x.EnvironmentsScanning.OperationPhase)
                .Include(x => x.EnvironmentsScanning.ReinventPhase)
                .Include(x => x.EnvironmentsScanning.Challenges)
                .Include(x => x.EnvironmentsScanning.Constraints)
                .Include(x => x.BusinessPostureIdentification)
                .Include(x => x.BusinessPostureIdentification.Postures)
                .Include(x => x.BusinessPostureIdentification.Postures.Select(y => y.DesiredStates))
                .Include(x => x.BusinessPostureIdentification.Postures.Select(y => y.PostureChallenges))
                .Include(x => x.BusinessPostureIdentification.Postures.Select(y => y.PostureConstraints))
                .OrderByDescending(x => x.Id)
                .FirstOrDefault(x => x.IsActive && x.BusinessPostureIdentification.IsApproved);
            if (planningBluePrint != null)
            {
                var response = new GetVoyagePlanResponse
                {
                    ConstructionPhase = planningBluePrint.EnvironmentsScanning.ConstructionPhase.MapTo<GetVoyagePlanResponse.UltimateObjectivePoint>(),
                    OperationPhase = planningBluePrint.EnvironmentsScanning.OperationPhase.MapTo<GetVoyagePlanResponse.UltimateObjectivePoint>(),
                    ReinventPhase = planningBluePrint.EnvironmentsScanning.ReinventPhase.MapTo<GetVoyagePlanResponse.UltimateObjectivePoint>(),
                    InternalChallenge = planningBluePrint.EnvironmentsScanning.Challenges.Where(x => x.Type == "Internal").ToList().MapTo<GetVoyagePlanResponse.Challenge>(),
                    ExternalChallenge = planningBluePrint.EnvironmentsScanning.Challenges.Where(x => x.Type == "External").ToList().MapTo<GetVoyagePlanResponse.Challenge>(),
                    Constraints = planningBluePrint.EnvironmentsScanning.Constraints.MapTo<GetVoyagePlanResponse.Constraint>(),
                    ConstructionPosture = planningBluePrint.BusinessPostureIdentification.Postures.First(x => x.Type == PostureType.Construction).MapTo<GetVoyagePlanResponse.Posture>(),
                    OperationPosture = planningBluePrint.BusinessPostureIdentification.Postures.First(x => x.Type == PostureType.Operation).MapTo<GetVoyagePlanResponse.Posture>(),
                    DecommissioningPosture = planningBluePrint.BusinessPostureIdentification.Postures.First(x => x.Type == PostureType.Decommissioning).MapTo<GetVoyagePlanResponse.Posture>(),
                };
                var scenario = DataContext.Scenarios.OrderByDescending(x => x.Id).FirstOrDefault(x => x.IsActive && x.IsDashboard);
                if (scenario != null)
                {
                    var outputCategories = _outputConfigService.CalculateOputput(new CalculateOutputRequest{ScenarioId = scenario.Id});
                    var keyOutputs = outputCategories.OutputCategories.SelectMany(x => x.KeyOutputs).ToList();
                    var planningIndicatorIds = planningBluePrint.KeyOutput.Select(x => x.Id).ToArray();
                    response.EconomicIndicators = keyOutputs.Where(x => planningIndicatorIds.Contains(x.Id)).ToList().MapTo<GetVoyagePlanResponse.KeyOutputResponse>();
                }
                return response;
            }
            return null;
        }


        public ApproveVoyagePlanResponse ApproveVoyagePlan(int id)
        {
            try
            {
                var businessPosture = DataContext.BusinessPostures
                    .First(x => x.PlanningBlueprint.Id == id);
                businessPosture.IsApproved = true;
                businessPosture.IsBeingReviewed = false;
                var midtermPhase = DataContext.MidtermPhaseFormulations.First(x => x.PlanningBlueprint.Id == id);
                midtermPhase.IsLocked = false;
                DataContext.SaveChanges();
                return new ApproveVoyagePlanResponse
                {
                    IsSuccess = true,
                    Message = "The voyage plan has been approved",
                    //BusinessPostureId = planningDashboard.BusinessPostureIdentification.Id
                };
            }
            catch {
                return new ApproveVoyagePlanResponse
                {
                    IsSuccess = false,
                    Message = "An error occured,please contact adminstrator for further information"
                };
            }
        }

        public ApproveMidtermStrategyResponse ApproveMidtermStrategy(int id)
        {
            try
            {
                var midtermPlanning = DataContext.MidtermStrategyPlannings
                    .First(x => x.PlanningBlueprint.Id == id);
                midtermPlanning.IsApproved = true;
                midtermPlanning.IsBeingReviewed = false;
                DataContext.SaveChanges();
                return new ApproveMidtermStrategyResponse
                {
                    IsSuccess = true,
                    Message = "The midterm strategy has been approved",
                    //BusinessPostureId = planningDashboard.BusinessPostureIdentification.Id
                };
            }
            catch
            {
                return new ApproveMidtermStrategyResponse
                {
                    IsSuccess = false,
                    Message = "An error occured,please contact adminstrator for further information"
                };
            }
        }


        public GetMidtermFormulationResponse GetMidtermStrategy()
        {
            var planningBlueprint = DataContext.PlanningBlueprints
                .Include(x => x.MidtermPhaseFormulation)
                .Include(x => x.MidtermPhaseFormulation.MidtermPhaseFormulationStages)
                .Include(x => x.MidtermPhaseFormulation.MidtermPhaseFormulationStages.Select(y => y.Descriptions))
                .Include(x => x.MidtermPhaseFormulation.MidtermPhaseFormulationStages.Select(y => y.KeyDrivers))
                .Include(x => x.BusinessPostureIdentification)
                .Include(x => x.BusinessPostureIdentification.Postures)
                .Include(x => x.BusinessPostureIdentification.Postures.Select(y => y.DesiredStates))
                .FirstOrDefault(x => x.IsActive == true && x.MidtermStragetyPlanning.IsApproved == true);

            if (planningBlueprint != null)
            {
                return new GetMidtermFormulationResponse
                {
                    Id = planningBlueprint.MidtermPhaseFormulation.Id,
                    IsLocked = planningBlueprint.MidtermPhaseFormulation.IsLocked,
                    ConstructionPosture = planningBlueprint.BusinessPostureIdentification.Postures.First(x => x.Type == PostureType.Construction).MapTo<GetMidtermFormulationResponse.Posture>(),
                    OperationPosture = planningBlueprint.BusinessPostureIdentification.Postures.First(x => x.Type == PostureType.Operation).MapTo<GetMidtermFormulationResponse.Posture>(),
                    DecommissioningPosture = planningBlueprint.BusinessPostureIdentification.Postures.First(x => x.Type == PostureType.Decommissioning).MapTo<GetMidtermFormulationResponse.Posture>(),
                    MidtermFormulationStages = planningBlueprint.MidtermPhaseFormulation.MidtermPhaseFormulationStages.MapTo<GetMidtermFormulationResponse.MidtermFormulationStage>()
                };
            }
            return null;
        }


        public GetPlanningBlueprintResponse GetPlanningBlueprint(int id)
        {
            return DataContext.PlanningBlueprints.Include(x => x.KeyOutput).FirstOrDefault(x => x.Id == id).MapTo<GetPlanningBlueprintResponse>();
        }
    }
}
