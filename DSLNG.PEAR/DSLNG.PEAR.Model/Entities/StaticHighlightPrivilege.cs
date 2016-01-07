

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DSLNG.PEAR.Data.Entities
{
    public class StaticHighlightPrivilege
    {
        public StaticHighlightPrivilege() {
            RoleGroups = new List<RoleGroup>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<RoleGroup> RoleGroups { get; set; }
    }
}
