using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.ProcessBlueprint
{
    public class GetProcessBlueprintPrivilegeRequest
    {
        public int RoleGroupId { get; set; }
        public int FileId { get; set; }
    }
}
