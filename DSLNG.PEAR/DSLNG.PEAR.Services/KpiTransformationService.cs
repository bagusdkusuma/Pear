

using System;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.KpiTransformation;
using DSLNG.PEAR.Services.Responses.KpiInformation;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities.KpiTransformationEngine;
using System.Linq;
using DSLNG.PEAR.Data.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Services.Responses;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class KpiTransformationService : BaseService, IKpiTransformationService
    {
        public KpiTransformationService(IDataContext dataContext) : base(dataContext) { }

        public GetKpiTransformationsResponse Get(GetKpiTransformationsRequest request)
        {
            int totalRecord = 0;
            var data = SortData(request.UserId, request.Search, request.SortingDictionary, out totalRecord);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }
            return new GetKpiTransformationsResponse
            {
                TotalRecords = totalRecord,
                KpiTransformations = data.ToList().MapTo<GetKpiTransformationsResponse.KpiTransformationResponse>()
            };
        }
        private IEnumerable<KpiTransformation> SortData(int userId, string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KpiTransformations.Include(x => x.RoleGroups).AsQueryable();
            if (userId != 0) {
                var user = DataContext.Users.First(x => x.Id == userId);
                if (!user.IsSuperAdmin)
                {
                    var roleId = user.RoleId.Value;
                    data = data.Where(x => x.RoleGroups.Select(y => y.Id).Contains(roleId)).AsQueryable();
                }
            }
            
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search));
            }
            if (sortingDictionary != null && sortingDictionary.Count > 0)
            {
                foreach (var sortOrder in sortingDictionary)
                {
                    switch (sortOrder.Key)
                    {
                        case "LastProcessing":
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.LastProcessing)
                                : data.OrderByDescending(x => x.LastProcessing);
                            break;
                        case "Name":
                        default:
                            data = sortOrder.Value == SortOrder.Ascending
                                ? data.OrderBy(x => x.Name)
                                : data.OrderByDescending(x => x.Name);
                            break;
                    }
                }
            }
            else
            {
                data = data.OrderBy(x => x.Name);
            }
            TotalRecords = data.Count();
            return data;
        }
        public SaveKpiTransformationResponse Save(SaveKpiTransformationRequest request)
        {
            try
            {
                var action = request.MapTo<BaseAction>();
                if (request.Id == 0)
                {
                    var kpiTransformation = new KpiTransformation { Name = request.Name, PeriodeType = request.PeriodeType };
                    foreach (var roleId in request.RoleGroupIds.Distinct().ToArray())
                    {
                        var role = new RoleGroup { Id = roleId };
                        DataContext.RoleGroups.Attach(role);
                        kpiTransformation.RoleGroups.Add(role);
                    }
                    foreach (var kpiId in request.KpiIds.Distinct().ToArray())
                    {
                        var kpi = new Kpi { Id = kpiId };
                        DataContext.Kpis.Attach(kpi);
                        kpiTransformation.Kpis.Add(kpi);
                    }
                    DataContext.KpiTransformations.Add(kpiTransformation);
                    DataContext.SaveChanges(action);
                }
                else
                {
                    var kpiTransformation = DataContext.KpiTransformations.Include(x => x.RoleGroups).Include(x => x.Kpis).Single(x => x.Id == request.Id);
                    kpiTransformation.Name = request.Name;
                    kpiTransformation.PeriodeType = request.PeriodeType;
                    foreach (var role in kpiTransformation.RoleGroups.ToList())
                    {
                        kpiTransformation.RoleGroups.Remove(role);
                    }
                    foreach (var kpi in kpiTransformation.Kpis.ToList())
                    {
                        kpiTransformation.Kpis.Remove(kpi);
                    }
                    foreach (var roleId in request.RoleGroupIds.Distinct().ToArray())
                    {
                        var role = DataContext.RoleGroups.Local.FirstOrDefault(x => x.Id == roleId);
                        if (role == null)
                        {
                            role = new RoleGroup { Id = roleId };
                            DataContext.RoleGroups.Attach(role);

                        }
                        kpiTransformation.RoleGroups.Add(role);

                    }
                    foreach (var kpiId in request.KpiIds.Distinct().ToArray())
                    {
                        var kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiId);
                        if (kpi == null)
                        {
                            kpi = new Kpi { Id = kpiId };
                            DataContext.Kpis.Attach(kpi);
                        }
                        kpiTransformation.Kpis.Add(kpi);
                    }
                    DataContext.SaveChanges(action);
                }
                return new SaveKpiTransformationResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully updated kpi transformation"
                };
            }
            catch(Exception e) {
                return new SaveKpiTransformationResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }

        public GetKpiTransformationResponse Get(int id)
        {
            return DataContext.KpiTransformations
                .Include(x => x.Kpis)
                .Include(x => x.Kpis.Select(y => y.Measurement))
                .Include(x => x.RoleGroups)
                .Single(x => x.Id == id).MapTo<GetKpiTransformationResponse>();
        }

        public BaseResponse Delete(int id)
        {
            try
            {
                var item = DataContext.KpiTransformations
                    .Include(x => x.Kpis)
                    .Include(x => x.Schedules)
                    .Include(x => x.Schedules.Select(y => y.Logs))
                    .First(x => x.Id == id);
                DataContext.KpiTransformations.Remove(item);
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = string.Format("{0} deleted Successfully", item.Name)
                };
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.InnerException.Message.Contains("dbo.KpiTransformations"))
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "This Item still Used by KpiTransformationService"
                    };
                }
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this item"
                };
            }
        }

        public BaseResponse Delete(DeleteTransformationRequest request)
        {
            try
            {
                var action = request.MapTo<BaseAction>();
                var item = DataContext.KpiTransformations
                    .Include(x => x.Kpis)
                    .Include(x => x.Schedules)
                    .Include(x => x.Schedules.Select(y => y.Logs))
                    .First(x => x.Id == request.Id);
                DataContext.KpiTransformations.Remove(item);
                DataContext.SaveChanges(action);
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = string.Format("{0} deleted Successfully", item.Name)
                };
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.InnerException.Message.Contains("dbo.KpiTransformations"))
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "This Item still Used by KpiTransformationService"
                    };
                }
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this item"
                };
            }
        }
    }
}
