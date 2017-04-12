using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities
{
    public class MenuRolePrivilege
    {
        public MenuRolePrivilege()
        {

        }
        [Key, Column(Order = 0), ForeignKey("Menu")]
        public int Menu_Id { get; set; }
        [Key, Column(Order = 1), ForeignKey("RolePrivilege")]
        public int RolePrivilege_Id { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowUpdate { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowView { get; set; }
        public bool AllowDownload { get; set; }
        public bool AllowUpload { get; set; }
        public bool AllowPublish { get; set; }
        public bool AllowApprove { get; set; }
        public bool AllowInput { get; set; }
        public Menu Menu { get; set; }
        public RolePrivilege RolePrivilege { get; set; }
    }
}
