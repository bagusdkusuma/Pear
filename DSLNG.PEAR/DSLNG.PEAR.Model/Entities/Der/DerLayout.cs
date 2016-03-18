using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.Der
{
    public class DerLayout
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        [DefaultValue("false")]
        public bool IsDeleted { get; set; }
        public ICollection<DerLayoutItem> Items { get; set; }
    }
}
