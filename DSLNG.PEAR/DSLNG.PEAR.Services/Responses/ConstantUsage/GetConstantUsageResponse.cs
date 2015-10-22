
using System.Collections.Generic;

namespace DSLNG.PEAR.Services.Responses.ConstantUsage
{
    public class GetConstantUsageResponse
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }
        public IList<CalculatorConstantResponse> Constants { get; set; }

        public class CalculatorConstantResponse {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Measurement { get; set; }
            public double Value { get; set; }
        }
    }
}
