using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Pillar
{
    public class GetPillarResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Oder { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetPillarsResponse : BaseResponse
    {
        public IList<Pillar> Pillars { get; set; }
        public int TotalRecords { get; set; }

        public class Pillar
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public int Order { get; set; }
            public string Color { get; set; }
            public string Icon { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
