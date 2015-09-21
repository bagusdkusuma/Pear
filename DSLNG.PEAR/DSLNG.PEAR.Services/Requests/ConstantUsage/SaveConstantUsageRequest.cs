

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Requests.ConstantUsage
{
    public class SaveConstantUsageRequest
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }
        public IList<int> CalculatorConstantIds { get; set; }
    }
}
