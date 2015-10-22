
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class NLSHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //public NextLoadingSchedule NextLoadingSchedule { get; set; }
        public string VesselName { get; set; }
        public string BuyerName { get; set; }
        public DateTime ETA { get; set; }
        public DateTime ETD { get; set; }
        public string Remark { get; set; }
    }
}
