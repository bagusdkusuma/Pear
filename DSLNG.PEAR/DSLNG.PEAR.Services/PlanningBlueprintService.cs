
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
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Data.Entities;
using System;

namespace DSLNG.PEAR.Services
{
    public class PlanningBlueprintService : BaseService, IPlanningBlueprintService
    {
        private readonly IOutputConfigService _outputConfigService;
        public PlanningBlueprintService(IDataContext dataContext, IOutputConfigService outputConfigService)
            : base(dataContext)
        {
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
                    foreach (var keyOutputId in request.KeyOutputIds)
                    {
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
                            keyOutputConfig = new KeyOutputConfiguration { Id = keyOutputId };
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
            catch
            {
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
                .Include(x => x.MidtermPhaseFormulation)
                .Include(x => x.MidtermPhaseFormulation.MidtermPhaseFormulationStages)
                .Include(x => x.MidtermPhaseFormulation.MidtermPhaseFormulationStages.Select(y => y.Descriptions))
                .Include(x => x.MidtermPhaseFormulation.MidtermPhaseFormulationStages.Select(y => y.KeyDrivers))
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
                    var outputCategories = _outputConfigService.CalculateOputput(new CalculateOutputRequest { ScenarioId = scenario.Id });
                    var keyOutputs = outputCategories.OutputCategories.SelectMany(x => x.KeyOutputs).ToList();
                    var planningIndicatorIds = planningBluePrint.KeyOutput.Select(x => x.Id).ToArray();
                    response.EconomicIndicators = keyOutputs.Where(x => planningIndicatorIds.Contains(x.Id)).ToList().MapTo<GetVoyagePlanResponse.KeyOutputResponse>();
                }
                response.MidtermFormulationStages = planningBluePrint.MidtermPhaseFormulation.MidtermPhaseFormulationStages.MapTo<GetVoyagePlanResponse.MidtermFormulationStage>();
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
            catch
            {
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


        public BaseResponse KpiTargetInput(KpiTargetInputRequest request)
        {
            try
            {
                //yearly
                var year = request.Start.Year;
                var yearlyTarget = DataContext.KpiTargets.FirstOrDefault(x => x.Kpi.Id == request.KpiId && x.PeriodeType == PeriodeType.Yearly
                    && x.Periode.Year == year);
                var kpi = new Kpi { Id = request.KpiId };
                DataContext.Kpis.Attach(kpi);
                if (yearlyTarget != null)
                {
                    yearlyTarget.Value = request.Value;
                }
                else
                {
                    var newYearlyTarget = new KpiTarget
                    {
                        Value = request.Value,
                        PeriodeType = PeriodeType.Yearly,
                        Periode = new DateTime(request.Start.Year, 1, 1),
                        Kpi = kpi
                    };
                    DataContext.KpiTargets.Add(newYearlyTarget);
                }

                //monthly
                //var monthlyTargets = DataContext.KpiTargets.Where(x => x.Kpi.Id == request.KpiId
                //    && x.PeriodeType == PeriodeType.Monthly
                //    && x.Periode >= request.Start && x.Periode <= request.End).ToList();

                //for (var i = request.Start.Month; i <= request.End.Month; i++)
                //{
                //    var monthlyTarget = monthlyTargets.FirstOrDefault(x => x.Periode.Month == i);
                //    if (monthlyTarget != null)
                //    {
                //        monthlyTarget.Value = request.Value;
                //    }
                //    else
                //    {
                //        var newMonthlyTarget = new KpiTarget
                //        {
                //            Periode = new DateTime(year, i, 1),
                //            PeriodeType = PeriodeType.Monthly,
                //            Value = request.Value,
                //            Kpi = kpi
                //        };
                //        DataContext.KpiTargets.Add(newMonthlyTarget);
                //    }
                //}

                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Kpi Valu has been saved successfully"
                };
            }
            catch
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact adminstrator for further information"
                };
            }
        }


        public BaseResponse KpiEconomicInput(KpiEconomicInputRequest request)
        {
            try
            {
                var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsActive && x.IsDashboard);
                if (scenario == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "An error occured, please contact adminstrator for further information"
                    };
                }

                //yearly
                var year = request.Start.Year;
                var yarlyEconomic = DataContext.KeyOperationDatas.FirstOrDefault(x => x.Kpi.Id == request.KpiId && x.PeriodeType == PeriodeType.Yearly
                    && x.Periode.Year == year && x.Scenario.Id == scenario.Id);
                var kpi = new Kpi { Id = request.KpiId };
                DataContext.Kpis.Attach(kpi);
                if (yarlyEconomic != null)
                {
                    yarlyEconomic.Value = request.Value;
                }
                else
                {
                    var newYearlyEconomic = new KeyOperationData
                    {
                        Value = request.Value,
                        PeriodeType = PeriodeType.Yearly,
                        Periode = request.Start,
                        Kpi = kpi,
                        Scenario = scenario
                    };
                    DataContext.KeyOperationDatas.Add(newYearlyEconomic);
                }

                //monthly
                //var monthlyEconomics = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == request.KpiId
                //    && x.PeriodeType == PeriodeType.Monthly
                //    && x.Periode >= request.Start && x.Periode <= request.End
                //    && x.Scenario.Id == scenario.Id).ToList();

                //for (var i = request.Start.Month; i <= request.End.Month; i++)
                //{
                //    var monthlyEconomic = monthlyEconomics.FirstOrDefault(x => x.Periode.Month == i);
                //    if (monthlyEconomic != null)
                //    {
                //        monthlyEconomic.Value = request.Value;
                //    }
                //    else
                //    {
                //        var newMonthlyEconomic = new KeyOperationData
                //        {
                //            Periode = new DateTime(year, i, 1),
                //            PeriodeType = PeriodeType.Monthly,
                //            Value = request.Value,
                //            Kpi = kpi,
                //            Scenario = scenario
                //        };
                //        DataContext.KeyOperationDatas.Add(newMonthlyEconomic);
                //    }
                //}

                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Kpi Valu has been saved successfully"
                };
            }
            catch
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact adminstrator for further information"
                };
            }
        }


        public BaseResponse RejectVoyagePlan(RejectVoyagePlanRequest request)
        {
            try
            {
                var businessPosture = DataContext.BusinessPostures
                    .First(x => x.PlanningBlueprint.Id == request.PlanningBlueprintId);
                businessPosture.IsRejected = true;
                businessPosture.Notes = request.Notes;
                businessPosture.IsBeingReviewed = false;
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "The voyage plan has been rejected"
                };
            }
            catch
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured,please contact adminstrator for further information"
                };
            }
        }


        public BaseResponse RejectMidtermStrategy(RejectMidtermStrategyRequest request)
        {
            try
            {
                var midtermPlanning = DataContext.MidtermStrategyPlannings
                    .First(x => x.PlanningBlueprint.Id == request.PlanningBlueprintId);
                midtermPlanning.IsRejected = true;
                midtermPlanning.Notes = request.Notes;
                midtermPlanning.IsBeingReviewed = false;
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "The midterm strategy has been approved",
                    //BusinessPostureId = planningDashboard.BusinessPostureIdentification.Id
                };
            }
            catch
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured,please contact adminstrator for further information"
                };
            }
        }


        public GetPlanningBlueprintsResponse GetPlanningBlueprints()
        {
            return new GetPlanningBlueprintsResponse
            {

                PlanningBlueprints = DataContext.PlanningBlueprints.Where(x => x.IsActive).ToList().MapTo<GetPlanningBlueprintsResponse.PlanningBlueprint>()
            };
        }
        public GetESCategoriesResponse GetESCategories(GetESCategoriesRequest request)
        {
            int totalRecords;
            var data = SortESCategory(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetESCategoriesResponse
            {
                TotalRecords = totalRecords,
                ESCategories = data.ToList().MapTo<GetESCategoriesResponse.ESCategory>()
            };

        }

        public IEnumerable<ESCategory> SortESCategory(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.ESCategories.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name)
                            : data.OrderByDescending(x => x.Name);
                        break;
                    case "Type":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Type)
                            : data.OrderByDescending(x => x.Type);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }


        public SaveESCategoryResponse SaveESCategory(SaveESCategoryRequest request)
        {
            var esCategory = request.MapTo<ESCategory>();
            if (request.Id == 0)
            {
                DataContext.ESCategories.Add(esCategory);
            }
            else
            {
                esCategory = DataContext.ESCategories.FirstOrDefault(x => x.Id == request.Id);
                request.MapPropertiesToInstance<ESCategory>(esCategory);
            }

            DataContext.SaveChanges();

            return new SaveESCategoryResponse
            {
                Id = esCategory.Id,
                Name = esCategory.Name,
                IsSuccess = true,
                Message = "ESCategory has been saved successfully"
            };
        }




        public GetESCategoryResponse GetESCategory(GetESCategoryRequest request)
        {
            return DataContext.ESCategories.FirstOrDefault(x => x.Id == request.Id).MapTo<GetESCategoryResponse>();
        }


        public DeleteESCategoryResponse DeleteESCategory(DeleteESCategoryRequest request)
        {
            var escategory = DataContext.ESCategories.FirstOrDefault(x => x.Id == request.Id);
            DataContext.ESCategories.Attach(escategory);
            DataContext.ESCategories.Remove(escategory);
            DataContext.SaveChanges();

            return new DeleteESCategoryResponse
            {
                IsSuccess = true,
                Message = "ESCategory has been Deleted"
            };
        }


        public BaseResponse Delete(int id)
        {
            try
            {
                var environmentsScanning = DataContext.EnvironmentsScannings
                    .Include(x => x.ConstructionPhase)
                    .Include(x => x.OperationPhase)
                    .Include(x => x.ReinventPhase)
                    .Include(x => x.Threat)
                    .Include(x => x.Strength)
                    .Include(x => x.Opportunity)
                    .Include(x => x.Weakness)
                    .First(x => x.Id == id);
                foreach (var cp in environmentsScanning.ConstructionPhase.ToList())
                {
                    environmentsScanning.ConstructionPhase.Remove(cp);
                }
                foreach (var op in environmentsScanning.OperationPhase.ToList())
                {
                    environmentsScanning.OperationPhase.Remove(op);
                }
                foreach (var rp in environmentsScanning.ReinventPhase.ToList())
                {
                    environmentsScanning.ReinventPhase.Remove(rp);
                }
                foreach (var t in environmentsScanning.Threat.ToList())
                {
                    environmentsScanning.Threat.Remove(t);
                }
                foreach (var s in environmentsScanning.Strength.ToList())
                {
                    environmentsScanning.Strength.Remove(s);
                }
                foreach (var o in environmentsScanning.Opportunity.ToList())
                {
                    environmentsScanning.Opportunity.Remove(o);
                }
                foreach (var w in environmentsScanning.Weakness.ToList())
                {
                    environmentsScanning.Weakness.Remove(w);
                }

                var businessPosture = DataContext.BusinessPostures
                    .Include(x => x.Postures)
                    .Include(x => x.Postures.Select(y => y.PostureChallenges))
                    .Include(x => x.Postures.Select(y => y.PostureConstraints))
                    .Include(x => x.Postures.Select(y => y.DesiredStates))
                    .First(x => x.Id == id);

                foreach (var pc in businessPosture.Postures.SelectMany(x => x.PostureChallenges).ToList())
                {
                    DataContext.PostureChalleges.Remove(pc);
                }
                foreach (var pc in businessPosture.Postures.SelectMany(x => x.PostureConstraints).ToList())
                {
                    DataContext.PostureConstraints.Remove(pc);
                }
                foreach (var dc in businessPosture.Postures.SelectMany(x => x.DesiredStates).ToList())
                {
                    DataContext.DesiredStates.Remove(dc);
                }

                var planningBlueprint = new PlanningBlueprint { Id = id };
                DataContext.PlanningBlueprints.Attach(planningBlueprint);
                DataContext.PlanningBlueprints.Remove(planningBlueprint);
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully deleted this item"
                };
            }
            catch
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error has been occured please contact administrator for further information"
                };
            }
        }


        public BaseResponse ResetWorkflow(int id)
        {
            try
            {
                var environmentsScanning = DataContext.EnvironmentsScannings.First(x => x.PlanningBlueprint.Id == id);
                environmentsScanning.IsLocked = false;
                var businessPosture = DataContext.BusinessPostures.First(x => x.PlanningBlueprint.Id == id);
                businessPosture.IsApproved = false;
                businessPosture.IsRejected = false;
                businessPosture.IsLocked = true;
                businessPosture.IsBeingReviewed = false;
                var midtermFormulation = DataContext.MidtermPhaseFormulations.First(x => x.PlanningBlueprint.Id == id);
                midtermFormulation.IsLocked = true;
                var midtermPlanning = DataContext.MidtermStrategyPlannings.First(x => x.PlanningBlueprint.Id == id);
                midtermPlanning.IsApproved = false;
                midtermPlanning.IsRejected = false;
                midtermPlanning.IsLocked = true;
                midtermPlanning.IsBeingReviewed = false;
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully reset the workflow of the planning blueprint"
                };
            }
            catch
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured please contact the adminstrator for further information"
                };
            }

        }
    }
}
