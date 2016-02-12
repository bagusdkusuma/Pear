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
                    Environmental.Threat = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
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
                    Environmental.Opportunity = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
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
                    Environmental.Weakness = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
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
                    Environmental.Strength = DataContext.EnvironmentsScannings.Where(x => x.Id == request.EnviId).FirstOrDefault();
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

    }
}
