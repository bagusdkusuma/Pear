
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Vessel
{
    public class VesselViewModel
    {
        public VesselViewModel() {
            this.Types = new List<SelectListItem>
            {
                new SelectListItem{Text = "CDS", Value="CDS"},
                new SelectListItem{Text = "LNG", Value="LNG"}
            };
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public double Capacity { get; set; }
        [Display(Name="Cargo Type")]
        public string Type { get; set; }
        public IList<SelectListItem> Types { get; set; }
        [Display(Name="Measurement")]
        public int MeasurementId { get; set; }
        public string Measurement { get; set; }
        public IList<SelectListItem> Measurements { get; set; }
    }
}