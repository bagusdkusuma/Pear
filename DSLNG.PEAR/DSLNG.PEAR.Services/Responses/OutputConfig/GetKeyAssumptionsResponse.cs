

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.OutputConfig
{
    public class GetKeyAssumptionsResponse
    {
        public IList<KeyAssumption> KeyAssumptions { get; set; }
        public class KeyAssumption {
            public int id { get; set; }
            public string Name { get; set; }
        }
    }
}
