using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.KpiTarget
{
    public class UpdateKpiTargetItemRequest
    {
        /*public int Id { get; set; }
        public int KpiId { get; set; }
        public double? Value { get; set; }
        public DateTime Periode { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public string Remark { get; set; }

        public bool IsActive { get; set; }*/

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
        public PeriodeType PeriodeType { get; set; }
        public int UserId { get; set; }
    }
}
