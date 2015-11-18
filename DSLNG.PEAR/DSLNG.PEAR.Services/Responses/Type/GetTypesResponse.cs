using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Type
{
    public class GetTypesResponse : BaseResponse
    {
        public IList<Type> Types { get; set; }
        public int TotalRecords { get; set; }
        public class Type
        {
            public int Id { get; set; }

            public string Name { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }
    }

    
}
