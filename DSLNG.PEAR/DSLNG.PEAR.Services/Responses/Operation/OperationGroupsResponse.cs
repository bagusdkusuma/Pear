using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Operation
{
   public class OperationGroupsResponse
    {
        public IList<OperationGroup> OperationGroups { get; set; }
        public class OperationGroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public IList<Kpi> Kpis { get; set; }
        public class Kpi
        {
           public int Id { get; set; }
           public string Name { get; set; }
        }
    }
}
