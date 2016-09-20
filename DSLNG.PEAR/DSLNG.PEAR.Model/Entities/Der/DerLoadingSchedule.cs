

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.Der
{
    public class DerLoadingSchedule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public DateTime Period { get; set; }
        public IList<NextLoadingSchedule> NextLoadingSchedules { get; set; }
    }
}
