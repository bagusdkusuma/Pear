using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Common.PmsSummary;

namespace DSLNG.PEAR.Web.ViewModels.PmsConfigDetails
{
    public class CreatePmsConfigDetailsViewModel
    {
        public CreatePmsConfigDetailsViewModel()
        {
            ScoreIndicators = new List<ScoreIndicatorViewModel>()
                {
                    new ScoreIndicatorViewModel()
                };
        }

        public int PmsConfigId { get; set; }
        public string PillarName { get; set; }
        public IEnumerable<SelectListItem> Kpis { get; set; }
        [Required]
        public int KpiId { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        public string ScoringType { get; set; }
        public double? Target { get; set; }
        public IEnumerable<SelectListItem> ScoringTypes { get; set; }
        public IList<ScoreIndicatorViewModel> ScoreIndicators { get; set; }
    }
}