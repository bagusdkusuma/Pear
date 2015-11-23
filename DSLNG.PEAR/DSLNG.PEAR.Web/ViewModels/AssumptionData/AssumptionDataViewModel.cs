using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        [Display(Name="Scenario")]
        public int IdScenario { get; set; }
        public IList<SelectListItem> Configs { get; set; }
        [Required]
        [Display(Name = "Key Assumption")]
        public int IdConfig { get; set; }
        [Display(Name = "Actual Value")]
        public string ActualValue { get; set; }
        [Display(Name = "Forecast Value")]
        public string ForecastValue { get; set; }
        public string Remark { get; set; }
        public int Id { get; set; }
        public string Measurement { get; set; }
    }
}