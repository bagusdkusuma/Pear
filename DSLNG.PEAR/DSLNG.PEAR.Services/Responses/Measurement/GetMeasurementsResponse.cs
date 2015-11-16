using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Measurement
{
    public class GetMeasurementsResponse : BaseResponse
    {
        public IList<Measurement> Measurements { get; set; }
        public int TotalRecords { get; set; }
        public class Measurement
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string IsActive { get; set; }
            public string Remark { get; set; }
        }
    }
}
