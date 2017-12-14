

using DSLNG.PEAR.Services.Requests.KpiTransformation;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.KpiInformation;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IKpiTransformationService
    {
        SaveKpiTransformationResponse Save(SaveKpiTransformationRequest request);
        GetKpiTransformationsResponse Get(GetKpiTransformationsRequest request);
        GetKpiTransformationResponse Get(int id);
        BaseResponse Delete(int id);
        BaseResponse Delete(DeleteTransformationRequest request);
    }
}
