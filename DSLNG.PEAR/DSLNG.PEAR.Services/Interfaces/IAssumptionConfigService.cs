using DSLNG.PEAR.Services.Requests.AssumptionConfig;
using DSLNG.PEAR.Services.Responses.AssumptionConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IAssumptionConfigService
    {
        GetAssumptionConfigsResponse GetAssumptionConfigs(GetAssumptionConfigsRequest request);
        GetAssumptionConfigCategoryResponse GetAssumptionConfigCategories();
        SaveAssumptionConfigResponse SaveAssumptionConfig(SaveAssumptionConfigRequest request);
        GetAssumptionConfigResponse GetAssumptionConfig(GetAssumptionConfigRequest request);
        DeleteAssumptionConfigResponse DeleteAssumptionConfig(DeleteAssumptionConfigRequest request);
    }
}
