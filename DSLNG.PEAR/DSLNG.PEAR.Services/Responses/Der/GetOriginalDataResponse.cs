using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Der
{
    public class GetOriginalDataResponse : BaseResponse
    {
        public GetOriginalDataResponse()
        {
            OriginalData = new List<OriginalDataResponse>();
        }

        public IList<OriginalDataResponse> OriginalData { get; set; } 
        public class OriginalDataResponse
        {
            public int Id { get; set; }
            public int LayoutItemId { get; set; }
            public string Data { get; set; }
            public string DataType { get; set; }
            public DateTime Periode { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public int Position { get; set; }
            public string Type { get; set; }
            public bool IsKpiAchievement { get; set; }
            public string Label { get; set; }
            public int KpiId { get; set; }
        }
    }
}
