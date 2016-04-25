using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.Menu
{
    public class MenuPrivilegeViewModel
    {
        public int Menu_Id { get; set; }
        public string Menu_Name { get; set; }
        public string Menu_Url { get; set; }
        public bool AllowCreate { get; set; }
        public bool AllowUpdate { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowView { get; set; }
        public bool AllowDownload { get; set; }
        public bool AllowUpload { get; set; }
        public bool AllowPublish { get; set; }
        public bool AllowApprove { get; set; }
    }
}