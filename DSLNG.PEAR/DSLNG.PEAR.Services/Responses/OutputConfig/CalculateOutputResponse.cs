
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.OutputConfig
{
    public class CalculateOutputResponse
    {
        public CalculateOutputResponse()
        {
            OutputCategories = new List<OutputCategoryResponse>();
        }
        public IList<OutputCategoryResponse> OutputCategories { get; set; }
        public class OutputCategoryResponse
        {
            public OutputCategoryResponse() {
                KeyOutputs = new List<KeyOutputResponse>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public IList<KeyOutputResponse> KeyOutputs { get; set; }
        }
        public class KeyOutputResponse
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
