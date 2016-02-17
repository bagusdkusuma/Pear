using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.EnvironmentScanning;
using DSLNG.PEAR.Services.Responses.EnvironmentScanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.Blueprint;

namespace DSLNG.PEAR.Services
{
    public class EnvironmentScanningService : BaseService, IEnvironmentScanningService
    {
        public EnvironmentScanningService(IDataContext dataContext) : base(dataContext) { }


        public GetEnvironmentsScanningResponse GetEnvironmentsScanning(GetEnvironmentsScanningRequest request)
        {
            return DataContext.EnvironmentsScannings.Where(x => x.Id == request.Id)
                .Include(x => x.ConstructionPhase)
                .Include(x => x.OperationPhase)
                .Include(x => x.ReinventPhase)
                .Include(x => x.Threat)
                .Include(x => x.Opportunity)
                .Include(x => x.Weakness)
                .Include(x => x.Strength)
                .Include(x => x.Constraints)
                .Include(x => x.Constraints.Select(y => y.Relation))
                .Include(x => x.Constraints.Select(y => y.Relation.Select(z => z.ThreatHost)))
                .Include(x => x.Constraints.Select(y => y.Relation.Select(z => z.OpportunityHost)))
                .Include(x => x.Constraints.Select(y => y.Relation.Select(z => z.WeaknessHost)))
                .Include(x => x.Constraints.Select(y => y.Relation.Select(z => z.StrengthHost)))
                .Include(x => x.Challenges)
                .Include(x => x.Challenges.Select(y => y.Relation))
                .Include(x => x.Challenges.Select(y => y.Relation.Select(z => z.ThreatHost)))
                .Include(x => x.Challenges.Select(y => y.Relation.Select(z => z.OpportunityHost)))
                .Include(x => x.Challenges.Select(y => y.Relation.Select(z => z.WeaknessHost)))
                .Include(x => x.Challenges.Select(y => y.Relation.Select(z => z.StrengthHost)))
                .FirstOrDefault().MapTo<GetEnvironmentsScanningResponse>();
        }


        public SaveEnvironmentScanningResponse SaveEnvironmentScanning(SaveEnvironmentScanningRequest request)
        {
            if (request.Id == 0)
            {
                if (request.Type == "cp")
                {
                    var Environmen = request.MapTo<UltimateObjectivePoint>();
                    Environmen.ConstructionPhaseHost = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EsId).FirstOrDefault();
                    DataContext.UltimateObjectivePoints.Add(Environmen);
                    DataContext.SaveChanges();
                    return new SaveEnvironmentScanningResponse
                    {
                        Id = Environmen.Id,
                        Description = Environmen.Description,
                        IsSuccess = true,
                        Message = "Environment has been saved succesfully!"
                    };
                }
                else if (request.Type == "op")
                {
                    var Environmen = request.MapTo<UltimateObjectivePoint>();
                    Environmen.OperationPhaseHost = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EsId).FirstOrDefault();
                    DataContext.UltimateObjectivePoints.Add(Environmen);
                    DataContext.SaveChanges();
                    return new SaveEnvironmentScanningResponse
                    {
                        Id = Environmen.Id,
                        Description = Environmen.Description,
                        IsSuccess = true,
                        Message = "Environment has been saved succesfully!"
                    };
                }
                else if (request.Type == "rp")
                {
                    var Environmen = request.MapTo<UltimateObjectivePoint>();
                    Environmen.ReinventPhaseHost = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EsId).FirstOrDefault();
                    DataContext.UltimateObjectivePoints.Add(Environmen);
                    DataContext.SaveChanges();
                    return new SaveEnvironmentScanningResponse
                    {
                        Id = Environmen.Id,
                        Description = Environmen.Description,
                        IsSuccess = true,
                        Message = "Environment has been saved succesfully!"
                    };
                }
                else
                {
                    return new SaveEnvironmentScanningResponse
                    {
                        IsSuccess = false,
                        Message = "False data input!"
                    };
                }
            }
            else
            {
                return new SaveEnvironmentScanningResponse
                {
                    IsSuccess = false,
                    Message = "False data input"
                };
            }

        }


        public DeleteEnvironmentScanningResponse DeleteEnvironmentScanning(DeleteEnvironmentScanningRequest request)
        {

            var ultimate = new UltimateObjectivePoint { Id = request.Id };
            DataContext.UltimateObjectivePoints.Attach(ultimate);
            DataContext.UltimateObjectivePoints.Remove(ultimate);
            DataContext.SaveChanges();

            return new DeleteEnvironmentScanningResponse
            {
                IsSuccess = true,
                Message = "Deleted has been succesfully"
            };
        }




        public DeleteEnvironmentScanningResponse DeleteEnvironmentalScanning(DeleteEnvironmentScanningRequest request)
        {
            var environmental = new EnvironmentalScanning { Id = request.Id };
            DataContext.EnvironmentalScannings.Attach(environmental);
            DataContext.EnvironmentalScannings.Remove(environmental);
            DataContext.SaveChanges();

            return new DeleteEnvironmentScanningResponse
            {
                IsSuccess = true,
                Message = "Deleted has been succesfully"
            };
        }




        public SaveEnvironmentalScanningResponse SaveEnvironmentalScanning(SaveEnvironmentalScanningRequest request)
        {
            if (request.Id == 0)
            {
                if (request.EnviType == "th")
                {
                    var Environmental = request.MapTo<EnvironmentalScanning>();
                    Environmental.ThreatHost = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
                    DataContext.EnvironmentalScannings.Add(Environmental);
                    DataContext.SaveChanges();
                    return new SaveEnvironmentalScanningResponse
                    {
                        Id = Environmental.Id,
                        Description = Environmental.Desc,
                        IsSuccess = true,
                        Message = "Environmental has been saved succesfully"
                    };
                }

                else if (request.EnviType == "opp")
                {
                    var Environmental = request.MapTo<EnvironmentalScanning>();
                    Environmental.OpportunityHost = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
                    DataContext.EnvironmentalScannings.Add(Environmental);
                    DataContext.SaveChanges();
                    return new SaveEnvironmentalScanningResponse
                    {
                        Id = Environmental.Id,
                        Description = Environmental.Desc,
                        IsSuccess = true,
                        Message = "Environmental has been saved succesfully"
                    };
                }

                else if (request.EnviType == "wk")
                {
                    var Environmental = request.MapTo<EnvironmentalScanning>();
                    Environmental.WeaknessHost = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
                    DataContext.EnvironmentalScannings.Add(Environmental);
                    DataContext.SaveChanges();
                    return new SaveEnvironmentalScanningResponse
                    {
                        Id = Environmental.Id,
                        Description = Environmental.Desc,
                        IsSuccess = true,
                        Message = "Environmental has been saved succesfully"
                    };
                }

                else if (request.EnviType == "st")
                {
                    var Environmental = request.MapTo<EnvironmentalScanning>();
                    Environmental.StrengthHost = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
                    DataContext.EnvironmentalScannings.Add(Environmental);
                    DataContext.SaveChanges();
                    return new SaveEnvironmentalScanningResponse
                    {
                        Id = Environmental.Id,
                        Description = Environmental.Desc,
                        IsSuccess = true,
                        Message = "Environmental has been saved succesfully"
                    };
                }
                else
                {
                    return new SaveEnvironmentalScanningResponse
                    {
                        IsSuccess = false,
                        Message = "invalid data!"
                    };
                }
            }
            else
            {
                return new SaveEnvironmentalScanningResponse
                {
                    IsSuccess = false,
                    Message = "invalid data!"
                };
            }
        }

        public DeleteConstraintResponse DeleteConstraint(DeleteConstraintRequest request)
        {
            var constraint = new Constraint { Id = request.Id };
            DataContext.Constraint.Attach(constraint);
            DataContext.Constraint.Remove(constraint);
            DataContext.SaveChanges();

            return new DeleteConstraintResponse
            {
                IsSuccess = true,
                Message = "Constraint has been Deleted Successfully"
            };
        }


        public DeleteChallengeResponse DeleteChallenge(DeleteChallengeRequest request)
        {
            var challenge = new Challenge { Id = request.Id };
            DataContext.Challenges.Attach(challenge);
            DataContext.Challenges.Remove(challenge);
            DataContext.SaveChanges();
            return new DeleteChallengeResponse
            {
                IsSuccess = true,
                Message = "Challenge has been Deleted Successfully"
            };
        }



        public SaveConstraintResponse SaveConstraint(SaveConstraintRequest request)
        {
            var constraint = request.MapTo<Constraint>();
            constraint.EnvironmentScanning = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
            foreach (var id in request.RelationIds)
            {
                var envistate = new EnvironmentalScanning { Id = id };
                DataContext.EnvironmentalScannings.Attach(envistate);
                constraint.Relation.Add(envistate);
            }

            DataContext.Constraint.Add(constraint);
            DataContext.SaveChanges();

            var result = DataContext.Constraint
                .Include(x => x.Relation)
                .Include(x => x.Relation.Select(y => y.ThreatHost))
                .Include(x => x.Relation.Select(y => y.OpportunityHost))
                .Include(x => x.Relation.Select(y => y.WeaknessHost))
                .Include(x => x.Relation.Select(y => y.StrengthHost))
                .Where(x => x.Id == constraint.Id).FirstOrDefault();
            
            return new SaveConstraintResponse
            {
                IsSuccess = true,
                Message = "Constraint has been saved successfully",
                Category = constraint.Category,
                Definition = constraint.Definition,
                Id = constraint.Id,
                Type = constraint.Type,
                RelationIds = constraint.Relation.Select(x => x.Id).ToArray(),
                ThreatIds = result.Relation.Where(x => x.ThreatHost != null).Select(y => y.ThreatHost.Id).ToArray(),
                OpportunityIds = result.Relation.Where(x => x.OpportunityHost != null).Select(y => y.OpportunityHost.Id).ToArray(),
                WeaknessIds = result.Relation.Where(x => x.WeaknessHost != null).Select(y => y.WeaknessHost.Id).ToArray(),
                StrengthIds = result.Relation.Where(x => x.StrengthHost != null).Select(y => y.StrengthHost.Id).ToArray(),

            };

        }


        public SaveChallengeResponse SaveChallenge(SaveChallengeRequest request)
        {
            var challenge = request.MapTo<Challenge>();
            challenge.EnvironmentScanning = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
            foreach(var id in request.RelationIds)
            {
                var envistate = new EnvironmentalScanning { Id = id };
                DataContext.EnvironmentalScannings.Attach(envistate);
                challenge.Relation.Add(envistate);
            }
            DataContext.Challenges.Add(challenge);
            DataContext.SaveChanges();

            return new SaveChallengeResponse
            {
                IsSuccess = true,
                Message = "Challenge has been saved successfully",
                Category = challenge.Category,
                Definition = challenge.Definition,
                Id = challenge.Id,
                Type = challenge.Type,
                RelationIds = challenge.Relation.Select(x => x.Id).ToArray()
            };
        }
    }
}
