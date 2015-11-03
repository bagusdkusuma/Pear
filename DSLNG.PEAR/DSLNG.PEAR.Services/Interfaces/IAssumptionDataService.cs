using DSLNG.PEAR.Services.Requests.AssumptionData;
using DSLNG.PEAR.Services.Responses.AssumptionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IAssumptionDataService
    {
        GetAssumptionDatasResponse GetAssumptionDatas(GetAssumptionDatasRequest request);
        GetAssumptionDataConfigResponse GetAssumptionDataConfig();
        SaveAssumptionDataResponse SaveAssumptionData(SaveAssumptionDataRequest request);
        GetAssumptionDataResponse GetAssumptionData(GetAssumptionDataRequest request);
        DeleteAssumptionDataResponse DeleteAssumptionData(DeleteAssumptionDataRequest request);
    }
}
