using System;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.KpiTransformationLog;
using DSLNG.PEAR.Services.Responses.KpiTransformationLog;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.KpiTransformationEngine;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;

namespace DSLNG.PEAR.Services
{
    public class KpiTransformationLogService : BaseService, IKpiTransformationLogService
    {
        public KpiTransformationLogService(IDataContext dataContext) : base(dataContext) {

        }
        public SaveKpiTransformationLogResponse Save(SaveKpiTransformationLogRequest request)
        {
            var kpiTransformationLog = request.MapTo<KpiTransformationLog>();
            var kpiTransformationSchedule = new KpiTransformationSchedule { Id = request.KpiTransformationScheduleId };
            DataContext.KpiTransformationSchedules.Attach(kpiTransformationSchedule);
            kpiTransformationLog.Schedule = kpiTransformationSchedule;
            var kpi = new Kpi { Id = request.KpiId };
            DataContext.Kpis.Attach(kpi);
            kpiTransformationLog.Kpi = kpi;
            DataContext.SaveChanges();
            return new SaveKpiTransformationLogResponse { IsSuccess = true, Message = "You have been successfully saved kpi transformation log" };
        }
    }
}
