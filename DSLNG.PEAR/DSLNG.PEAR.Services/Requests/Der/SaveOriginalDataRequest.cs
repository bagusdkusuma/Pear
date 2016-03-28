using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Der
{
    public class SaveOriginalDataRequest
    {
        public SaveOriginalDataRequest()
        {
            OriginalData = new List<OriginalDataRequest>();
        }

        public IList<OriginalDataRequest> OriginalData { get; set; }

        public class OriginalDataRequest
        {
            public int Id { get; set; }
            public int LayoutItemId { get; set; }
            public string Data { get; set; }
            public string DataType { get; set; }
            public DateTime Periode { get; set; }
            public string PeriodeType { get; set; }
            public string Type { get; set; }
            public int Position { get; set; }
            public bool IsKpiAchievement { get; set; }
            public int KpiId { get; set; }
        }
    }

}
