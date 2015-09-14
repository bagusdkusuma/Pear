
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.Vessel
{
    public class VesselViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Capacity { get; set; }
        public string Type { get; set; }
        [Display(Name="Measurement")]
        public int MeasurementId { get; set; }
        public IList<SelectListItem> Measurements { get; set; }
    }
}