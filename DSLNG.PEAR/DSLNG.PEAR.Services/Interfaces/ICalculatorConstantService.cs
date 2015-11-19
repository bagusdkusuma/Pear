

using DSLNG.PEAR.Services.Requests.CalculatorConstant;
using DSLNG.PEAR.Services.Responses.CalculatorConstant;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface ICalculatorConstantService
    {
        GetCalculatorConstantsResponse GetCalculatorConstants(GetCalculatorConstantsRequest request);
        GetCalculatorConstantResponse GetCalculatorConstant(GetCalculatorConstantRequest request);
        SaveCalculatorConstantResponse SaveCalculatorConstant(SaveCalculatorConstantRequest request);
        DeleteCalculatorConstantResponse DeleteCalculatorConstant(DeleteCalculatorConstantRequest request);
        GetCalculatorConstantsForGridRespone GetCalculatorConstantsForGrid(GetCalculatorConstantForGridRequest request);
    }
}
