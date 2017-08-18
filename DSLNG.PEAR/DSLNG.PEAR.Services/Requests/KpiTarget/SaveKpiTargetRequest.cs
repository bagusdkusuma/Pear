using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.KpiTarget
{
    public class SaveKpiTargetRequest : BaseRequest
    {
        public int Id { get; set; }
        public int KpiId { get; set; }
        public DateTime Periode { get; set; }
        public string Value { get; set; }
        public double? RealValue
        {
            get
            {
                double realValue;
                var isParsed = double.TryParse(Value, out realValue);
                return isParsed ? realValue : default(double?);
            }
        }
        public string Remark { get; set; }
        public Data.Enums.PeriodeType PeriodeType { get; set; }
        //public int UserId { get; set; }
    }
}
