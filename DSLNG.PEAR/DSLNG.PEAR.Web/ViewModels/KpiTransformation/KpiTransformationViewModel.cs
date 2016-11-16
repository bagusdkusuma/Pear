using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.KpiTransformation
{
    public class KpiTransformationViewModel
    {
        public KpiTransformationViewModel() {
            PeriodeTypes = new List<SelectListItem>();
            ProcessingTypes = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Accountability")]
        public int[] RoleGroupIds { get; set; }
        public MultiSelectList RoleGroupOptions { get; set; }
        [Display(Name="KPI")]
        public int[] KpiIds { get; set; }
        public IList<KpiViewModel> Kpis { get; set; }
        [Display(Name = "Last Processing")]
        public DateTime LastProcessing { get; set; }
        [Display(Name = "Periode Type")]
        public PeriodeType PeriodeType { get; set; }
        public IList<SelectListItem> PeriodeTypes { get; set; }
        public IList<SelectListItem> ProcessingTypes { get; set; }

        public class KpiViewModel {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}