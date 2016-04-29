using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.FileManagerRolePrivilege
{
    public class UpdateFilePrivilegeRequest
    {
        public int ProcessBlueprint_Id { get; set; }
        public int RoleGroup_Id { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowMove { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowRename { get; set; }
        public bool AllowCopy { get; set; }
        public bool AllowDownload { get; set; }
        public bool AllowUpload { get; set; }
        public bool AllowBrowse { get; set; }
    }

    public class BatchUpdateFilePrivilegeRequest
    {
        public BatchUpdateFilePrivilegeRequest()
        {
            BatchUpdateFilePrivilege = new List<UpdateFilePrivilegeRequest>();
        }

        public IList<UpdateFilePrivilegeRequest> BatchUpdateFilePrivilege { get; set; }
    }
}
