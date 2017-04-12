using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Menu
{
    public class GetMenuPrivilegeResponse : BaseResponse
    {
        public int Menu_Id { get; set; }
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
        public MenuPrivilege Menu { get; set; }
        public Privilege RolePrivilege { get; set; }

        public class Privilege
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class MenuPrivilege
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
