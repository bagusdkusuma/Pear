

using System.Collections.Generic;
using System.Web.Mvc;
namespace DSLNG.PEAR.Web.ViewModels.OutputConfig
{
    public class OutputConfigViewModel
    {
        public OutputConfigViewModel() {
            Formulas = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int MeasurementId { get; set; }
        public IList<SelectListItem> Measurements { get; set; }
        public IList<SelectListItem> OutputCategories { get; set; }
        public string Formula { get; set; }
        public IList<SelectListItem> Formulas { get; set; }
        public IList<int> KpiIds { get; set; }
        public IList<int> KeyAssumptionIds { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public IList<SelectListItem> Kpis { get; set; }
        public IList<SelectListItem> KeyAssumptions { get; set; }
    }
}