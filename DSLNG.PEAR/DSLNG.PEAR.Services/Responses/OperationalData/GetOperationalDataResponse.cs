using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.OperationalData
{
    public class GetOperationalDataResponse
    {
        public int Id { get; set; }
        public int IdKeyOperation { get; set; }
        public int IdKPI { get; set; }
        public double ActualValue { get; set; }
        public double ForecastValue { get; set; }
        public string Remark { get; set; }
    }
}
