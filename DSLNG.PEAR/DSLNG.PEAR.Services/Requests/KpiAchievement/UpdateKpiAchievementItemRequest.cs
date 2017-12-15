using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.KpiAchievement
{
    public class DeleteKpiAchievementRequest : BaseRequest
    {
        public int kpiId { get; set; }
        public DateTime periode { get; set; }
        public PeriodeType periodeType { get; set; }
    }
    public class UpdateKpiAchievementItemRequest : BaseRequest
    {
        public int Id { get; set; }
        public int KpiId { get; set; }
        public DateTime Periode { get; set; }
        public string Value { get; set; }
        public double? Mtd { get; set; }
        public double? Ytd { get; set; }
        public double? Itd { get; set; }
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
        //public int UserId { get; set; }
        public bool UpdateDeviation { get; set; }
        public string UpdateFrom { get; set; }
        public string ValueType { get; set; }
    }

    public class BatchUpdateKpiAchievementRequest : BaseRequest
    {
        public BatchUpdateKpiAchievementRequest()
        {
            BatchUpdateKpiAchievementItemRequest = new List<UpdateKpiAchievementItemRequest>();
        }
        public List<UpdateKpiAchievementItemRequest> BatchUpdateKpiAchievementItemRequest { get; set; }
    }
}
