using DSLNG.PEAR.Services.Requests.InputData;
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
        SaveOrUpdateResponse SaveOrUpdateInputData(SaveOrUpdateInputDataRequest request);
    }
}
