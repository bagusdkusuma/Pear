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
using DSLNG.PEAR.Services.Responses;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class KpiTransformationScheduleService : BaseService, IKpiTransformationScheduleService
    {
        //private IKpiTransformationLogService _logService;

        public KpiTransformationScheduleService(IDataContext dataContext) : base(dataContext)
        {
           // _logService = logService;
        }

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
            var action = request.MapTo<BaseAction>();
            var kpiTransformationSchedule = request.MapTo<KpiTransformationSchedule>();
            var kpiTransformation = DataContext.KpiTransformations.Single(x => x.Id == request.KpiTransformationId);
            kpiTransformationSchedule.KpiTransformation = kpiTransformation;
            if (request.ProcessingType == ProcessingType.Instant)
            {
                kpiTransformationSchedule.ProcessingDate = DateTime.Now;
                kpiTransformationSchedule.Status = KpiTransformationStatus.InProgress;
                kpiTransformation.LastProcessing = kpiTransformationSchedule.ProcessingDate;
            }
            DataContext.Kpis.Where(x => request.KpiIds.Contains(x.Id)).ToList();
            foreach (var kpiIdReq in request.KpiIds)
            {
                var kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiIdReq);
                kpiTransformationSchedule.SelectedKpis.Add(kpi);
            }
            DataContext.KpiTransformationSchedules.Add(kpiTransformationSchedule);
            DataContext.SaveChanges(action);
            kpiTransformationSchedule = DataContext.KpiTransformationSchedules.Include(x => x.KpiTransformation).Include(x => x.SelectedKpis)
                .Include(x => x.SelectedKpis.Select(y => y.Method)).First(x => x.Id == kpiTransformationSchedule.Id);
            var response = new SaveKpiTransformationScheduleResponse
            {
                IsSuccess = true,
                Message = "You have been successfully saved kpi transformation schedule"
            };
            kpiTransformationSchedule.MapPropertiesToInstance<SaveKpiTransformationScheduleResponse>(response);
            response.UserId = request.UserId;
            response.ControllerName = request.ControllerName;
            response.ActionName = request.ActionName;
            return response;
        }

        public void UpdateStatus(int id, KpiTransformationStatus status)
        {
            try
            {
                var schedule = DataContext.KpiTransformationSchedules.Single(x => x.Id == id);
                schedule.Status = status;
                DataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                var schedule = DataContext.KpiTransformationSchedules.Single(x => x.Id == id);
                DataContext.KpiTransformationLogs.Add(new KpiTransformationLog { Periode = DateTime.Now, Status = KpiTransformationStatus.Error, Kpi = null, Notes = "Error When Update Status " + ex.Message, Schedule = schedule });
                DataContext.SaveChanges();
            }

        }

        public GetKpiTransformationSchedulesResponse.KpiTransformationScheduleResponse Get(int Id)
        {
            return DataContext.KpiTransformationSchedules.Include(x => x.KpiTransformation).FirstOrDefault(x => x.Id == Id).MapTo<GetKpiTransformationSchedulesResponse.KpiTransformationScheduleResponse>();
        }

        public BaseResponse Delete(int id)
        {
            var response = new BaseResponse();
            try
            {
                var schedule = DataContext.KpiTransformationSchedules
                    .Include(x => x.Logs)
                    .FirstOrDefault(x => x.Id == id);
                if (schedule != null)
                {
                    if (schedule.Logs.Count > 0)
                    {
                        foreach (var log in schedule.Logs.ToList())
                        {
                            DataContext.KpiTransformationLogs.Remove(log);
                        }
                        DataContext.SaveChanges();
                    }
                    DataContext.KpiTransformationSchedules.Remove(schedule);
                    DataContext.SaveChanges();
                }
                response.IsSuccess = true;
                response.Message = string.Format("Schedule for {0:MMM dd yyyy H:mm:ss} deleted Successfully", schedule.ProcessingDate);
            }
            catch (DbUpdateException e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }

        public BaseResponse BatchDelete(int[] ids)
        {
            var response = new BaseResponse();
            if (ids.Count() > 0)
            {
                int succeed = 0;
                int failed = 0;
                List<string> failedSchedule = new List<string>();
                for (int i = 0; i < ids.Count(); i++)
                {
                    var result = Delete(ids[i]);
                    if (result.IsSuccess)
                    {
                        succeed++;
                    }
                    else
                    {
                        failed++;
                        failedSchedule.Add(ids[i].ToString());
                    }
                }
                response.IsSuccess = succeed > failed ? true : false;
                var failedDeleted = string.Empty;
                if (failed > 0)
                {
                    failedDeleted = string.Format(" This item are failed to deleted : ({0}) ", string.Join(",", failedSchedule));
                }
                if (response.IsSuccess)
                {
                    response.Message = string.Format("Batch Delete Not Success, only {0} schedule deleted, and {1} failed to delete. {2}", succeed, failed, failedDeleted);
                }
                else
                {
                    response.Message = string.Format("Successfully Delete {0} schedule, and {1} failed to delete. {2}", succeed, failed, failedDeleted);
                }

            }
            return response;
        }

        public BaseResponse Delete(DeleteKPITransformationScheduleRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var schedule = DataContext.KpiTransformationSchedules
                    .Include(x => x.Logs)
                    .FirstOrDefault(x => x.Id == request.Id);
                if (schedule != null)
                {
                    if (schedule.Logs.Count > 0)
                    {
                        foreach (var log in schedule.Logs.ToList())
                        {
                            DataContext.KpiTransformationLogs.Remove(log);
                        }
                        DataContext.SaveChanges(action);
                    }
                    DataContext.KpiTransformationSchedules.Remove(schedule);
                    DataContext.SaveChanges(action);
                }
                response.IsSuccess = true;
                response.Message = string.Format("Schedule for {0:MMM dd yyyy H:mm:ss} deleted Successfully", schedule.ProcessingDate);
            }
            catch (DbUpdateException e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }
            return response;
        }
    }
}
