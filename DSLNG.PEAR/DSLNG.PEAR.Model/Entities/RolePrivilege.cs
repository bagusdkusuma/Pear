using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities
{
    public class RolePrivilege : BaseEntity
    {
        public RolePrivilege()
        {
            Users = new HashSet<User>();
            //MenuRolePrivileges = new List<MenuRolePrivilege>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("RoleGroup")]
        public int RoleGroup_Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Descriptions { get; set; }
        public RoleGroup RoleGroup { get; set; }
        public ICollection<User> Users { get; set; }
        public virtual ICollection<MenuRolePrivilege> MenuRolePrivileges { get; set; }
    }
}
