using DSLNG.PEAR.Services.Requests.DerTransaction;
using DSLNG.PEAR.Services.Responses.DerTransaction;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IDerTransactionService
    {
        GetDerLayoutItemsResponse GetDerLayoutItems(GetDerLayoutItemsRequest request);
        GetKpiInformationValuesResponse GetKpiInformationValues(GetKpiInformationValuesRequest request);
        GetHighlightValuesResponse GetHighlightValues(GetHighlightValuesRequest request);
    }
}
