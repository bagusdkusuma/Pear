using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.ConstantUsage
{
    public class ConstantUsageViewModel
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }
        public IList<CalculatorConstantViewModel> Constants { get; set; }
        public class CalculatorConstantViewModel {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public double Value { get; set; }
            public string Measurement { get; set; }
        }
    }
}