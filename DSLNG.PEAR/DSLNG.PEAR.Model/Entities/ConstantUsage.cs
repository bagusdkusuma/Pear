

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class ConstantUsage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Role { get; set; }
        public string Group { get; set; }
        public IList<CalculatorConstant> Constants { get; set; }
    }
}
