

using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace DSLNG.PEAR.Web.ViewModels.VesselSchedule
{
    public class VesselScheduleViewModel
    {
        public VesselScheduleViewModel()
        {
            Types = new List<SelectListItem>{
                new SelectListItem{Value = "FOB", Text="FOB"},
                new SelectListItem{Value = "DES", Text= "DES"}
            };
            //SalesTypes = new List<SelectListItem>{
            //    new SelectListItem{Value = "SPA", Text="SPA"},
            //    new SelectListItem{Value = "Spot Market", Text= "Spot Market"}
            //};
        }
        public int Id { get; set; }
        public int VesselId { get; set; }
        public string VesselName { get; set; }
        public IList<SelectListItem> Types { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public bool IsActive { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public IList<SelectListItem> SalesTypes { get; set; }
        public string Location { get; set; }
        public string SalesType { get; set; }
        public string Type { get; set; }
    }
}