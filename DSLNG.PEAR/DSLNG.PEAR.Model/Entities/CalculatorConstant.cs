

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class CalculatorConstant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Measurement { get; set; }
        public double Value { get; set; }
        public IList<ConstantUsage> ConstantUsages { get; set; }
    }
}
