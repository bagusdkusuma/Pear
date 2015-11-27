

using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Services.Responses.OutputConfig;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IOutputConfigService
    {
        GetKpisResponse GetKpis(GetKpisRequest request);
        GetKeyAssumptionsResponse GetKeyAssumptions(GetKeyAssumptionsRequest request);
        SaveOutputConfigResponse Save(SaveOutputConfigRequest request);
        GetOutputConfigResponse Get(GetOutputConfigRequest request);
        GetOutputConfigsResponse GetOutputConfigs(GetOutputConfigsRequest request);
        CalculateOutputResponse CalculateOputput(CalculateOutputRequest request);
        DeleteOutputConfigResponse DeleteOutput(DeleteOutputConfigRequest request);
    }
}
