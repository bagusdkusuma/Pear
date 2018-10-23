using System.Data.SqlClient;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Kpi;
using DSLNG.PEAR.Services.Responses.Kpi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using KpiRelationModel = DSLNG.PEAR.Data.Entities.KpiRelationModel;
using DSLNG.PEAR.Services.Responses;

namespace DSLNG.PEAR.Services
{
    public class KpiService : BaseService, IKpiService
    {
        public KpiService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public GetKpiResponse GetBy(GetKpiRequest request)
        {
            var query = DataContext.Kpis.Include(x => x.Measurement);
            if (request.Id != 0)
            {
                query = query.Where(x => x.Id == request.Id);
            }
            return query.FirstOrDefault().MapTo<GetKpiResponse>();
        }


        public GetKpiToSeriesResponse GetKpiToSeries(GetKpiToSeriesRequest request)
        {
            var query = DataContext.Kpis
                    .Include(x => x.Measurement).Where(x => x.Name.Contains(request.Term)).AsQueryable();
            if (request.MeasurementId != 0) {
                query = query.Where(x => x.Measurement.Id == request.MeasurementId);
            }
            if (request.PeriodeType.HasValue) {
                query = query.Where(x => x.Period == request.PeriodeType.Value);
            }
            
            return new GetKpiToSeriesResponse
            {
                KpiList = query.Take(20).ToList()
                .MapTo<GetKpiToSeriesResponse.Kpi>()
            };
        }

        public GetKpiResponse GetKpi(GetKpiRequest request)
        {
            try
            {
                var kpi = GetSingleKpi(request);
                var response = kpi.MapTo<GetKpiResponse>();

                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetKpiResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        public GetKpiDetailResponse GetKpiDetail(GetKpiRequest request)
        {
            try
            {
                var kpi = GetSingleKpi(request);
                var response = kpi.MapTo<GetKpiDetailResponse>();
                response.IsSuccess = true;
                return response;
            }
            catch (System.InvalidOperationException x)
            {
                return new GetKpiDetailResponse
                {
                    IsSuccess = false,
                    Message = x.Message
                };
            }
        }

        private Kpi GetSingleKpi(GetKpiRequest request)
        {
            var kpi = DataContext.Kpis
                                 .Include(x => x.Pillar)
                                 .Include(x => x.Level)
                                 .Include(x => x.RoleGroup)
                                 .Include(x => x.Group)
                                 .Include(x => x.Type)
                                 .Include(x => x.Measurement)
                                 .Include(x => x.Method)
                                 .Include(x => x.RelationModels)
                                 .Include("RelationModels.Kpi").First(x => x.Id == request.Id);

            var relationModels =
                DataContext.KpiRelationModels.Include(x => x.Kpi).Include(x => x.KpiParent).Where(x => x.Kpi.Id == kpi.Id);
            foreach (var item in relationModels)
            {
                if (kpi.RelationModels.FirstOrDefault(x => x.Kpi.Id.Equals(item.KpiParent.Id)) == null)
                {
                    kpi.RelationModels.Add(new Data.Entities.KpiRelationModel
                        {
                            Id = item.Id,
                            Kpi = item.KpiParent,
                            KpiParent = item.Kpi,
                            Method = item.Method
                        });
                }
            }
            return kpi;
        }

        public GetKpisResponse GetKpis(GetKpisRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            //var kpis = new Queryable<Kpi>();
            /*if (request.Take != 0)
            {
                kpis = DataContext.Kpis.Include(x => x.Pillar).OrderBy(x => x.Id).Skip(request.Skip).Take(request.Take);
            }
            else
            {
                kpis = DataContext.Kpis.Include(x => x.Pillar);
            }

            if (request.PillarId > 0)
            {
                kpis = kpis.Include(x => x.Pillar).Where(x => x.Pillar.Id == request.PillarId);
            }*/


            var response = new GetKpisResponse();
            response.TotalRecords = totalRecords;
            response.Kpis = data.ToList().MapTo<GetKpisResponse.Kpi>();

            return response;
        }

        public GetKpisResponse GetKpis(List<int> kpiIds)
        {
            GetKpisResponse response = new GetKpisResponse();
            response.Kpis = new List<GetKpisResponse.Kpi>();
            var kpis = DataContext.Kpis
                .Include(x => x.Measurement)
                .Where(x => kpiIds.Contains(x.Id))
                .ToList();
            
            foreach(var kpi in kpis)
            {
                response.Kpis.Add(new GetKpisResponse.Kpi
                {
                    Id = kpi.Id,
                    Name = kpi.Name,
                    Measurement = new GetKpisResponse.Measurement { Name = kpi.Measurement.Name }
                });
            }

            response.IsSuccess = true;
            return response;
        }

        public CreateKpiResponse Create(CreateKpiRequest request)
        {
            var response = new CreateKpiResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var kpi = request.MapTo<Kpi>();
                if (request.PillarId.HasValue)
                {
                    kpi.Pillar = DataContext.Pillars.FirstOrDefault(x => x.Id == request.PillarId);
                }
                if (request.GroupId.HasValue)
                {
                    kpi.Group = DataContext.Groups.FirstOrDefault(x => x.Id == request.GroupId);
                }
                if (request.RoleGroupId.HasValue)
                {
                    kpi.RoleGroup = DataContext.RoleGroups.FirstOrDefault(x => x.Id == request.RoleGroupId.Value);
                }
                if (request.MeasurementId.HasValue)
                {
                    kpi.Measurement = DataContext.Measurements.FirstOrDefault(x => x.Id == request.MeasurementId);
                }
                kpi.Level = DataContext.Levels.FirstOrDefault(x => x.Id == request.LevelId);
                kpi.Type = DataContext.Types.FirstOrDefault(x => x.Id == request.TypeId);
                kpi.Method = DataContext.Methods.FirstOrDefault(x => x.Id == request.MethodId);
                kpi.CreatedBy = DataContext.Users.FirstOrDefault(x=>x.Id == request.UserId);
                if (request.RelationModels.Count > 0)
                {
                    var relation = new List<KpiRelationModel>();
                    foreach (var item in request.RelationModels)
                    {
                        if (item.KpiId != 0)
                        {
                            var kpiRelation = DataContext.Kpis.FirstOrDefault(x => x.Id == item.KpiId);
                            relation.Add(new KpiRelationModel
                            {
                                Kpi = kpiRelation,
                                Method = item.Method
                            });
                        }
                    }
                    kpi.RelationModels = relation;
                }

                DataContext.Kpis.Add(kpi);
                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "KPI item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public UpdateKpiResponse Update(UpdateKpiRequest request)
        {
            var response = new UpdateKpiResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var updateKpi = request.MapTo<Kpi>();
                if (request.PillarId.HasValue)
                {
                    updateKpi.Pillar = DataContext.Pillars.FirstOrDefault(x => x.Id == request.PillarId);
                }
                if (request.GroupId.HasValue)
                {
                    updateKpi.Group = DataContext.Groups.FirstOrDefault(x => x.Id == request.GroupId);
                }
                if (request.RoleGroupId.HasValue)
                {
                    updateKpi.RoleGroup = DataContext.RoleGroups.FirstOrDefault(x => x.Id == request.RoleGroupId.Value);
                }

                updateKpi.Measurement = DataContext.Measurements.Single(x => x.Id == request.MeasurementId);
                updateKpi.Level = DataContext.Levels.FirstOrDefault(x => x.Id == request.LevelId);
                updateKpi.Type = DataContext.Types.FirstOrDefault(x => x.Id == request.TypeId);
                updateKpi.Method = DataContext.Methods.FirstOrDefault(x => x.Id == request.MethodId);
                updateKpi.UpdatedBy = DataContext.Users.FirstOrDefault(x => x.Id == request.UserId);
                var existedkpi = DataContext.Kpis
                    .Where(x => x.Id == request.Id)
                    .Include(x => x.RelationModels.Select(y => y.Kpi))
                    .Include(x => x.RelationModels.Select(y => y.KpiParent))
                    .Include(x => x.Pillar)
                    .Include(x => x.Level)
                    .Include(x => x.RoleGroup)
                    .Include(x => x.Group)
                    .Include(x => x.Type)
                    .Include(x => x.Measurement)
                    .Include(x => x.Method)
                    .Single();

                DataContext.Entry(existedkpi).CurrentValues.SetValues(updateKpi);

                if (updateKpi.Group != null)
                {
                    DataContext.Groups.Attach(updateKpi.Group);
                    existedkpi.Group = updateKpi.Group;
                }

                if (updateKpi.RoleGroup != null)
                {
                    DataContext.RoleGroups.Attach(updateKpi.RoleGroup);
                    existedkpi.RoleGroup = updateKpi.RoleGroup;
                }

                if (updateKpi.Pillar != null)
                {
                    DataContext.Pillars.Attach(updateKpi.Pillar);
                    existedkpi.Pillar = updateKpi.Pillar;
                }
                DataContext.Measurements.Attach(updateKpi.Measurement);
                existedkpi.Measurement = updateKpi.Measurement;

                DataContext.Levels.Attach(updateKpi.Level);
                existedkpi.Level = updateKpi.Level;

                DataContext.Types.Attach(updateKpi.Type);
                existedkpi.Type = updateKpi.Type;

                DataContext.Methods.Attach(updateKpi.Method);
                existedkpi.Method = updateKpi.Method;

                var joinedRelationModels = existedkpi.RelationModels.ToList();
                var additionalRelationModels = DataContext.KpiRelationModels
                    .Include(x => x.Kpi)
                    .Include(x => x.KpiParent)
                    .Where(x => x.Kpi.Id == request.Id).ToList();


                foreach (var item in additionalRelationModels)
                {
                    joinedRelationModels.Add(item);
                }

                foreach (var joinedRelationModel in joinedRelationModels)
                {
                    DataContext.KpiRelationModels.Remove(joinedRelationModel);
                }

                if (request.RelationModels.Count > 0)
                {
                    var relation = new List<KpiRelationModel>();
                    foreach (var item in request.RelationModels)
                    {
                        if (item.KpiId != 0)
                        {
                            var kpiRelation = DataContext.Kpis.FirstOrDefault(x => x.Id == item.KpiId);
                            relation.Add(new KpiRelationModel
                            {
                                Kpi = kpiRelation,
                                Method = item.Method
                            });
                        }
                    }
                    existedkpi.RelationModels = relation;
                }

                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "KPI item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public DeleteKpiResponse Delete(int id)
        {
            var response = new DeleteKpiResponse();
            try
            {
                var kpi = new Kpi { Id = id };
                DataContext.Kpis.Attach(kpi);
                DataContext.Entry(kpi).State = EntityState.Deleted;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "KPI item has been deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }

        public bool IsValidKpi(GetKpiByRole request)
        {
            try {
                DataContext.Kpis.First(x => x.RoleGroup.Id == request.RoleId);
                return true;
            }
            catch (System.InvalidOperationException) {
                return false;
            }
        }

        private IEnumerable<Kpi> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int totalRecords)
        {
            var data = DataContext.Kpis
                .Include(x => x.Pillar)
                .Include(x => x.Measurement)
                .Include(x => x.Type)
                .AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Code.Contains(search) || x.Pillar.Name.Contains(search) ||
                                       x.Name.Contains(search));
            }

            if (sortingDictionary != null && sortingDictionary.Count > 0)
            {
                foreach (var sortOrder in sortingDictionary)
                {
                    switch (sortOrder.Key)
                    {
                        case "Code":
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.Code).ThenBy(x => x.Order)
                                       : data.OrderByDescending(x => x.Code).ThenBy(x => x.Order);
                            break;
                        case "Name":
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.Name).ThenBy(x => x.Order)
                                       : data.OrderByDescending(x => x.Name).ThenBy(x => x.Order);
                            break;
                        case "PillarName":
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.Pillar.Name).ThenBy(x => x.Order)
                                       : data.OrderByDescending(x => x.Pillar.Name).ThenBy(x => x.Order);
                            break;
                        case "IsActive":
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.IsActive).ThenBy(x => x.Order)
                                       : data.OrderByDescending(x => x.IsActive).ThenBy(x => x.Order);
                            break;
                        case "IsEconomic":
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.IsEconomic).ThenBy(x => x.Order)
                                       : data.OrderByDescending(x => x.IsEconomic).ThenBy(x => x.Order);
                            break;
                        case "Type.Name":
                        case "Type":
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.Type.Name).ThenBy(x => x.Order)
                                       : data.OrderByDescending(x => x.Type.Name).ThenBy(x => x.Order);
                            break;
                        default:
                            data = sortOrder.Value == SortOrder.Ascending
                                       ? data.OrderBy(x => x.Order)
                                       : data.OrderByDescending(x => x.Order);
                            break;
                    }
                }
            }
            
            totalRecords = data.Count();
            return data;
        }


        public IList<Kpi> DownloadKpis()
        {
            var data =  DataContext.Kpis
                .Include(x => x.Pillar)
                .Include(x => x.Measurement)
                .Include(x => x.Type)
                .Include(x => x.Level)
                .Include(x => x.Group)
                .Include(x => x.UpdatedBy)
                .Include(x => x.CreatedBy)
                .Include(x => x.Method)
                .Include(x => x.RoleGroup)
                .ToList();
            return data;
        }

        public DeleteKpiResponse Delete(DeleteKpiRequest request)
        {
            var response = new DeleteKpiResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var kpi = new Kpi { Id = request.Id };
                DataContext.Kpis.Attach(kpi);
                DataContext.Entry(kpi).State = EntityState.Deleted;
                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "KPI item has been deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.Message = dbUpdateException.Message;
            }

            return response;
        }
    }
}
