using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Group
{
    public class GetGroupsResponse : BaseResponse
    {
        public IList<Group> Groups { get; set; }
        public int TotalRecords { get; set; }
        public class Group
        {
            public int Id { get; set; }

            public string Name { get; set; }
            public int? Order { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
