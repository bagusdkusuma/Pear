using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities
{
    public class Select
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public IList<SelectOption> Options { get; set; }
        public bool IsActive { get; set; }
    }
}
