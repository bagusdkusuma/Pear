using DSLNG.PEAR.Services.Requests.EconomicConfig;
using DSLNG.PEAR.Services.Responses.EconomicConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IEconomicConfigService
    {
        GetEconomicConfigsResponse GetEconomicConfigs(GetEconomicConfigsRequest request);
        GetEconomicConfigSelectListResponse GetEconomicConfigSelectList();
        SaveEconomicConfigResponse SaveEconomicConfig(SaveEconomicConfigRequest request);
        GetEconomicConfigResponse GetEconomicConfig(GetEconomicConfigRequest request);
        DeleteEconomicConfigResponse DeleteEconomicConfig(DeleteEconomicConfigRequest request);
    }
}
