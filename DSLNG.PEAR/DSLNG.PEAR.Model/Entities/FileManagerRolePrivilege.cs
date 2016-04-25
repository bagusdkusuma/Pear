using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Data.Entities
{
    public class FileManagerRolePrivilege
    {
        public FileManagerRolePrivilege()
        {
        }

        [Key, Column(Order = 1), ForeignKey("ProcessBlueprint")]
        public int ProcessBlueprint_Id { get; set; }

        [Key, Column(Order = 2), ForeignKey("RoleGroup")]
        public int RoleGroup_Id { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowMove { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowRename { get; set; }
        public bool AllowCopy { get; set; }
        public bool AllowDownload { get; set; }
        public bool AllowUpload { get; set; }
        public bool AllowBrowse { get; set; }


        public ProcessBlueprint ProcessBlueprint { get; set; }
        public RoleGroup RoleGroup { get; set; }
    }
}
