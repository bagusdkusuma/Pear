

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.CalculatorConstant
{
    public class GetCalculatorConstantsResponse
    {
        public IList<CalculatorConstantResponse> CalculatorConstants { get; set; }
        public int Count { get; set; }
        public class CalculatorConstantResponse {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
        }
    }
}
