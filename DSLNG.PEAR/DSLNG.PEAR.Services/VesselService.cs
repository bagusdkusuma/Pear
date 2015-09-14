using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Vessel;
using DSLNG.PEAR.Services.Responses.Vessel;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Data.Persistence;

namespace DSLNG.PEAR.Services
{
    public class VesselService : BaseService,IVesselService
    {
        public VesselService(IDataContext dataContext) : base(dataContext) { }

        public GetVesselResponse GetVessel(GetVesselRequest request)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
    }
}
