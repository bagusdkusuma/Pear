using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.EconomicModel
{
    public class KeyOperationConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public KeyOperationGroup KeyOperationGroup { get; set; }
        public Kpi Kpi { get; set; }
        public string Desc { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
}
