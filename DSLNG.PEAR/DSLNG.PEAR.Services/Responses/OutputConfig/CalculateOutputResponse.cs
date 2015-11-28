
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

        public class OutputResult
        {
            public string Actual { get; set; }
            public string Forecast { get; set; }
            public string ScenarioId { get; set; }
        }

        public class Scenario
        {
            public int Id { get; set; }
            public string Name   { get; set; } 
        }

        public class Group  
        {
            public int Id { get; set; }
            public string Name   { get; set; }
            public IList<KeyOutput> KeyOutputs { get; set; }

        }

        public class KeyOutput
        {
            public string Name { get; set; }
            public string Measurement { get; set; }
            public IList<OutputResult> OutPutResults { get; set; }
        }

        public class EconomicSummaryResponse
        {
            public IList<Scenario> Scenarios { get; set; }
            public IList<Group> Groups { get; set; }
            public OutputResult OutputResult { get; set; }
        }
    }
}
