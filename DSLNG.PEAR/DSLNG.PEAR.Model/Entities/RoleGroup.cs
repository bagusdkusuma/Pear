using DSLNG.PEAR.Data.Entities.KpiTransformationEngine;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities
{
    public class RoleGroup
    {
        public RoleGroup()
        {
            Menus = new HashSet<Menu>();
            SelectOptions = new List<SelectOption>();
            //RolePrivileges = new HashSet<RolePrivilege>();
            //FileManagerRolePrivileges = new List<FileManagerRolePrivilege>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public Level Level { get; set; }
        public int? LevelId { get; set; }
        public string Icon { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; }
        public ICollection<Menu> Menus { get; set; }
        public ICollection<SelectOption> SelectOptions { get; set; }
        public ICollection<StaticHighlightPrivilege> StaticHighlightPrivileges { get; set; }
        public virtual ICollection<FileManagerRolePrivilege> FileManagerRolePrivileges { get; set; }
        public virtual ICollection<RolePrivilege> RolePrivileges { get; set; }
        public ICollection<KpiTransformation> KpiTransformations { get; set; }
    }
}
