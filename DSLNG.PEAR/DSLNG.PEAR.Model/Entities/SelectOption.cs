using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities
{
    public class SelectOption
    {
        public SelectOption() {
            IsActive = true;
            RoleGroups = new List<RoleGroup>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public Select Select { get; set; }
        public HighlightGroup Group { get; set; }
        public int Order { get; set; }
        public ICollection<RoleGroup> RoleGroups { get; set; }
        public bool IsActive { get; set; }
        public bool IsDashboard { get; set; }
        public bool IsDer { get; set; }
    }
}
