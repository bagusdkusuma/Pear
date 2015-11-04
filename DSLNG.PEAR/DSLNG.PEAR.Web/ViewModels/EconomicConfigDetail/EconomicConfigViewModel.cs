using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.EconomicConfigDetail
{
    public class EconomicConfigViewModel
    {
        public EconomicConfigViewModel()
        {
            Scenarios = new List<SelectListItem>();
            EconomicSummaries = new List<SelectListItem>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name="Scenario")]
        public int IdScenario { get; set; }
        public IList<SelectListItem> Scenarios { get; set; }
        [Required]
        [Display(Name = "Economic Summary")]
        public int IdEconomicSummary { get; set; }
        public IList<SelectListItem> EconomicSummaries { get; set; }
        public bool IsActive { get; set; }
    }
}