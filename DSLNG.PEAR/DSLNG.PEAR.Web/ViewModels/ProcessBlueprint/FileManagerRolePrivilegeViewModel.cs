using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.ProcessBlueprint
{
    public class FileManagerRolePrivilegeViewModel
    {
        public int RoleGroupId { get; set; }
        public int FileId { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowMove { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowRename { get; set; }
        public bool AllowCopy { get; set; }
        public bool AllowDownload { get; set; }
        public bool AllowUpload { get; set; }
        public bool AllowBrowse { get; set; }
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