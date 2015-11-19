

using DSLNG.PEAR.Services.Requests.Buyer;
using DSLNG.PEAR.Services.Responses.Buyer;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IBuyerService
    {
        GetBuyerResponse GetBuyer(GetBuyerRequest request);
        GetBuyersResponse GetBuyers(GetBuyersRequest request);
        SaveBuyerResponse SaveBuyer(SaveBuyerRequest request);
        DeleteBuyerResponse Delete(DeleteBuyerRequest request);
        GetBuyerForGridResponse GetBuyersForGrid(GetBuyerForGridRequest request);
    }
}
