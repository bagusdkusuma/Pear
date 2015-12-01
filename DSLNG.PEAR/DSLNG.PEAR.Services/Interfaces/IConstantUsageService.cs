

using DSLNG.PEAR.Services.Requests.ConstantUsage;
using DSLNG.PEAR.Services.Responses.ConstantUsage;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IConstantUsageService
    {
        GetConstantUsagesResponse GetConstantUsages(GetConstantUsagesRequest request);
        GetConstantUsagesResponse GetConstantUsagesForGrid(GetConstantUsagesRequest request);
        GetConstantUsageResponse GetConstantUsage(GetConstantUsageRequest request);
        SaveConstantUsageResponse SaveConstantUsage(SaveConstantUsageRequest request);
        DeleteConstantUsageResponse DeleteConstantUsage(DeleteConstantUsageRequest request);
    }
}
