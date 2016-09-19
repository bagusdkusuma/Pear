using DSLNG.PEAR.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.KpiTransformationSchedule;
using DSLNG.PEAR.Services.Responses.KpiTransformationSchedule;
using DSLNG.PEAR.Data.Entities.KpiTransformationEngine;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class KpiTransformationScheduleService : BaseService,IKpiTransformationScheduleService
    {
        public KpiTransformationScheduleService(IDataContext dataContext) : base(dataContext) { }
        public SaveKpiTransformationScheduleResponse Save(SaveKpiTransformationScheduleRequest request)
        {

            var kpiTransformationSchedule = request.MapTo<KpiTransformationSchedule>();
            var kpiTransformation = DataContext.KpiTransformations.Single(x => x.Id == request.KpiTransformationId);
            kpiTransformationSchedule.KpiTransformation = kpiTransformation;
            if(request.ProcessingType == Data.Enums.ProcessingType.Instant)
            {
                kpiTransformationSchedule.ProcessingDate = DateTime.Now;
            }
            DataContext.Kpis.Where(x => request.KpiIds.Contains(x.Id)).ToList();
            foreach (var kpiIdReq in request.KpiIds) {
                var kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiIdReq);
                kpiTransformationSchedule.SelectedKpis.Add(kpi);
            }
            DataContext.KpiTransformationSchedules.Add(kpiTransformationSchedule);
            DataContext.SaveChanges();
            kpiTransformationSchedule = DataContext.KpiTransformationSchedules.Include(x => x.KpiTransformation).Include(x => x.SelectedKpis).First(x => x.Id == kpiTransformationSchedule.Id);
            var response =  new SaveKpiTransformationScheduleResponse
            {
                IsSuccess = true,
                Message = "You have been successfully saved kpi transformation schedule"
            };
            kpiTransformationSchedule.MapPropertiesToInstance<SaveKpiTransformationScheduleResponse>(response);
            response.UserId = request.UserId;
            return response;
        }
    }
}
