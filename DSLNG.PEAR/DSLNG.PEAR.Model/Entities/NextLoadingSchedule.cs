
using DSLNG.PEAR.Data.Entities.Der;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class NextLoadingSchedule
    {
        public NextLoadingSchedule() {
            CreatedAt = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public VesselSchedule VesselSchedule { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
