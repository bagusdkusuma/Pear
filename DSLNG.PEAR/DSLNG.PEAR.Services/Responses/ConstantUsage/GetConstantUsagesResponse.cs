

using System.Collections;
using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.ConstantUsage
{
    public class GetConstantUsagesResponse
    {
        public IList<ConstantUsageResponse> ConstantUsages { get; set; }
        public int Count { get; set; }
        public class ConstantUsageResponse {
            public int Id { get; set; }
            public string Role { get; set; }
            public string Group { get; set; }
            public IList<string> Constants { get; set; }
            public string ConstantNames { get { return string.Join(", ", Constants); } }
        }
    }
}
