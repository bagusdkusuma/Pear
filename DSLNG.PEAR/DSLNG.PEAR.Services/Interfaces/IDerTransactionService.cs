using DSLNG.PEAR.Data.Entities.Der;
using DSLNG.PEAR.Services.Requests.Der;
using DSLNG.PEAR.Services.Requests.DerTransaction;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.DerTransaction;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IDerTransactionService
    {
        GetDerLayoutItemsResponse GetDerLayoutItems(GetDerLayoutItemsRequest request);
        GetKpiInformationValuesResponse GetKpiInformationValues(GetKpiInformationValuesRequest request);
        GetHighlightValuesResponse GetHighlightValues(GetHighlightValuesRequest request);
        BaseResponse CreateDerInputFile(CreateDerInputFileRequest request);
        GetDerInputFilesResponse GetDerInputFiles(GetDerInputFilesRequest request);
        DeleteDerInputFileResponse DeleteDerInputFile(DerDeleteRequest request);
    }
}
