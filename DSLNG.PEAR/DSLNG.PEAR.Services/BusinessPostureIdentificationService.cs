using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.BusinessPosture;
using DSLNG.PEAR.Services.Responses.BusinessPosture;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.Blueprint;
using System.Collections.Generic;

namespace DSLNG.PEAR.Services
{
    public class BusinessPostureIdentificationService : BaseService, IBusinessPostureIdentificationService
    {
        public BusinessPostureIdentificationService(IDataContext dataContext):base(dataContext) { 
        
        }
        public GetBusinessPostureResponse Get(GetBusinessPostureRequest request)
        {
            var response = DataContext.BusinessPostures.Include(x => x.Postures)
                .Include(x => x.PlanningBlueprint)
                .Include(x => x.Postures.Select(y => y.DesiredStates))
                .Include(x => x.Postures.Select(y => y.PostureChallenges))
                .Include(x => x.Postures.Select(y => y.PostureChallenges.Select(z => z.DesiredStates)))
                .Include(x => x.Postures.Select(y => y.PostureConstraints))
                .Include(x => x.Postures.Select(y => y.PostureConstraints.Select(z => z.DesiredStates)))
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetBusinessPostureResponse>();

            response.EnvironmentScanningHost = DataContext.EnvironmentsScannings
                .Include(x => x.PlanningBlueprint)
                .Include(x => x.ConstructionPhase)
                .Include(x => x.OperationPhase)
                .Include(x => x.ReinventPhase)
                .Include(x => x.Constraints)
                .Include(x => x.Constraints.Select(y => y.ESCategory))
                .Include(x => x.Challenges.Select(y => y.ESCategory))
                .Where(x => x.PlanningBlueprint.Id == response.PlanningBlueprintId).FirstOrDefault().MapTo<GetBusinessPostureResponse.EnvironmentScanning>();
            return response;
        }


        public SaveDesiredStateResponse SaveDesiredState(SaveDesiredStateRequest request)
        {
            try
            {
               var desiredState = request.MapTo<DesiredState>();
                
                if (request.Id == 0)
                {
                    var posture = new Posture { Id = request.PostureId };
                    DataContext.Postures.Attach(posture);
                    desiredState.Posture = posture;
                    DataContext.DesiredStates.Add(desiredState);
                }
                else
                {
                    desiredState = DataContext.DesiredStates.First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<DesiredState>(desiredState);
                }
                DataContext.SaveChanges();
                return new SaveDesiredStateResponse
                {
                    IsSuccess = true,
                    Message = "The item has been saved successfully",
                    Id = desiredState.Id,
                    Description = desiredState.Description
                };
            }
            catch {
                return new SaveDesiredStateResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the administrator for further information"
                };
            }
        }


        public DeleteDesiredStateResponse DeleteDesiredState(DeleteDesiredStateRequest request)
        {
            try
            {
                var desiredStae = new DesiredState { Id = request.Id };
                DataContext.DesiredStates.Attach(desiredStae);
                DataContext.DesiredStates.Remove(desiredStae);
                DataContext.SaveChanges();
                return new DeleteDesiredStateResponse
                {
                    IsSuccess = true,
                    Message = "The item has been deleted successfully",
                };
            }
            catch {
                return new DeleteDesiredStateResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the administrator for further information"
                };
            }
        }


        public SavePostureChallengeResponse SavePostureChallenge(SavePostureChallengeRequest request)
        {
            try
            {
                var postureChallenge = request.MapTo<PostureChallenge>();

                if (request.Id == 0)
                {
                    var posture = new Posture { Id = request.PostureId };
                    DataContext.Postures.Attach(posture);
                    postureChallenge.Posture = posture;
                    foreach (var id in request.RelationIds) {
                        var desiredState = new DesiredState { Id = id };
                        DataContext.DesiredStates.Attach(desiredState);
                        postureChallenge.DesiredStates.Add(desiredState);
                    }
                    DataContext.PostureChalleges.Add(postureChallenge);
                }
                else
                {
                    postureChallenge = DataContext.PostureChalleges.Include(x => x.DesiredStates).First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<PostureChallenge>(postureChallenge);
                    postureChallenge.DesiredStates = new List<DesiredState>();
                    foreach (var id in request.RelationIds)
                    {
                        var desiredState = DataContext.DesiredStates.Local.FirstOrDefault(x => x.Id == id);
                        if (desiredState == null)
                        {
                            desiredState = new DesiredState { Id = id };
                            DataContext.DesiredStates.Attach(desiredState);
                        }
                        postureChallenge.DesiredStates.Add(desiredState);
                    }
                   
                }
                DataContext.SaveChanges();
                return new SavePostureChallengeResponse
                {
                    IsSuccess = true,
                    Message = "The item has been saved successfully",
                    Id = postureChallenge.Id,
                    Definition = postureChallenge.Definition,
                    RelationIds = postureChallenge.DesiredStates.Select(x => x.Id).ToArray()
                };
            }
            catch
            {
                return new SavePostureChallengeResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the administrator for further information"
                };
            }
        }


        public DeletePostureChallengeResponse DeletePostureChallenge(DeletePostureChallengeRequest request)
        {
            try
            {
                var postureChallenge = new PostureChallenge { Id = request.Id };
                DataContext.PostureChalleges.Attach(postureChallenge);
                DataContext.PostureChalleges.Remove(postureChallenge);
                DataContext.SaveChanges();
                return new DeletePostureChallengeResponse
                {
                    IsSuccess = true,
                    Message = "The item has been deleted successfully",
                };
            }
            catch
            {
                return new DeletePostureChallengeResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the administrator for further information"
                };
            }
        }



        public SavePostureConstraintResponse SavePostureConstraint(SavePostureConstraintRequest request)
        {
            try
            {
                var postureConstraint = request.MapTo<PostureConstraint>();

                if (request.Id == 0)
                {
                    var posture = new Posture { Id = request.PostureId };
                    DataContext.Postures.Attach(posture);
                    postureConstraint.Posture = posture;
                    foreach (var id in request.RelationIds)
                    {
                        var desiredState = new DesiredState { Id = id };
                        DataContext.DesiredStates.Attach(desiredState);
                        postureConstraint.DesiredStates.Add(desiredState);
                    }
                    DataContext.PostureConstraints.Add(postureConstraint);
                }
                else
                {
                    postureConstraint = DataContext.PostureConstraints.Include(x => x.DesiredStates).First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<PostureConstraint>(postureConstraint);
                    postureConstraint.DesiredStates = new List<DesiredState>();
                    foreach (var id in request.RelationIds)
                    {
                        var desiredState = DataContext.DesiredStates.Local.FirstOrDefault(x => x.Id == id);
                        if (desiredState == null)
                        {
                            desiredState = new DesiredState { Id = id };
                            DataContext.DesiredStates.Attach(desiredState);
                        }
                        postureConstraint.DesiredStates.Add(desiredState);
                    }

                }
                DataContext.SaveChanges();
                return new SavePostureConstraintResponse
                {
                    IsSuccess = true,
                    Message = "The item has been saved successfully",
                    Id = postureConstraint.Id,
                    Definition = postureConstraint.Definition,
                    RelationIds = postureConstraint.DesiredStates.Select(x => x.Id).ToArray()
                };
            }
            catch
            {
                return new SavePostureConstraintResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the administrator for further information"
                };
            }
        }

        public DeletePostureConstraintResponse DeletePostureConstraint(DeletePostureConstraintRequest request)
        {
            try
            {
                var postureConstraint = new PostureConstraint { Id = request.Id };
                DataContext.PostureConstraints.Attach(postureConstraint);
                DataContext.PostureConstraints.Remove(postureConstraint);
                DataContext.SaveChanges();
                return new DeletePostureConstraintResponse
                {
                    IsSuccess = true,
                    Message = "The item has been deleted successfully",
                };
            }
            catch
            {
                return new DeletePostureConstraintResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the administrator for further information"
                };
            }
        }

        public SubmitBusinessPostureResponse SubmitBusinessPosture(int id)
        {
            try
            {
                var businessPosture = DataContext.BusinessPostures.First(x => x.Id == id);
                businessPosture.IsLocked = true;
                businessPosture.IsBeingReviewed = true;
                businessPosture.IsRejected = false;
                DataContext.SaveChanges();
                return new SubmitBusinessPostureResponse
                {
                    IsSuccess = true,
                    Message = "You have been sucessfully submit the item"
                };
            }
            catch
            {
                return new SubmitBusinessPostureResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the adminstrator for further information"
                };
            }
        }
         public GetPostureChallengeResponse GetPostureChallenge(GetPostureChallengeRequest request)
        {
            return DataContext.PostureChalleges.Where(x => x.Id == request.Id)
                .Include(x => x.DesiredStates)
                .Include(x => x.DesiredStates.Select(y => y.Posture))
                .FirstOrDefault().MapTo<GetPostureChallengeResponse>();
        }


        public GetPostureConstraintResponse GetPostureConstraint(GetPostureConstraintRequest requet)
        {
            return DataContext.PostureConstraints.Where(x => x.Id == requet.Id)
                .Include(x => x.DesiredStates)
                .Include(x => x.DesiredStates.Select(y => y.Posture))
                .FirstOrDefault().MapTo<GetPostureConstraintResponse>();
        }
    }
}
