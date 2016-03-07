using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.KpiAchievement
{
    public class GetKpiAchievementRequestByValue
    {
        public int KpiId { get; set; }
        public string PeriodeType { get; set; }
        public DateTime Periode { get; set; }
    }
}
