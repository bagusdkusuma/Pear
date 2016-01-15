using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Entities;

namespace DSLNG.PEAR.Services.Responses.Menu
{
    public class GetSiteMenuActiveResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public ICollection<Menu> Menus { get; set; }
        public int Order { get; set; }
        public bool IsRoot { get; set; }
        public ICollection<RoleGroup> RoleGroups { get; set; }
        public string Remark { get; set; }
        public string Module { get; set; }
        //public Menu Parent { get; set; }
        public int ParentId { get; set; }
        public bool IsActive { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public Data.Entities.Menu SelectedMenu { get; set; }
        public List<int> HistoryMenu { get; set; } 
    }
}
