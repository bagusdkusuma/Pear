using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.EconomicModel
{
    public class KeyAssumptionData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Scenario Scenario { get; set; }
        public KeyAssumptionConfig KeyAssumptionConfig { get; set; }
        public string ActualValue { get; set; }
        public string ForecastValue { get; set; }
        public string Remark { get; set;  }
    }
}
