using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.KpiTransformationSchedule
{
    public class SaveKpiTransformationScheduleRequest : BaseRequest
    {
        public int KpiTransformationId { get; set; }
        public PeriodeType PeriodeType { get; set; }
        public ProcessingType ProcessingType { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
       public int UserId { get; set; }
        public IList<int> KpiIds { get; set; }
        
    }

    public class DeleteKPITransformationScheduleRequest : BaseRequest
    {
        public int Id { get; set; }
    }
}
