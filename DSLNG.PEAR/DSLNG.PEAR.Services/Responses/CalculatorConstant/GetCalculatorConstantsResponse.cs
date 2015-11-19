

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.CalculatorConstant
{
    public class GetCalculatorConstantsResponse
    {
        public IList<CalculatorConstantResponse> CalculatorConstants { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class CalculatorConstantResponse {
            public int id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public double Value { get; set; }
            public string Measurement { get; set; }
        }
    }
}
