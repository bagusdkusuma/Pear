using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities.EconomicModel
{
    public class KeyAssumptionConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //public KeyAssumptionStage Stage { get; set; }
        public KeyAssumptionCategory Category { get; set; }
        public string Name { get; set; }
        public Measurement Measurement { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public ICollection<KeyOutputConfiguration> KeyOutputConfigurations { get; set; }
    }
}
