
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

namespace DSLNG.PEAR.Services
{
    public class PlanningBlueprintService : BaseService,IPlanningBlueprintService
    {
        public PlanningBlueprintService(IDataContext dataContext) : base(dataContext) { 
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
            data = data.Include(x => x.EnvironmentsScanning).Include(x => x.BusinessPostureIdentification);
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
                    var businessPostureIdentification = new BusinessPostureIdentification();
                    var constructionPosture = new Posture { Type = PostureType.Construction };
                    var operationPosture = new Posture { Type = PostureType.Operation };
                    var decommissioningPosture = new Posture { Type = PostureType.Decommissioning };
                    businessPostureIdentification.Postures.Add(constructionPosture);
                    businessPostureIdentification.Postures.Add(operationPosture);
                    businessPostureIdentification.Postures.Add(decommissioningPosture);
                    planningBluePrint.EnvironmentsScanning = environmentsScanning;
                    planningBluePrint.BusinessPostureIdentification = businessPostureIdentification;
                    DataContext.PlanningBlueprints.Add(planningBluePrint);
                }
                else
                {
                    var planningBlueprint = DataContext.PlanningBlueprints.First(x => x.Id == request.Id);
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
                .FirstOrDefault(x => x.IsActive && x.IsApproved);
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
                return response;
            }
            return null;
        }


        public ApproveVoyagePlanResponse ApproveVoyagePlan(int id)
        {
            try
            {
                var planningDashboard = DataContext.PlanningBlueprints
                    .First(x => x.Id == id);
                planningDashboard.IsApproved = true;
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
    }
}
