using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Menu
{
    public class UpdateMenuRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
        public string Module { get; set; }
        public bool IsActive { get; set; }
        public bool IsRoot { get; set; }
        public List<int> RoleGroupIds { get; set; }
        public int? ParentId { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
    }
}
