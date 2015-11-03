using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.AssumptionData
{
    public class AssumptionDataViewModel
    {
        public AssumptionDataViewModel()
        {
            Scenarios = new List<SelectListItem>();
            Configs = new List<SelectListItem>();
        }
        public IList<SelectListItem> Scenarios { get; set; }
        public int IdScenario { get; set; }
        public IList<SelectListItem> Configs { get; set; }
        public int IdConfig { get; set; }
        public double ActualValue { get; set; }
        public double ForecastValue { get; set; }
        public string Remark { get; set; }
        public int Id { get; set; }

    }
}