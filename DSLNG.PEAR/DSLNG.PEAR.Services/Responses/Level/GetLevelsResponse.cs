using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Level
{
    public class GetLevelsResponse : BaseResponse
    {
        public IList<Level> Levels { get; set; }
        public int TotalRecords { get; set; }
        public class Level {
            public int Id { get; set; }

            public string Code { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
            public string Remark { get; set; }

            public bool IsActive { get; set; }
        }
    }
}
