using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities
{
    public class ProcessBlueprint : BaseEntity
    {
        public ProcessBlueprint()
        {
            RoleGroups = new List<RoleGroup>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public bool IsFolder { get; set; }

        public byte[] Data { get; set; }

        public DateTime? LastWriteTime { get; set; }
        public ICollection<RoleGroup> RoleGroups { get; set; }
    }
}
