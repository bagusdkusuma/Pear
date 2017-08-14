using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.RoleGroup
{
    public class CreateRoleGroupRequest : BaseRequest
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public int LevelId { get; set; }
    }
}
