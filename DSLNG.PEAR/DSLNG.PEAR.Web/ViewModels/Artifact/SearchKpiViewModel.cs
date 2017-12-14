
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Web.ViewModels.Artifact
{
    public class SearchKpiViewModel
    {
        public string Term { get; set; }
        public PeriodeType? PeriodeType { get; set; }
        public int MeasurementId { get; set; }
    }
}