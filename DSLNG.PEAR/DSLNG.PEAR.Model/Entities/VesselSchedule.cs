

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class VesselSchedule
    {
        public VesselSchedule() {
            IsActive = true;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Vessel Vessel { get; set; }
        public DateTime ETA { get; set; }
        public DateTime ETD { get; set; }
        public bool IsActive { get; set; }
        public Buyer Buyer { get; set; }
        public string Location { get; set; }
        public string SalesType { get; set; }
        public string Type { get; set; }
    }
}
