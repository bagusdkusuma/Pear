using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.AssumptionConfig
{
    public class GetAssumptionConfigCategoryResponse
    {
        public IList<AssumptionConfigCategoryResponse> AssumptionConfigCategoriesResponse { get; set;}
        public class AssumptionConfigCategoryResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            //public string Category { get; set; }
            public bool IsActive { get; set; }
        }

        public IList<MeasurementSelectList> MeasurementsSelectList { get; set; }
        public class MeasurementSelectList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

    }
}
