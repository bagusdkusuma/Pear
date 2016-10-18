using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.KpiTransformationSchedule
{
    public class GetKpiTransformationSchedulesResponse
    {
        public IList<KpiTransformationScheduleResponse> Schedules { get; set; }
        public int TotalRecords { get; set; }
        public class KpiTransformationScheduleResponse
        {
            public int Id { get; set; }
            public int KpiTransformation_Id { get; set; }
            public string Name { get; set; }
            public PeriodeType PeriodeType { get; set; }
            public DateTime ProcessingDate { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public KpiTransformationStatus Status { get; set; }
            public string StatusName { get { return Enum.GetName(typeof(KpiTransformationStatus), this.Status); }}
        }
    }
}
