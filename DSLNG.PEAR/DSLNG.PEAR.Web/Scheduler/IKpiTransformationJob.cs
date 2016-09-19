using DSLNG.PEAR.Services.Responses.KpiTransformationSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.Scheduler
{
    public interface IKpiTransformationJob
    {
        void Process(SaveKpiTransformationScheduleResponse kpiTransformationSchedule);
    }
}