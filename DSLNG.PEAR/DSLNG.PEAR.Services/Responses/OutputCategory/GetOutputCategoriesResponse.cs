using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.OutputCategory
{
    public class GetOutputCategoriesResponse
    {

        public IList<OutputCategory> OutputCategories { get; set;  }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class OutputCategory
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
