
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace DSLNG.PEAR.Web.ViewModels.NLS
{
    public class NLSViewModel
    {
        public int Id { get; set; }
        [Display(Name="Vessel Schedule")]
        public int VesselScheduleId { get; set; }
        public string VesselName { get; set; }
        [Display(Name="Remark Date")]
        public DateTime? CreatedAt { get; set; }
        private string _createdAtDisplay { get; set; }
        [Required]
        [Display(Name = "Remark Date")]
        public string CreatedAtDisplay
        {
            get
            {
                if (CreatedAt.HasValue)
                {
                    return CreatedAt.Value.ToString("MM/dd/yyyy");
                }
                return this._createdAtDisplay;
            }
            set
            {
                this.CreatedAt = DateTime.ParseExact(value, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                this._createdAtDisplay = value;
            }
        }
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

        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }
    }
}