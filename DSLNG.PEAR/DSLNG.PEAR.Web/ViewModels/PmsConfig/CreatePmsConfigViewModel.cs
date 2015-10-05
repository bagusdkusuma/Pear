using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Common.PmsSummary;

namespace DSLNG.PEAR.Web.ViewModels.PmsConfig
{
    public class CreatePmsConfigViewModel
    {
        public CreatePmsConfigViewModel()
        {
            ScoreIndicators = new List<ScoreIndicatorViewModel>()
                {
                    new ScoreIndicatorViewModel()
                };
        }
        public IEnumerable<SelectListItem> Pillars { get; set; }
        [Required]
        public int PillarId { get; set; }
        /*public IEnumerable<SelectListItem> Kpis { get; set; }
        public int KpiId { get; set; }*/
        [Required]
        public double Weight { get; set; }
        [Required]
        public string ScoringType { get; set; }
        public IEnumerable<SelectListItem> ScoringTypes { get; set; }
        public IList<ScoreIndicatorViewModel> ScoreIndicators { get; set; }
        public int PmsSummaryId { get; set; }
        //public string Color { get; set; }
    }
}