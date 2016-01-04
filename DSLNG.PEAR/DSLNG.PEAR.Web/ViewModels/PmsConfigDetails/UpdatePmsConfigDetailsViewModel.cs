using System.Collections.Generic;
using System.Web.Mvc;
using DSLNG.PEAR.Web.ViewModels.Common.PmsSummary;

namespace DSLNG.PEAR.Web.ViewModels.PmsConfigDetails
{
    public class UpdatePmsConfigDetailsViewModel
    {
        public int Id { get; set; }
        public int PmsConfigId { get; set; }
        public int PillarId { get; set; }
        public string PillarName { get; set; }
        public int KpiId { get; set; }
        public Kpi Kpi { get; set; }
        public string KpiName { get; set; }
        public double Weight { get; set; }
        public string ScoringType { get; set; }
        public string Target { get; set; }
        public IEnumerable<SelectListItem> ScoringTypes { get; set; }
        public IEnumerable<ScoreIndicatorViewModel> ScoreIndicators { get; set; }
        public IEnumerable<SelectListItem> Targets { get; set; }
        public IEnumerable<SelectListItem> Kpis { get; set; }
    }

    public class Kpi
    {
        public int Id { get; set; }
        public Pillar Pillar { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Remark { get; set; }
    }

    public class Pillar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Order { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }
}