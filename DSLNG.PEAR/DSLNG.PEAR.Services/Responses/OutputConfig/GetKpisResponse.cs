

using System.Collections.Generic;
namespace DSLNG.PEAR.Services.Responses.OutputConfig
{
    public class GetKpisResponse
    {
        public IList<Kpi> KpiList { get; set; }

        public class Kpi
        {
            public int id { get; set; }
            public string Name { get; set; }
        }
    }
}
