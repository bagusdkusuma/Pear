using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Periode
{
    public class GetPeriodeResponse : BaseResponse
    {
        public int Id { get; set; }
        public PeriodeType Name { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetPeriodesResponse : BaseResponse
    {
        public IList<Periode> Periodes { get; set; }
        public int TotalRecords { get; set; }
        public class Periode
        {
            public int Id { get; set; }
            public PeriodeType Name { get; set; }
            public string Remark { get; set; }
            public bool IsActive { get; set; }
        }
    }

    public class CreatePeriodeResponse : BaseResponse { }

    public class UpdatePeriodeResponse : BaseResponse { }

    public class DeletePeriodeResponse : BaseResponse { }

    public enum PeriodeType
    {
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}
