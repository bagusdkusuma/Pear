using DSLNG.PEAR.Services.Requests.OperationalData;
using DSLNG.PEAR.Services.Responses.OperationalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IOperationDataService
    {
        GetOperationalDatasResponse GetOperationalDatas(GetOperationalDatasRequest request);
        GetOperationalSelectListResponse GetOperationalSelectList();
        SaveOperationalDataResponse SaveOperationalData(SaveOperationalDataRequest request);
        GetOperationalDataResponse GetOperationalData(GetOperationalDataRequest request);
        DeleteOperationalDataResponse DeleteOperationalData(DeleteOperationalDataRequest request);
        GetOperationalDataDetailResponse GetOperationalDataDetail(GetOperationalDataDetailRequest request);
        GetOperationDataConfigurationResponse GetOperationDataConfiguration(GetOperationDataConfigurationRequest request);
        UpdateOperationDataResponse Update(UpdateOperationDataRequest request);
    }
}
