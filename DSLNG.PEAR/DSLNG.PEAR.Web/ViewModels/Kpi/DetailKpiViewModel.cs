using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Kpi
{
    public class DetailKpiViewModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "KPI Code")]
        public string Code { get; set; }

        [Display(Name = "KPI Name")]
        public string Name { get; set; }

        [Display(Name = "Level")]
        public string Level { get; set; }

        [Display(Name = "Accountability")]
        public string RoleGroup { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Group")]
        public string Group { get; set; }

        [Display(Name = "Ordering")]
        public string Order { get; set; }
        
        [Display(Name = "Is Economic Model")]
        public string IsEconomic { get; set; }

        [Display(Name = "Measurement")]
        public string Measurement { get; set; }

        public string Method { get; set; }

        public string Periode { get; set; }

        public string Remark { get; set; }

        public ICollection<KpiRelationModel> RelationModels { get; set; }

        public string Icon { get; set; }

        public string Pillar { get; set; }

        [Display(Name = "Active")]
        public string IsActive { get; set; }

         [Display(Name = "YTD Formula")]
        public string YtdFormula { get; set; }
    }
}