using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.PlanningBlueprint
{
    public class GetESCategoriesResponse
    {
        public int TotalRecords { get; set; }
        public IList<ESCategory> ESCategories { get; set; }
        public class ESCategory
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsActive { get; set; }
            public EnvirontmentType Type { get; set; }
        }
    }
}
