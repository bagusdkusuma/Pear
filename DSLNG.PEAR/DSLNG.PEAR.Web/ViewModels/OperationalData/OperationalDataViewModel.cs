using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DSLNG.PEAR.Web.ViewModels.OperationalData
{
    public class OperationalDataViewModel
    {
        public OperationalDataViewModel()
        {
            KeyOperations = new List<SelectListItem>();
            KPIS = new List<SelectListItem>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name="Key Operation")]
        public int IdKeyOperation { get; set; }
        public IList<SelectListItem> KeyOperations { get; set; }
        [Required]
        [Display(Name = "KPI")]
        public int IdKPI { get; set;  }
        public IList<SelectListItem> KPIS { get; set; }
        [Display(Name = "Actual Value")]
        public double ActualValue { get; set; }
        [Display(Name = "ForecastValue")]
        public double ForecastValue { get; set; }
        public string Remark { get; set; }
    }
}