

using DSLNG.PEAR.Services.Requests.NLS;
using DSLNG.PEAR.Services.Responses.NLS;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface INLSService
    {
        GetNLSResponse GetNLS(GetNLSRequest request);
        GetNLSListResponse GetNLSList(GetNLSListRequest request);
        SaveNLSResponse SaveNLS(SaveNLSRequest request);
        DeleteNLSResponse Delete(DeleteNLSRequest request);
        GetNLSListResponse GetNLSListForGrid(GetNLSListRequest request);
    }
}
