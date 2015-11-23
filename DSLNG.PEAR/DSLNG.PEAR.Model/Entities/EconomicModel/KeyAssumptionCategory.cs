using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.EconomicModel
{
    public class KeyAssumptionCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        //public KeyAssumptionStage KeyAssumptionStage { get; set; }
        public string Desc { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public ICollection<KeyAssumptionConfig> KeyAssumptions { get; set; }
    }
}
