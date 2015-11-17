

using System.Collections;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.ConstantUsage
{
    public class GetConstantUsagesResponse
    {
        public IList<ConstantUsageResponse> ConstantUsages { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class ConstantUsageResponse {
            public int Id { get; set; }
            public string Role { get; set; }
            public string Group { get; set; }
            public IList<string> StringConstants { get; set; }
            public IList<ConstantResponse> Constants { get; set; }
            public string ConstantNames { get { return string.Join(", ", StringConstants); } }
        }
        public class ConstantResponse {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Measurement { get; set; }
            public double Value { get; set; }
        }
    }
}
