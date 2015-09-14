using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Vessel;
using DSLNG.PEAR.Services.Responses.Vessel;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;

namespace DSLNG.PEAR.Services
{
    public class VesselService : BaseService,IVesselService
    {
        public VesselService(IDataContext dataContext) : base(dataContext) { }

        public GetVesselResponse GetVessel(GetVesselRequest request)
        {
            return DataContext.Vessels
                .Include(x => x.Measurement)
                .FirstOrDefault(x => x.Id == request.Id)
                .MapTo<GetVesselResponse>();
        }

        public GetVesselsResponse GetVessels(GetVesselsRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetVesselsResponse { Count = DataContext.Vessels.Count() };
            }
            else
            {
                return new GetVesselsResponse
                {
                    Vessels = DataContext.Vessels
                    .Include(x => x.Measurement)
                    .OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take)
                                    .ToList().MapTo<GetVesselsResponse.VesselResponse>()
                };
            }
        }

        public SaveVesselResponse SaveVessel(SaveVesselRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var vessel = request.MapTo<Vessel>();
                    var measurement = new Measurement { Id = request.MeasurementId };
                    DataContext.Measurements.Attach(measurement);
                    vessel.Measurement = measurement;
                    DataContext.Vessels.Add(vessel);
                }
                else
                {
                    var vessel = DataContext.Vessels.FirstOrDefault(x => x.Id == request.Id);
                    if (vessel != null)
                    {
                        request.MapPropertiesToInstance<Vessel>(vessel);
                        var measurement = new Measurement { Id = request.MeasurementId };
                        DataContext.Measurements.Attach(measurement);
                        vessel.Measurement = measurement;
                    }
                }
                DataContext.SaveChanges();
                return new SaveVesselResponse
                {
                    IsSuccess = true,
                    Message = "Highlight has been saved"
                };
            }
            catch (InvalidOperationException e)
            {
                return new SaveVesselResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
    }
}
