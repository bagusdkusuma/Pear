

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
            IsActive = true;
        }
        public int Id { get; set; }
        [Display(Name = "Vessel")]
        public int VesselId { get; set; }
        public string VesselName { get; set; }
        public IList<SelectListItem> Types { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Buyer")]
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public IList<SelectListItem> SalesTypes { get; set; }
        public IList<SelectListItem> Buyers { get; set; }
        public IList<SelectListItem> Vessels { get; set; }
        [DataType(DataType.MultilineText)]
        public string Location { get; set; }
        [Display(Name = "Cargo Type")]
        public string SalesType { get; set; }
        [Display(Name = "DES/FOB")]
        public string Type { get; set; }
        [Display(Name = "Cargo #")]
        public string Cargo { get; set; }
        public bool AsNew { get; set; }
        public DateTime? DerTransactionDate { get; set; }
        public string DerTransactionDateInput
        {
            get
            {
                return this.DerTransactionDate.HasValue ? this.DerTransactionDate.Value.ToString("MM/dd/yyyy") : null;
            }
            set
            {
                this.DerTransactionDate = DateTime.ParseExact(value, "MM/dd/yyyy", CultureInfo.InvariantCulture); ;
            }
        }
    }
}