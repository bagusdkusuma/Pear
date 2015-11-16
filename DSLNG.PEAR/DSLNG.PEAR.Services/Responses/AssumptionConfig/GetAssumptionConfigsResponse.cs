using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.AssumptionConfig
{
    public class GetAssumptionConfigsResponse
    {
        public IList<AssumptionConfig> AssumptionConfigs { get; set; }
        public int Count { get; set; }
        public int TotalRecords { get; set; }
        public class AssumptionConfig
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Measurement { get; set; }
            public int Order { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
