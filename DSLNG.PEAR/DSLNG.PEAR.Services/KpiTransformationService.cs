

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

namespace DSLNG.PEAR.Services
{
    public class KpiTransformationService : BaseService, IKpiTransformationService
    {
        public KpiTransformationService(IDataContext dataContext) : base(dataContext) { }

        public GetKpiTransformationsResponse Get(GetKpiTransformationsRequest request)
        {
            int totalRecord = 0;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecord);
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
        private IEnumerable<KpiTransformation> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.KpiTransformations.Include(x => x.RoleGroups).AsQueryable();
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
                    DataContext.SaveChanges();
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
                    DataContext.SaveChanges();
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
    }
}
