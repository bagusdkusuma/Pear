using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.CustomFormula;
using DSLNG.PEAR.Services.Responses.CustomFormula;
using System;

namespace DSLNG.PEAR.Services
{
    public class CustomFormulaService : ICustomFormulaService
    {
        public GetCustomFormulaResponse GetFeedGasGSA_JOB(GetFeedGasGSARequest request)
        {
            var response = new GetCustomFormulaResponse();
            try
            {
                response.Value = ((0.154 * Math.Min(request.JccPrice, 45) - 1.1044) * 0.982 * 0.88 * 0.50) + (request.JccPrice > 45 ? 0.154 * (request.JccPrice - 45) * 0.982 * 0.88 * 0.9 : 0);// (IF(K62 > 45, 0.154 * ((K62 - 45)) * 98.2 % *88 % *90 %, 0));
                response.IsSuccess = true;
            }
            catch (Exception e)
            {

                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }

        public GetCustomFormulaResponse GetFeedGasGSA_MGDP(GetFeedGasGSARequest request)
        {
            var response = new GetCustomFormulaResponse();
            try
            {
                response.Value = ((0.154 * Math.Min(request.JccPrice, 45) + 0.4) * 0.982 * 0.88 * 0.50) + (request.JccPrice > 45 ? 0.154 * (request.JccPrice - 45) * 0.982 * 0.88 * 0.9 : 0);// (IF(K62 > 45, 0.154 * ((K62 - 45)) * 98.2 % *88 % *90 %, 0));
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }

        public GetCustomFormulaResponse GetLNGPriceSPA_DES(GetLNGPriceSpaRequest request)
        {
            var response = new GetCustomFormulaResponse();
            try
            {
                response.Value = request.JccPrice > 30 ? Math.Round(((0.154 * request.JccPrice) - 0.1) + (0.52 + (0.00023 * (request.BunkerPrice - 575))), 2) : Math.Round(((0.154 * 30) - 0.1) + (0.52 + (0.00023 * (request.BunkerPrice - 575))), 2);
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }

        public GetCustomFormulaResponse GetLNGPriceSPA_FOB(GetFeedGasGSARequest request)
        {
            var response = new GetCustomFormulaResponse();
            try
            {
                response.Value = request.JccPrice > 30 ? Math.Round((0.1475 * request.JccPrice) + ((0.0065 * request.JccPrice) - 0.1), 2) : Math.Round((0.1475 * 30) + ((0.0065 * 30) - 0.1), 2);
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }
    }
}
