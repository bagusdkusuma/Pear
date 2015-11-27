using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.EconomicModel
{
    public class EconomicSummary
    {
        public EconomicSummary()
        {
            Scenarios = new List<Scenario>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Scenario> Scenarios { get; set; }
        public string Desc { get; set; }
        public bool IsActive { get; set; }
    }
}
