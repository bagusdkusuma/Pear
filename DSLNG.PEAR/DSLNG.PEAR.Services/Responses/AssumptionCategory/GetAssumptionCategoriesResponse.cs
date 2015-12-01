using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.AssumptionCategory
{
    public class GetAssumptionCategoriesResponse : BaseResponse
    {
        public IList<AssumptionCategory> AssumptionCategorys { get; set; }
        public int Count { get; set; }
        public int TotalRecords { get; set; }
        public class AssumptionCategory
        {
            public AssumptionCategory() {
                Assumptions = new List<Assumption>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public bool IsActive { get; set; }
            public int Order { get; set; }
            public IList<Assumption> Assumptions { get; set; }
        }
        public class Assumption {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public bool IsActive { get; set; }
            public string Measurement { get; set; }
        }
    }
}
