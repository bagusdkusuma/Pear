using System;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.KpiTransformationLog;
using DSLNG.PEAR.Services.Responses.KpiTransformationLog;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.KpiTransformationEngine;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class KpiTransformationLogService : BaseService, IKpiTransformationLogService
    {
        public KpiTransformationLogService(IDataContext dataContext) : base(dataContext)
        {

        }

        public GetKpiTransformationLogsResponse Get(GetKpiTransformationLogsRequest request)
        {
            int totalRecord = 0;
            var data = SortData(request.Search, request.SortingDictionary, request.ScheduleId, out totalRecord);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }
            return new GetKpiTransformationLogsResponse
            {
                TotalRecords = totalRecord,
                Logs = data.ToList().MapTo<GetKpiTransformationLogsResponse.KpiTransformationLogResponse>()
            };
        }
        private IEnumerable<KpiTransformationLog> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, int scheduleId, out int TotalRecords)
        {
            var data = DataContext.KpiTransformationLogs.Include(x => x.Schedule.KpiTransformation).Include(x => x.Kpi).Include(x => x.Kpi.Measurement)
                .Where(x => x.Schedule.Id == scheduleId).AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x => x.Kpi.Name.Contains(search));
            }
            if (sortingDictionary != null && sortingDictionary.Count > 0)
            {
                foreach (var sortOrder in sortingDictionary)
                {
                    switch (sortOrder.Key)
                    {
                        case "Periode":
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.Periode)
                                : data.OrderByDescending(x => x.Periode);
                            break;
                        case "KpiName":
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.Kpi.Name)
                                : data.OrderByDescending(x => x.Kpi.Name);
                            break;
                        case "Status":
                        default:
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.Status)
                                : data.OrderByDescending(x => x.Status);
                            break;
                    }
                }
            }
            else
            {
                data = data.OrderByDescending(x => x.Kpi.Name);
            }
            TotalRecords = data.Count();
            return data;
        }

        public SaveKpiTransformationLogResponse Save(SaveKpiTransformationLogRequest request)
        {
            try
            {
                var kpiTransformationLog = request.MapTo<KpiTransformationLog>();
                var kpiTransformationSchedule = DataContext.KpiTransformationSchedules.Local.FirstOrDefault(x => x.Id == request.KpiTransformationScheduleId);
                if (kpiTransformationSchedule == null)
                {
                    kpiTransformationSchedule = new KpiTransformationSchedule { Id = request.KpiTransformationScheduleId };
                    DataContext.KpiTransformationSchedules.Attach(kpiTransformationSchedule);
                }
                kpiTransformationLog.Schedule = kpiTransformationSchedule;
                var kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == request.KpiId);
                if (kpi == null)
                {
                    kpi = new Kpi { Id = request.KpiId };
                    DataContext.Kpis.Attach(kpi);
                }
                kpiTransformationLog.Kpi = kpi;
                DataContext.KpiTransformationLogs.Add(kpiTransformationLog);

                //remove related kpi if error
                if (request.Status == Data.Enums.KpiTransformationStatus.Error && request.MethodId == 1 && request.NeedCleanRowWhenError)
                {
                    var achievements = DataContext.KpiAchievements.Where(
                       x => x.Kpi.Id == request.KpiId && x.Periode == request.Periode && x.PeriodeType == request.PeriodeType).ToList();
                    foreach (var achievement in achievements)
                    {
                        DataContext.KpiAchievements.Remove(achievement);
                    }
                    DataContext.SaveChanges();
                }

                DataContext.SaveChanges();
                return new SaveKpiTransformationLogResponse { IsSuccess = true, Message = "You have been successfully saved kpi transformation log" };

            }
            catch (Exception ex)
            {
                return new SaveKpiTransformationLogResponse { IsSuccess = false, Message = "Error when saving kpi transformation log" + ex.Message };
            }
        }
    }
}
