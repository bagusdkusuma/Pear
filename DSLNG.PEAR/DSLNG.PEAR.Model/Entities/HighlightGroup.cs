
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities
{
    public class HighlightGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public ICollection<SelectOption> Options { get; set; }
    }
}
