

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.MidtermFormulation;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Data.Entities.Blueprint;
using System.Globalization;
using DSLNG.PEAR.Services.Requests.MidtermFormulation;
using System;
using DSLNG.PEAR.Services.Responses;

namespace DSLNG.PEAR.Services
{
    public class MidtermFormulationService : BaseService, IMidtermFormulationService
    {
        public MidtermFormulationService(IDataContext dataContext) : base(dataContext) { }
        public GetMidtermFormulationResponse Get(int id)
        {
            var planningBlueprint = DataContext.PlanningBlueprints
                .Include(x => x.MidtermPhaseFormulation)
                .Include(x => x.MidtermPhaseFormulation.MidtermPhaseFormulationStages)
                .Include(x => x.MidtermPhaseFormulation.MidtermPhaseFormulationStages.Select(y => y.Descriptions))
                .Include(x => x.MidtermPhaseFormulation.MidtermPhaseFormulationStages.Select(y => y.KeyDrivers))
                .Include(x => x.BusinessPostureIdentification)
                .Include(x => x.BusinessPostureIdentification.Postures)
                .Include(x => x.BusinessPostureIdentification.Postures.Select(y => y.DesiredStates))
                .First(x => x.MidtermPhaseFormulation.Id == id);

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


        public AddStageResponse AddStage(AddStageRequest request)
        {
            try
            {
                var stage = request.MapTo<MidtermPhaseFormulationStage>();
                var formulation = new MidtermPhaseFormulation { Id = request.MidtermFormulationId };
                DataContext.MidtermPhaseFormulations.Attach(formulation);
                stage.MidtermPhaseFormulation = formulation;
                DataContext.MidtermPhaseFormulationStages.Add(stage);
                DataContext.SaveChanges();
                return new AddStageResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully add new stage",
                    Id = stage.Id,
                    Title = stage.Title,
                    Start = request.StartDate.HasValue ? request.StartDate.Value.ToString("MM/yyyy", CultureInfo.InvariantCulture) : "",
                    End = request.EndDate.HasValue? request.EndDate.Value.ToString("MM/yyyy", CultureInfo.InvariantCulture) : ""
                };
            }
            catch {
                return new AddStageResponse
                              {
                                  IsSuccess = false,
                                  Message = "An Error occured, please contact administrator for further information"
                              };
            }
        }


        public AddDefinitionResponse AddDefinition(AddDefinitionRequest request)
        {
            if (string.Equals(request.Type, "description", StringComparison.InvariantCultureIgnoreCase)) {
                return AddMidtermStageDescription(request);
            }
            return AddMidtermStageKey(request);
        }

        private AddDefinitionResponse AddMidtermStageKey(AddDefinitionRequest request) {
            try
            {
                var stageKey = request.MapTo<MidtermPhaseKeyDriver>();
                if (request.Id == 0)
                {
                    var formulation = new MidtermPhaseFormulationStage { Id = request.MidtermPhaseStageId };
                    stageKey.Formulation = formulation;
                    DataContext.MidtermPhaseFormulationStages.Attach(formulation);
                    DataContext.MidtermPhaseKeyDrivers.Add(stageKey);
                }
                else
                {
                    stageKey = DataContext.MidtermPhaseKeyDrivers.First(x => x.Id == request.Id);
                    stageKey.Value = request.Value;
                }
                DataContext.SaveChanges();
                return new AddDefinitionResponse
                {
                    IsSuccess = true,
                    Message = "New Stage has been added",
                    Id = stageKey.Id,
                    Value = stageKey.Value
                };
            }
            catch
            {
                return new AddDefinitionResponse
                {
                    IsSuccess = false,
                    Message = "An error has been occured, please contact the administrator for further information"
                };
            }
        }

        private AddDefinitionResponse AddMidtermStageDescription(AddDefinitionRequest request) {
            try
            {
                var stageDescription = request.MapTo<MidtermPhaseDescription>();
                if (request.Id == 0)
                {
                    var formulation = new MidtermPhaseFormulationStage { Id = request.MidtermPhaseStageId };
                    stageDescription.Formulation = formulation;
                    DataContext.MidtermPhaseFormulationStages.Attach(formulation);
                    DataContext.MidtermPhaseDescriptions.Add(stageDescription);
                }
                else
                {
                    stageDescription = DataContext.MidtermPhaseDescriptions.First(x => x.Id == request.Id);
                    stageDescription.Value = request.Value;
                }
                DataContext.SaveChanges();
                return new AddDefinitionResponse
                {
                    IsSuccess = true,
                    Message = "New Stage has been added",
                    Id = stageDescription.Id,
                    Value = stageDescription.Value
                };
            }
            catch { 
            return new AddDefinitionResponse
                {
                    IsSuccess = false,
                    Message = "An error has been occured, please contact the administrator for further information"
                };
            }
        }


        public Responses.BaseResponse DeleteStage(int id)
        {
            try
            {
                var midtermStage = new MidtermPhaseFormulationStage { Id = id };
                DataContext.MidtermPhaseFormulationStages.Attach(midtermStage);
                DataContext.MidtermPhaseFormulationStages.Remove(midtermStage);
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully deleted the item"
                };
            }
            catch {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact administrator for further information"
                };
            }
        }

        public BaseResponse DeleteStageDesc(int id)
        {
            try
            {
                var stageDesc = new MidtermPhaseDescription { Id = id };
                DataContext.MidtermPhaseDescriptions.Attach(stageDesc);
                DataContext.MidtermPhaseDescriptions.Remove(stageDesc);
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully deleted the item"
                };
            }
            catch {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact administrator for further information"
                };
            
            }
        }

        public BaseResponse DeleteStageKey(int id)
        {
            try
            {
                var stageKey = new MidtermPhaseKeyDriver { Id = id };
                DataContext.MidtermPhaseKeyDrivers.Attach(stageKey);
                DataContext.MidtermPhaseKeyDrivers.Remove(stageKey);
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully deleted the item"
                };
            }
            catch
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact administrator for further information"
                };

            }
        }
    }
}
