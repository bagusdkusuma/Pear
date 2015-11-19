using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.Periode
{
    public class GetPeriodeRequest
    {
        public int Id { get; set; }
    }

    public class GetPeriodesRequest : GridBaseRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        
    }

    public class CreatePeriodeRequest
    {
        public PeriodeType Name { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdatePeriodeRequest
    {
        public int Id { get; set; }
        public PeriodeType Name { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
    }


    public enum PeriodeType
    {
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}
