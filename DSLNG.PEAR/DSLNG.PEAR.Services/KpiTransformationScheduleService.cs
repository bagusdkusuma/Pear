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
using System.Data.SqlClient;
using DSLNG.PEAR.Data.Enums;

namespace DSLNG.PEAR.Services
{
    public class KpiTransformationScheduleService : BaseService,IKpiTransformationScheduleService
    {
        public KpiTransformationScheduleService(IDataContext dataContext) : base(dataContext) { }

        public GetKpiTransformationSchedulesResponse Get(GetKpiTransformationSchedulesRequest request)
        {
            int totalRecord = 0;
            var data = SortData(request.SortingDictionary, request.KpiTransformationId, out totalRecord);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }
            return new GetKpiTransformationSchedulesResponse
            {
                TotalRecords = totalRecord,
                Schedules = data.ToList().MapTo<GetKpiTransformationSchedulesResponse.KpiTransformationScheduleResponse>()
            };
        }
        private IEnumerable<KpiTransformationSchedule> SortData(IDictionary<string, SortOrder> sortingDictionary, int kpiTransformationId, out int TotalRecords)
        {
            var data = DataContext.KpiTransformationSchedules.Include(x => x.KpiTransformation).Where(x => x.KpiTransformation.Id == kpiTransformationId).AsQueryable();
            if (sortingDictionary != null && sortingDictionary.Count > 0)
            {
                foreach (var sortOrder in sortingDictionary)
                {
                    switch (sortOrder.Key)
                    {
                        case "End":
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.End)
                                : data.OrderByDescending(x => x.End);
                            break;
                        case "Start":
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.Start)
                                : data.OrderByDescending(x => x.Start);
                            break;
                        case "ProcessingDate":
                        default:
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.ProcessingDate)
                                : data.OrderByDescending(x => x.ProcessingDate);
                            break;
                    }
                }
            }
            else
            {
                data = data.OrderByDescending(x => x.ProcessingDate);
            }
            TotalRecords = data.Count();
            return data;
        }

        public SaveKpiTransformationScheduleResponse Save(SaveKpiTransformationScheduleRequest request)
        {

            var kpiTransformationSchedule = request.MapTo<KpiTransformationSchedule>();
            var kpiTransformation = DataContext.KpiTransformations.Single(x => x.Id == request.KpiTransformationId);
            kpiTransformationSchedule.KpiTransformation = kpiTransformation;
            if(request.ProcessingType == ProcessingType.Instant)
            {
                kpiTransformationSchedule.ProcessingDate = DateTime.Now;
                kpiTransformationSchedule.Status = KpiTransformationStatus.InProgress;
                kpiTransformation.LastProcessing = kpiTransformationSchedule.ProcessingDate;
            }
            DataContext.Kpis.Where(x => request.KpiIds.Contains(x.Id)).ToList();
            foreach (var kpiIdReq in request.KpiIds) {
                var kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiIdReq);
                kpiTransformationSchedule.SelectedKpis.Add(kpi);
            }
            DataContext.KpiTransformationSchedules.Add(kpiTransformationSchedule);
            DataContext.SaveChanges();
            kpiTransformationSchedule = DataContext.KpiTransformationSchedules.Include(x => x.KpiTransformation).Include(x => x.SelectedKpis)
                .Include(x => x.SelectedKpis.Select(y => y.Method)).First(x => x.Id == kpiTransformationSchedule.Id);
            var response =  new SaveKpiTransformationScheduleResponse
            {
                IsSuccess = true,
                Message = "You have been successfully saved kpi transformation schedule"
            };
            kpiTransformationSchedule.MapPropertiesToInstance<SaveKpiTransformationScheduleResponse>(response);
            response.UserId = request.UserId;
            return response;
        }

        public void UpdateStatus(int id, KpiTransformationStatus status)
        {
            var schedule = DataContext.KpiTransformationSchedules.Single(x => x.Id == id);
            schedule.Status = status;
            DataContext.SaveChanges();
        }
    }
}
