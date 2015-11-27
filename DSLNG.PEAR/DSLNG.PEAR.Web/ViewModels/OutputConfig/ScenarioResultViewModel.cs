
using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.OutputConfig
{
    public class ScenarioResultViewModel
    {
        public string ScenarioName { get; set; }
        public IList<OutputCategoryViewModel> OutputCategories { get; set; }
        public class OutputCategoryViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public IList<KeyOutputViewModel> KeyOutputs { get; set; }
        }
        public class KeyOutputViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Actual { get; set; }
            public string Forecast { get; set; }
            public int Order { get; set; }
            public string Measurement { get; set; }
        }
    }
}