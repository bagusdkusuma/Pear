using DSLNG.PEAR.Services.Requests.Vessel;
using DSLNG.PEAR.Services.Responses.Vessel;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IVesselService
    {
        GetVesselResponse GetVessel(GetVesselRequest request);
        GetVesselsResponse GetVessels(GetVesselsRequest request);
        SaveVesselResponse SaveVessel(SaveVesselRequest request);
        DeleteVesselResponse Delete(DeleteVesselRequest request);
        GetVesselsResponse GetVesselsForGrid(GetVesselsRequest request);
    }
}
