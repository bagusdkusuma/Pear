

using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Services.Responses.OutputConfig;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IOutputConfigService
    {
        GetKpisResponse GetKpis(GetKpisRequest request);
        GetKeyAssumptionsResponse GetKeyAssumptions(GetKeyAssumptionsRequest request);
        GetOutputConfigsResponse GetOutputConfigs(GetOutputConfigsRequest request);
        //SaveOutputConfigResponse Save(SaveOutputConfigRequest request);
    }
}
