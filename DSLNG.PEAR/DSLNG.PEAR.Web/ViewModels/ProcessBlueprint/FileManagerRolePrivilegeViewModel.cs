using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.ProcessBlueprint
{
    public class FileManagerRolePrivilegeViewModel
    {
        [Key,Column(Order=0)]
        public int RoleGroupId { get; set; }
        [Key, Column(Order = 1)]
        public int FileId { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowMove { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowRename { get; set; }
        public bool AllowCopy { get; set; }
        public bool AllowDownload { get; set; }
        public bool AllowUpload { get; set; }
        public bool AllowBrowse { get; set; }
        public string RoleGroupName { get; set; }
        public BlueprintFile ProcessBlueprint { get; set; }
        public class BlueprintFile
        {
            public int Id { get; set; }

            public int ParentId { get; set; }

            public string Name { get; set; }

            public bool IsFolder { get; set; }

            public byte[] Data { get; set; }

            public DateTime? LastWriteTime { get; set; }
        }
    }
}