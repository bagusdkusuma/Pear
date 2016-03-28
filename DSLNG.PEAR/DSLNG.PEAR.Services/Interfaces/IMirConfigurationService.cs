

using DSLNG.PEAR.Services.Requests.MirConfiguration;
using DSLNG.PEAR.Services.Responses.MirConfiguration;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IMirConfigurationService
    {
        GetMirConfigurationsResponse Get(GetMirConfigurationsRequest request);
    }
}
