using DSLNG.PEAR.Services.Requests.Operation;
using DSLNG.PEAR.Services.Responses.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IOperationConfigService
    {
        GetOperationsResponse GetOperations(GetOperationsRequest request);
        OperationGroupsResponse GetOperationGroups();
        SaveOperationResponse SaveOperation(SaveOperationRequest request);
        GetOperationResponse GetOperation(GetOperationRequest request);
        DeleteOperationResponse DeleteOperation(DeleteOperationRequest request);
        UpdateOperationResponse UpdateOperation(UpdateOperationRequest request);
        GetOperationsInResponse GetOperationIn(GetOperationsInRequest request);
    }
}
