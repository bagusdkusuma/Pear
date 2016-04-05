using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.KpiAchievement
{
    public class UpdateKpiAchievementItemRequest
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
        public PeriodeType PeriodeType { get; set; }
        public int UserId { get; set; }
    }

    public class BatchUpdateKpiAchievementRequest
    {
        public BatchUpdateKpiAchievementRequest()
        {
            BatchUpdateKpiAchievementItemRequest = new List<UpdateKpiAchievementItemRequest>();
        }
        public List<UpdateKpiAchievementItemRequest> BatchUpdateKpiAchievementItemRequest { get; set; }
    }
}
