using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class AnnualBusinessPlanViewModel
    {
        [Display(Name="Planning Blueprint")]
        public int PlanningBlueprintId { get; set; }
        public IList<SelectListItem> PlanningBlueprints { get; set; }
    }
}