using DSLNG.PEAR.Services.Requests.CustomFormula;
using DSLNG.PEAR.Services.Responses.CustomFormula;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface ICustomFormulaService
    {
        GetCustomFormulaResponse GetFeedGasGSA_JOB(GetFeedGasGSARequest request);
        GetCustomFormulaResponse GetFeedGasGSA_MGDP(GetFeedGasGSARequest request);
        GetCustomFormulaResponse GetLNGPriceSPA_FOB(GetFeedGasGSARequest request);
        GetCustomFormulaResponse GetLNGPriceSPA_DES(GetLNGPriceSpaRequest request);
    }
}
