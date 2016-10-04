

using DSLNG.PEAR.Data.Entities.Der;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class VesselSchedule
    {
        public VesselSchedule() {
            IsActive = true;
            NextLoadingSchedules = new List<NextLoadingSchedule>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public Vessel Vessel { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public bool IsActive { get; set; }
        public Buyer Buyer { get; set; }
        public string Location { get; set; }
        public string SalesType { get; set; }
        public string Type { get; set; }
        public string Cargo { get; set; }
        public ICollection<NextLoadingSchedule> NextLoadingSchedules { get; set; }
        public IList<DerLoadingSchedule> DerLoadingSchedules { get; set; }
    }
}
