using DSLNG.PEAR.Services.Requests.InputData;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IInputDataService
    {
        GetInputDatasResponse GetInputData(GetInputDatasRequest request);
        GetInputDataResponse GetInputData(int Id);
        SaveOrUpdateInputDataResponse SaveOrUpdateInputData(SaveOrUpdateInputDataRequest request);
        GetInputDatasResponse GetInputDatas();
        BaseResponse Delete(int id);
        BaseResponse Delete(DeleteInputDataRequest request);
    }
}
