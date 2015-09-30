using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Common.PmsSummary;

namespace DSLNG.PEAR.Web.ViewModels.PmsConfig
{
    public class UpdatePmsConfigViewModel
    {
        public UpdatePmsConfigViewModel()
        {
            ScoreIndicators = new List<ScoreIndicatorViewModel>()
                {
                    new ScoreIndicatorViewModel()
                };
        }

        public int Id { get; set; }
        /*public IEnumerable<SelectListItem> Pillars { get; set; }
        [Required]
        public int PillarId { get; set; }*/

        public string PillarName { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        public string ScoringType { get; set; }
        public IEnumerable<SelectListItem> ScoringTypes { get; set; }
        public IList<ScoreIndicatorViewModel> ScoreIndicators { get; set; }
        //public string Color { get; set; }
        //public int PmsSummaryId { get; set; }
    }
}