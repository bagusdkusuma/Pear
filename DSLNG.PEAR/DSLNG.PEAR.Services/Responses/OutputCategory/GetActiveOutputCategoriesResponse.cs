
using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.OutputCategory
{
    public class GetActiveOutputCategoriesResponse
    {
        public IList<OutputCategoryResponse> OutputCategories { get; set; }
        public class OutputCategoryResponse {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public IList<KeyOutputResponse> KeyOutputs { get; set; }
        }
        public class KeyOutputResponse {
            public int Id { get; set; }
            public string Name { get; set; }
            public Formula Formula { get; set; }
            public IList<KpiResponse> Kpis { get; set; }
            public IList<KeyAssumptionResponse> KeyAssumptions { get; set; }
            public int Order { get; set; }
            public string Measurement { get; set; }
        }

        public class KpiResponse {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class KeyAssumptionResponse {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
