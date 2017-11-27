
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services.Requests.Kpi
{
    public class GetKpiToSeriesRequest
    {
        public string Term { get; set; }
        public DSLNG.PEAR.Data.Enums.PeriodeType? PeriodeType { get; set; }
        public int MeasurementId { get; set; }
    }
}
