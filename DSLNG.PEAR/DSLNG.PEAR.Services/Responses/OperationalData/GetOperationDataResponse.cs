using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.OperationalData
{
    public class GetOperationDataResponse : BaseResponse
    {
        public int Id { get; set; }
        public Kpi kpi { get; set; }
        public double? Value { get; set; }
        public DateTime Periode { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public string Remark { get; set; }

        public bool IsActive { get; set; }

        public class Kpi
        {

            public int Id { get; set; }
            public string Name { get; set; }
            public string Measurement { get; set; }
            public string Remark { get; set; }

        }
    }
}
