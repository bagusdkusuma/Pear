using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Entities.Der;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Der;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.Der;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DSLNG.PEAR.Services
{
    public class DerService : BaseService, IDerService
    {
        private readonly IKpiAchievementService _kpiAchievementService;
        private readonly IKpiTargetService _kpiTargetService;
        public DerService(IDataContext dataContext, IKpiAchievementService kpiAchievementService, IKpiTargetService kpiTargetService)
            : base(dataContext)
        {
            _kpiAchievementService = kpiAchievementService;
            _kpiTargetService = kpiTargetService;
        }

        public GetDersResponse GetDers(GetDersRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }
            var derList = data.ToList();
            var respData = GetCustomHighlights(request, derList);
            return new GetDersResponse
            {
                TotalRecords = totalRecords,
                Ders = respData.Ders.MapTo<GetDersResponse.Der>()
            };
        }

        public IEnumerable<Der> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Ders.AsQueryable();
            //data = data.Join(DataContext.Highlights, ders => ders.Date, highlight => highlight.Date, (ders, highlight) => ders);
            //data = data.Join()
            data = data.Include(x => x.GenerateBy)
                .Include(x => x.RevisionBy);
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                var dates = search.Split('-');
                var year = int.Parse(dates[1].Trim());
                var month = int.Parse(dates[0].Trim());
                if (month == 0)
                {
                    var lastDay = new DateTime(year, 1, 1);
                    var earlyNextYear = new DateTime(year + 1, 1, 1);
                    data = data.Where(x => (x.Date.Year == year && x.Date != lastDay) || x.Date == earlyNextYear);
                }
                else
                {
                    var earlyNextMonth = new DateTime();
                    if (month < 12)
                    {
                        earlyNextMonth = new DateTime(year, month + 1, 1);
                    }
                    else if (month == 12)
                    {
                        earlyNextMonth = new DateTime(year + 1, 1, 1);
                    }

                    var earlyThisMonth = new DateTime(year, month, 1);
                    data = data.Where(x => ((x.Date.Year == year && x.Date.Month == month) && x.Date != earlyThisMonth) || x.Date == earlyNextMonth);
                }
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Title":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Title).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.Title).ThenBy(x => x.IsActive);
                        break;
                    case "Date":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Date).ThenBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.Date).ThenBy(x => x.IsActive);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }

        public CreateOrUpdateResponse CreateOrUpdate(CreateOrUpdateDerRequest request)
        {
            var response = new CreateOrUpdateResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var existingDer = DataContext.Ders.FirstOrDefault(s => s.Title == request.Title);
                var user = new User { Id = request.RevisionBy };
                DataContext.Users.Attach(user);
                if (existingDer != null)
                {
                    existingDer.IsActive = request.IsActive;
                    existingDer.Title = request.Title;
                    existingDer.Date = request.Date;
                    existingDer.Filename += ";" + request.Filename;
                    existingDer.RevisionBy = user;
                    existingDer.Revision = existingDer.Revision + 1;
                }
                else
                {
                    var der = new Der();
                    der.IsActive = request.IsActive;
                    der.Title = request.Title;
                    der.Date = request.Date;
                    der.Filename = request.Filename;
                    der.RevisionBy = user;
                    der.GenerateBy = user;
                    der.Revision = 0;
                    DataContext.Ders.Add(der);
                }

                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "DER has been added successfully";
            }
            catch (Exception exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }

            return response;
        }

        public GetActiveDerResponse GetActiveDer()
        {
            var response = new GetActiveDerResponse();
            try
            {
                var der = DataContext.DerLayouts
                    .Include(x => x.Items)
                    .First(x => x.IsActive && !x.IsDeleted);

                response = der.MapTo<GetActiveDerResponse>();

                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public GetDerItemResponse GetDerItem(GetDerItemRequest request)
        {
            var response = new GetDerItemResponse();
            try
            {
                if (request.Id > 0)
                {
                    var der = DataContext.DerItems.Single(x => x.Id == request.Id);
                    response = der.MapTo<GetDerItemResponse>();
                }
                else
                {
                    response = request.MapTo<GetDerItemResponse>();
                }

                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public CreateOrUpdateDerLayoutResponse CreateOrUpdateDerLayout(CreateOrUpdateDerLayoutRequest request)
        {
            var response = new CreateOrUpdateDerLayoutResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                if (request.Id > 0)
                {
                    var derLayout = DataContext.DerLayouts.Single(x => x.Id == request.Id);
                    derLayout.IsActive = request.IsActive;
                    derLayout.Title = request.Title;
                    DataContext.Entry(derLayout).State = EntityState.Modified;

                }
                else
                {
                    DataContext.DerLayouts.Add(new DerLayout() { IsActive = request.IsActive, Title = request.Title, IsDeleted = false });
                }

                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "DER Layout has been Saved successfully";
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public GetDerLayoutsResponse GetDerLayouts()
        {
            var derLayouts = DataContext.DerLayouts.Where(x => x.IsDeleted == false).ToList();
            return new GetDerLayoutsResponse
            {
                IsSuccess = true,
                DerLayouts = derLayouts.Select(x => new GetDerLayoutsResponse.DerLayout
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Title = x.Title
                }).ToList()
            };
        }

        public BaseResponse DeleteLayout(int id)
        {
            var response = new BaseResponse();
            //var derLayoutItems = new List<DerLayoutItem>();
            //var res = new DeleteDerLayoutItemResponse();
            try
            {
                var derLayout = DataContext.DerLayouts
                .Include(x => x.Items)
                .Single(x => x.Id == id);
                derLayout.IsDeleted = true;
                DataContext.Entry(derLayout).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }
            //derLayoutItems = derLayout.Items.ToList();
            //foreach(var item in derLayoutItems)
            //{
            //    res = DeleteLayoutItem(id, item.Type);
            //    if(res.Message != null)
            //    {
            //        break;
            //    }
            //};
            //if (res.Message == null)
            //{
            //    DataContext.DerLayouts.Remove(derLayout);
            //    DataContext.SaveChanges();
            //} else
            //{
            //    response.Message = res.Message;
            //}

            return response;
        }

        public GetDerLayoutitemsResponse GetDerLayoutItems(int derLayoutId)
        {
            var response = new GetDerLayoutitemsResponse();
            var derLayoutItems = DataContext.DerLayoutItems
                                            .Include(x => x.DerLayout)
                                            .Where(x => x.DerLayout.Id == derLayoutId)
                                            .ToList();

            IList<RowAndColumns> rowAndColumns = new List<RowAndColumns>();
            rowAndColumns.Add(new RowAndColumns { Row = 0, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 0, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 0, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 0, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 1, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 1, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 1, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 1, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 2, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 2, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 2, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 2, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 4 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 5 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 6 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 7 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 8 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 9 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 10 });
            rowAndColumns.Add(new RowAndColumns { Row = 4, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 4, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 4, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 5, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 5, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 5, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 5, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 5, Column = 4 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 4 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 5 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 6 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 7 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 8 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 9 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 10 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 11 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 12 });
            rowAndColumns.Add(new RowAndColumns { Row = 7, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 7, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 7, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 8, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 8, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 8, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 8, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 8, Column = 4 });
            rowAndColumns.Add(new RowAndColumns { Row = 9, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 9, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 10, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 10, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 10, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 11, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 11, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 11, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 12, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 12, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 12, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 13, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 13, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 13, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 14, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 14, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 14, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 15, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 15, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 15, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 15, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 15, Column = 4 });
            rowAndColumns.Add(new RowAndColumns { Row = 15, Column = 5 });
            rowAndColumns.Add(new RowAndColumns { Row = 16, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 16, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 16, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 16, Column = 4 });
            rowAndColumns.Add(new RowAndColumns { Row = 16, Column = 5 });
            rowAndColumns.Add(new RowAndColumns { Row = 16, Column = 6 });
            rowAndColumns.Add(new RowAndColumns { Row = 16, Column = 7 });

            foreach (var rowAndColumn in rowAndColumns)
            {
                var item = derLayoutItems.FirstOrDefault(x => x.Row == rowAndColumn.Row && x.Column == rowAndColumn.Column);
                if (item == null)
                {
                    response.Items.Add(new GetDerLayoutitemsResponse.LayoutItem()
                    {
                        Row = rowAndColumn.Row,
                        Column = rowAndColumn.Column
                    });
                }
                else
                {
                    response.Items.Add(new GetDerLayoutitemsResponse.LayoutItem()
                    {
                        Column = item.Column,
                        Row = item.Row,
                        Id = item.Id,
                        DerLayoutId = item.DerLayout.Id,
                        Type = item.Type
                    });
                }
            }

            return response;


        }

        public GetDerLayoutitemResponse GetDerLayoutItem(int id)
        {
            var response = new GetDerLayoutitemResponse();
            try
            {
                var derLayoutItem = DataContext
                    .DerLayoutItems
                    .Include(x => x.DerLayout)
                    .Include(x => x.Artifact)
                    .Include(x => x.Artifact.Measurement)
                    .Include(x => x.Artifact.Series)
                    .Include(x => x.Artifact.Series.Select(y => y.Kpi))
                    .Include(x => x.Artifact.Series.Select(y => y.Kpi.Measurement))
                    /*.Include(x => x.Artifact.Charts)
                    .Include(x => x.Artifact.Charts.Select(y => y.Series))*/
                    .Include(x => x.Artifact.Charts.Select(y => y.Series.Select(z => z.Kpi)))
                    .Include(x => x.Artifact.Charts.Select(y => y.Measurement))
                    .Include(x => x.Artifact.Tank)
                    .Include(x => x.Artifact.Tank.VolumeInventory)
                    .Include(x => x.Artifact.Tank.DaysToTankTop)
                    .Include(x => x.Artifact.Tank.VolumeInventory.Measurement)
                    .Include(x => x.Artifact.Tank.DaysToTankTop.Measurement)
                    .Include(x => x.Artifact.CustomSerie)
                    .Include(x => x.Artifact.CustomSerie.Measurement)
                    .Include(x => x.Artifact.Plots)
                    .Include(x => x.Highlight)
                    .Include(x => x.Highlight.SelectOption)
                    .Include(x => x.KpiInformations.Select(y => y.SelectOption))
                    .Include(x => x.KpiInformations.Select(y => y.Kpi.Measurement))
                    .Include(x => x.SignedBy)
                    .Single(x => x.Id == id);

                response = derLayoutItem.MapTo<GetDerLayoutitemResponse>();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public DeleteDerLayoutItemResponse DeleteLayoutItem(int id, string type)
        {
            var response = new DeleteDerLayoutItemResponse();
            switch (type.ToLowerInvariant())
            {
                case "highlight":
                    {
                        try
                        {
                            var derLayoutItem = DataContext.DerLayoutItems
                                .Include(x => x.Highlight)
                                .Include(x => x.Highlight.SelectOption)
                                .Include(x => x.DerLayout)
                                .Single(x => x.Id == id);
                            response.DerLayoutId = derLayoutItem.DerLayout.Id;
                            DataContext.DerHighlights.Remove(derLayoutItem.Highlight);
                            DataContext.DerLayoutItems.Remove(derLayoutItem);
                            DataContext.SaveChanges();
                            response.IsSuccess = true;
                        }
                        catch (Exception exception)
                        {
                            response.Message = exception.Message;
                        }
                        break;
                    }
                case "safety":
                case "security":
                case "job-pmts":
                case "avg-ytd-key-statistic":
                case "temperature":
                case "lng-and-cds":
                case "total-feed-gas":
                case "table-tank":
                case "mgdp":
                case "hhv":
                case "lng-and-cds-production":
                case "weekly-maintenance":
                case "critical-pm":
                case "procurement":
                case "indicative-commercial-price":
                case "plant-availability":
                case "economic-indicator":
                case "loading-duration":
                case "key-equipment-status":
                    {
                        try
                        {
                            var derLayoutItem = DataContext.DerLayoutItems
                                .Include(x => x.KpiInformations)
                                .Include(x => x.DerLayout)
                                .Single(x => x.Id == id);
                            //var kpiInformations = new DerKpiInformation();
                            foreach (var item in derLayoutItem.KpiInformations.ToList())
                            {
                                var kpiInformation = DataContext.DerKpiInformations.Single(x => x.Id == item.Id);
                                DataContext.DerKpiInformations.Remove(kpiInformation);
                            }
                            response.DerLayoutId = derLayoutItem.DerLayout.Id;
                            DataContext.DerLayoutItems.Remove(derLayoutItem);
                            DataContext.SaveChanges();
                            response.IsSuccess = true;
                        }
                        catch (Exception exception)
                        {
                            response.Message = exception.Message;
                        }
                        break;
                    }
                case "multiaxis":
                    {
                        try
                        {
                            var derLayoutItem = DataContext.DerLayoutItems
                                .Include(x => x.Artifact)
                                .Include(x => x.DerLayout)
                                .Include(x => x.Artifact.Charts)
                                .Include(x => x.Artifact.CustomSerie)
                                .Single(x => x.Id == id);
                            //var kpiInformations = new DerKpiInformation();
                            //foreach (var item in derLayoutItem.KpiInformations.ToList())
                            //{
                            //    var kpiInformation = DataContext.DerKpiInformations.Single(x => x.Id == item.Id);
                            //    DataContext.DerKpiInformations.Remove(kpiInformation);
                            //}
                            response.DerLayoutId = derLayoutItem.DerLayout.Id;
                            DataContext.DerLayoutItems.Remove(derLayoutItem);
                            DataContext.SaveChanges();
                            response.IsSuccess = true;
                        }
                        catch (Exception exception)
                        {
                            response.Message = exception.Message;
                        }
                        break;
                    }
            }

            return response;
        }

        public GetKpiValueResponse GetKpiValue(GetKpiValueRequest request)
        {
            GetKpiValueResponse response = request.ConfigType == ConfigType.KpiTarget
                                               ? _kpiTargetService.GetKpiTarget(request.KpiId, request.Periode,
                                                                                request.RangeFilter)
                                                                  .MapTo<GetKpiValueResponse>()
                                               : _kpiAchievementService.GetKpiAchievement(request.KpiId, request.Periode,
                                                                                          request.RangeFilter)
                                                                       .MapTo<GetKpiValueResponse>();

            return response;
        }

        public SaveLayoutItemResponse SaveLayoutItem(SaveLayoutItemRequest request)
        {
            var baseResponse = new BaseResponse();
            switch (request.Type.ToLowerInvariant())
            {
                case "line":
                    {
                        baseResponse = request.Id > 0 ? UpdateLineChart(request) : SaveLineChart(request);
                        break;
                    }
                case "multiaxis":
                case "jcc-monthly-trend":
                    {
                        baseResponse = request.Id > 0 ? UpdateMultiAxis(request) : SaveMultiAxis(request);
                        break;
                    }
                case "pie":
                    {
                        baseResponse = request.Id > 0 ? UpdatePie(request) : SavePie(request);
                        break;
                    }
                case "tank":
                    {
                        baseResponse = request.Id > 0 ? UpdateTank(request) : SaveTank(request);
                        break;
                    }
                case "speedometer":
                case "barmeter":
                    {
                        baseResponse = request.Id > 0 ? UpdateSpeedometer(request) : SaveSpeedometer(request);
                        break;
                    }
                case "highlight":
                    {
                        baseResponse = request.Id > 0 ? UpdateHighlight(request) : SaveHighlight(request);
                        break;
                    }
                case "weather":
                case "alert":
                case "wave":
                case "nls":
                    {
                        baseResponse = SaveDynamicHighlight(request);
                        break;
                    }
                case "avg-ytd-key-statistic":
                case "temperature":
                case "safety":
                case "lng-and-cds":
                case "security":
                case "job-pmts":
                case "total-feed-gas":
                case "table-tank":
                case "mgdp":
                case "hhv":
                case "lng-and-cds-production":
                case "weekly-maintenance":
                case "critical-pm":
                case "procurement":
                case "indicative-commercial-price":
                case "plant-availability":
                case "economic-indicator":
                case "key-equipment-status":
                case "global-stock-market":
                case "dafwc":
                case "termometer":
                case "loading-duration":
                case "person-on-board":
                case "flare":
                case "total-commitment":
                case "no2":
                case "so2":
                case "ph":
                case "particulate":
                case "oil-grease":
                    {
                        baseResponse = request.Id > 0 ? UpdateKpiInformations(request) : SaveKpiInformations(request);
                        break;
                    }
                case "prepared-by":
                case "reviewed-by":
                    {
                        baseResponse = request.Id > 0 ? UpdateUser(request) : SaveUser(request);
                        break;

                    }
            }

            var response = new SaveLayoutItemResponse
            {
                IsSuccess = baseResponse.IsSuccess,
                Message = baseResponse.Message
            };
            return response;
        }

        public GetDerLayoutResponse GetDerLayout(int id)
        {
            var response = new GetDerLayoutResponse();
            try
            {
                var derLayout = DataContext.DerLayouts
                    .Include(x => x.Items)
                    .Include(x => x.Items.Select(y => y.DerLayout))
                    //.Include(x => x.Items.Select(y => y.DerLayout.Items))
                    /*.Include(x => x.Items.Select(y => y.Artifact))
                    .Include(x => x.Items.Select(y => y.Artifact.Measurement))
                    .Include(x => x.Items.Select(y => y.Artifact.Series))
                    .Include(x => x.Items.Select(y => y.Artifact.Series.Select(z => z.Kpi)))
                    .Include(x => x.Items.Select(y => y.Highlight))
                    .Include(x => x.Items.Select(y => y.Highlight.SelectOption))*/
                    .Single(x => x.Id == id);

                /*Include("PmsConfigs.Pillar")
                                            .Include("PmsConfigs.ScoreIndicators")
                                            .Include("PmsConfigs.PmsConfigDetailsList.Kpi.Measurement")
                                            .Include("PmsConfigs.PmsConfigDetailsList.Kpi.KpiAchievements")
                                            .Include("PmsConfigs.PmsConfigDetailsList.Kpi.KpiTargets")
                                            .Include("PmsConfigs.PmsConfigDetailsList.Kpi.Pillar")
                                            .Include("PmsConfigs.PmsConfigDetailsList.ScoreIndicators")*/

                response = derLayout.MapTo<GetDerLayoutResponse>();
                response.IsSuccess = true;

            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public GetOriginalDataResponse GetOriginalData(int layoutId, DateTime date)
        {
            IDictionary<string, List<string>> labels = new Dictionary<string, List<string>>();
            labels.Add("dafwc", new List<string>() { "Days Without DAFWC (since)", "Days Without LOPC (since)", "Safe Man-hours since Last DAFWC " });

            var response = new GetOriginalDataResponse();
            try
            {
                var der = DataContext.DerLayouts
                    //.Include(x => x.Items.Select(y => y.KpiInformations))
                    .Include(x => x.Items.Select(y => y.KpiInformations.Select(z => z.Kpi.Measurement)))
                    .Single(x => x.Id == layoutId);

                foreach (var item in der.Items)
                {
                    switch (item.Type)
                    {
                        case "dafwc":
                            {
                                DerLayoutItem item1 = item;
                                var list = DataContext.DerOriginalDatas
                                           .Include(x => x.LayoutItem)
                                           .Where(x => x.LayoutItem.Id == item1.Id && x.Periode.Day == date.Day && x.Periode.Month == date.Month &&
                                               x.Periode.Year == date.Year).ToList();

                                for (int i = 0; i <= 1; i++)
                                {
                                    var datum = (list.ElementAtOrDefault(i) != null)
                                                    ? list[i].MapTo<GetOriginalDataResponse.OriginalDataResponse>()
                                                    : new GetOriginalDataResponse.OriginalDataResponse
                                                    {
                                                        Periode = date,
                                                        PeriodeType = PeriodeType.Daily,
                                                        Position = i,
                                                        DataType = "datetime",
                                                        LayoutItemId = item.Id
                                                    };

                                    datum.Type = item.Type;
                                    datum.Label = labels.ContainsKey(item.Type.ToLowerInvariant()) ? labels[item.Type.ToLowerInvariant()][i] : "undefined";

                                    response.OriginalData.Add(datum);
                                }

                                break;
                            }
                        case "job-pmts":
                            {
                                for (int i = 0; i <= 2; i++)
                                {
                                    var datum = new GetOriginalDataResponse.OriginalDataResponse();
                                    var kpiInformation = item.KpiInformations.ElementAtOrDefault(i);
                                    if (kpiInformation != null)
                                    {
                                        datum.LayoutItemId = item.Id;
                                        datum.PeriodeType = PeriodeType.Daily;
                                        datum.Position = i;
                                        datum.DataType = "double";
                                        var kpiAchievement = DataContext.KpiAchievements.Include(x => x.Kpi).FirstOrDefault(x => x.PeriodeType == PeriodeType.Daily &&
                                                                            x.Kpi.Id == kpiInformation.Kpi.Id && (x.Periode.Day == date.Day && x.Periode.Month == date.Month &&
                                                                             x.Periode.Year == date.Year));
                                        datum.Data = (kpiAchievement != null && kpiAchievement.Value.HasValue) ? kpiAchievement.Value.ToString() : string.Empty;
                                        datum.Type = item.Type;
                                        datum.IsKpiAchievement = true;
                                        datum.Label = string.Format(@"{0} ({1})", kpiInformation.Kpi.Name, kpiInformation.Kpi.Measurement.Name);
                                        datum.KpiId = kpiInformation.Kpi.Id;
                                        datum.Periode = date;
                                        response.OriginalData.Add(datum);
                                    }
                                }
                                break;
                            }
                    }
                }

                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public SaveOriginalDataResponse SaveOriginalData(SaveOriginalDataRequest request)
        {
            var response = new SaveOriginalDataResponse();
            try
            {
                foreach (var datum in request.OriginalData)
                {
                    var layoutItem = new DerLayoutItem { Id = datum.LayoutItemId };
                    if (DataContext.DerLayoutItems.Local.FirstOrDefault(x => x.Id == layoutItem.Id) == null)
                    {
                        DataContext.DerLayoutItems.Attach(layoutItem);
                    }
                    else
                    {
                        layoutItem = DataContext.DerLayoutItems.Local.FirstOrDefault(x => x.Id == layoutItem.Id);
                    }

                    switch (datum.Type)
                    {
                        case "job-pmts":
                            {
                                if (datum.IsKpiAchievement)
                                {
                                    SaveOriginalDataRequest.OriginalDataRequest datum1 = datum;
                                    var kpi = DataContext.Kpis.Single(x => x.Id == datum1.KpiId);
                                    var kpiAchievements = DataContext.KpiAchievements
                                        .Include(x => x.Kpi)
                                        .Where(x => x.Kpi.Id == datum1.KpiId && ((x.Periode.Month == datum1.Periode.Month &&
                                                                             x.Periode.Year == datum1.Periode.Year) || x.Periode.Year == datum1.Periode.Year)).ToList();
                                    var kpiAchievementYearly =
                                        DataContext.KpiAchievements.Where(
                                            x => x.Periode.Year == 2016 && x.PeriodeType == PeriodeType.Yearly).ToList();
                                    var dailyActual = kpiAchievements.FirstOrDefault(x => x.PeriodeType == PeriodeType.Daily
                                        && x.Periode.Day == datum1.Periode.Day);

                                    if (!string.IsNullOrEmpty(datum1.Data))
                                    {
                                        double val;
                                        bool isParsed = double.TryParse(datum1.Data, out val);
                                        if (isParsed)
                                        {
                                            if (dailyActual != null)
                                            {
                                                dailyActual.Value = val;
                                            }
                                            else
                                            {
                                                dailyActual = new KpiAchievement
                                                {
                                                    Kpi = DataContext.Kpis.Single(x => x.Id == datum.KpiId),
                                                    Value = val
                                                };
                                                DataContext.KpiAchievements.Add(dailyActual);
                                            }
                                        }
                                    }

                                    var monthly = kpiAchievements.Where(x => x.PeriodeType == PeriodeType.Daily &&
                                                                              (x.Periode.Month == datum1.Periode.Month &&
                                                                               x.Periode.Year == datum1.Periode.Year))
                                                                  .AsQueryable();
                                    double? achievementMtd = null;
                                    if (kpi.YtdFormula == YtdFormula.Sum)
                                    {
                                        achievementMtd = monthly.Sum(x => x.Value);
                                    }
                                    else if (kpi.YtdFormula == YtdFormula.Average)
                                    {
                                        achievementMtd = monthly.Average(x => x.Value);
                                    }


                                    var monthlyActual = monthly.FirstOrDefault();

                                    if (monthlyActual != null)
                                    {
                                        monthlyActual.Value = achievementMtd;
                                    }

                                    var yearly = kpiAchievements.Where(x => x.PeriodeType == PeriodeType.Monthly && x.Periode.Year == datum1.Periode.Year)
                                                                  .AsQueryable();
                                    double? achievementYtd = null;
                                    if (kpi.YtdFormula == YtdFormula.Sum)
                                    {
                                        achievementYtd = yearly.Sum(x => x.Value);
                                    }
                                    else if (kpi.YtdFormula == YtdFormula.Average)
                                    {
                                        achievementYtd = yearly.Average(x => x.Value);
                                    }

                                    var yearlyActual = yearly.FirstOrDefault();
                                    if (yearlyActual != null)
                                    {
                                        yearlyActual.Value = achievementYtd;
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                if (datum.Id > 0)
                                {
                                    var originalData = datum.MapTo<DerOriginalData>();
                                    originalData.LayoutItem = layoutItem;
                                    DataContext.DerOriginalDatas.Attach(originalData);
                                    DataContext.Entry(originalData).State = EntityState.Modified;
                                }
                                else
                                {
                                    var originalData = datum.MapTo<DerOriginalData>();
                                    originalData.LayoutItem = layoutItem;
                                    DataContext.DerOriginalDatas.Add(originalData);
                                }
                                break;
                            }
                    }
                }

                //DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public GetDafwcDataResponse GetDafwcData(int id, DateTime date)
        {
            var response = new GetDafwcDataResponse();

            try
            {
                var derLayoutItem = DataContext.DerLayoutItems.Include(x => x.OriginalData).Single(x => x.Id == id);
                var daysWithoutDafwcData = derLayoutItem.OriginalData.FirstOrDefault(x => x.Position == 0 &&
                    (x.Periode.Day == date.Day && x.Periode.Month == date.Month && x.Periode.Year == date.Year));
                var daysWithoutLopcData = derLayoutItem.OriginalData.FirstOrDefault(x => x.Position == 1 &&
                    (x.Periode.Day == date.Day && x.Periode.Month == date.Month && x.Periode.Year == date.Year));

                if (daysWithoutDafwcData != null)
                {
                    DateTime dafwcDate;
                    bool isDate = DateTime.TryParse(daysWithoutDafwcData.Data, out dafwcDate);
                    if (isDate)
                    {
                        response.DaysWithoutDafwc = (date - dafwcDate).TotalDays.ToString(CultureInfo.InvariantCulture) + " days";
                        response.DaysWithoutDafwcSince = dafwcDate.ToShortDateString();
                    }
                }

                if (daysWithoutLopcData != null)
                {
                    DateTime lopcDate;
                    bool isDate = DateTime.TryParse(daysWithoutLopcData.Data, out lopcDate);
                    if (isDate)
                    {
                        response.DaysWithoutLopc = (date - lopcDate).TotalDays.ToString(CultureInfo.InvariantCulture) + " days";
                        response.DaysWithoutLopcSince = lopcDate.ToShortDateString();
                    }
                }

                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse SaveLineChart(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var derLayoutItem = new DerLayoutItem();
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derArtifact = new DerArtifact();
                derArtifact.GraphicType = request.Type;
                derArtifact.HeaderTitle = request.Artifact.HeaderTitle;

                var measurement = new Measurement { Id = request.Artifact.MeasurementId };
                DataContext.Measurements.Attach(measurement);
                derArtifact.Measurement = measurement;

                var series = request.Artifact.LineChart.Series.Select(x => new DerArtifactSerie
                {
                    Color = x.Color,
                    Kpi = DataContext.Kpis.FirstOrDefault(y => y.Id == x.KpiId),
                    Label = x.Label,
                    Artifact = derArtifact
                }).ToList();

                derArtifact.Series = series;
                DataContext.DerArtifacts.Add(derArtifact);
                derLayoutItem.Artifact = derArtifact;
                DataContext.DerLayoutItems.Add(derLayoutItem);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse SaveSpeedometer(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var derLayoutItem = new DerLayoutItem();
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derArtifact = new DerArtifact();
                derArtifact.GraphicType = request.Type;

                var plots = request.Artifact.Speedometer.PlotBands.Select(x => new DerArtifactPlot
                {
                    Color = x.Color,
                    From = x.From,
                    To = x.To
                }).ToList();
                derArtifact.Plots = plots;
                derArtifact.CustomSerie = DataContext.Kpis.FirstOrDefault(y => y.Id == request.Artifact.Speedometer.Series.KpiId);
                if (request.Artifact.Speedometer.LabelSeries != null)
                {
                    var labelSeries = new DerArtifactSerie
                    {
                        Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.Artifact.Speedometer.LabelSeries.KpiId),
                        Label = "ton/d",
                        Color = "#000"
                    };
                    derArtifact.Series = new List<DerArtifactSerie>();
                    derArtifact.Series.Add(labelSeries);
                }
                DataContext.DerArtifacts.Add(derArtifact);
                derLayoutItem.Artifact = derArtifact;
                DataContext.DerLayoutItems.Add(derLayoutItem);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse UpdateSpeedometer(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var derLayoutItem = DataContext.DerLayoutItems
                    .Include(x => x.Artifact)
                    .Include(x => x.Artifact.Plots)
                    .Include(x => x.Artifact.CustomSerie)
                    .Include(x => x.Artifact.Series)
                    .Single(x => x.Id == request.Id);

                //DataContext.DerArtifacts.Remove(derLayoutItem.Artifact);

                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derArtifact = new DerArtifact();
                derArtifact.GraphicType = request.Type;
                var plots = request.Artifact.Speedometer.PlotBands.Select(x => new DerArtifactPlot
                {
                    Color = x.Color,
                    From = x.From,
                    To = x.To
                }).ToList();

                derArtifact.Plots = plots;
                derArtifact.CustomSerie = DataContext.Kpis.FirstOrDefault(y => y.Id == request.Artifact.Speedometer.Series.KpiId);
                if (request.Artifact.Speedometer.LabelSeries != null)
                {
                    if (derArtifact.Series != null)
                    {
                        foreach (var serie in derArtifact.Series.ToList())
                        {
                            derArtifact.Series.Remove(serie);
                        }
                    }
                    else
                    {
                        derArtifact.Series = new List<DerArtifactSerie>();
                    }
                    var labelSeries = new DerArtifactSerie
                    {
                        Kpi = DataContext.Kpis.FirstOrDefault(x => x.Id == request.Artifact.Speedometer.LabelSeries.KpiId),
                        Label = "tonnes/day",
                        Color = "#000"
                    };
                    derArtifact.Series.Add(labelSeries);
                }

                DataContext.DerArtifacts.Add(derArtifact);
                derLayoutItem.Artifact = derArtifact;
                //DataContext.DerLayoutItems.Add(derLayoutItem);

                var oldArtifact = new DerArtifact { Id = request.Artifact.Id };
                if (DataContext.DerArtifacts.Local.FirstOrDefault(x => x.Id == oldArtifact.Id) == null)
                {
                    DataContext.DerArtifacts.Attach(oldArtifact);
                }
                else
                {
                    oldArtifact = DataContext.DerArtifacts.Local.FirstOrDefault(x => x.Id == oldArtifact.Id);
                }

                foreach (var plot in oldArtifact.Plots.ToList())
                {
                    DataContext.DerArtifactPlots.Remove(plot);
                }

                DataContext.DerArtifacts.Remove(oldArtifact);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse UpdateLineChart(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var derLayoutItem = DataContext.DerLayoutItems
                    .Include(x => x.Artifact)
                    .Include(x => x.Artifact.Measurement)
                    .Include(x => x.Artifact.Series)
                    .Single(x => x.Id == request.Id);

                //DataContext.DerArtifacts.Remove(derLayoutItem.Artifact);

                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derArtifact = new DerArtifact();
                derArtifact.GraphicType = request.Type;
                derArtifact.HeaderTitle = request.Artifact.HeaderTitle;
                var measurement = new Measurement { Id = request.Artifact.MeasurementId };
                if (DataContext.Measurements.Local.FirstOrDefault(x => x.Id == measurement.Id) == null)
                {
                    DataContext.Measurements.Attach(measurement);
                }
                else
                {
                    measurement = DataContext.Measurements.Local.FirstOrDefault(x => x.Id == measurement.Id);
                }

                derArtifact.Measurement = measurement;
                var series = request.Artifact.LineChart.Series.Select(x => new DerArtifactSerie
                {
                    Color = x.Color,
                    Kpi = DataContext.Kpis.FirstOrDefault(y => y.Id == x.KpiId),
                    Label = x.Label,
                    Artifact = derArtifact
                }).ToList();

                derArtifact.Series = series;
                DataContext.DerArtifacts.Add(derArtifact);
                derLayoutItem.Artifact = derArtifact;
                //DataContext.DerLayoutItems.Add(derLayoutItem);

                var oldArtifact = new DerArtifact { Id = request.Artifact.Id };
                if (DataContext.DerArtifacts.Local.FirstOrDefault(x => x.Id == oldArtifact.Id) == null)
                {
                    DataContext.DerArtifacts.Attach(oldArtifact);
                }
                else
                {
                    oldArtifact = DataContext.DerArtifacts.Local.FirstOrDefault(x => x.Id == oldArtifact.Id);
                }

                DataContext.DerArtifacts.Remove(oldArtifact);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse SaveMultiAxis(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {

                var derLayoutItem = request.MapTo<DerLayoutItem>();// new DerLayoutItem();
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                //derLayoutItem.Column = request.Column;
                //derLayoutItem.Row = request.Row;
                //derLayoutItem.Type = request.Type;
                var derArtifact = request.MapTo<DerArtifact>();
                //derArtifact.GraphicType = request.Type;
                //derArtifact.HeaderTitle = request.Artifact.HeaderTitle;
                /*var measurement = new Measurement { Id = request.Artifact.MeasurementId };
                DataContext.Measurements.Attach(measurement);
                derArtifact.Measurement = measurement;*/

                derArtifact.Charts = new List<DerArtifactChart>();
                foreach (var item in request.Artifact.MultiAxis.Charts)
                {
                    var chart = item.MapTo<DerArtifactChart>();

                    var measurement = new Measurement { Id = item.MeasurementId };
                    if (DataContext.Measurements.Local.FirstOrDefault(x => x.Id == measurement.Id) == null)
                    {
                        DataContext.Measurements.Attach(measurement);
                    }
                    else
                    {
                        measurement = DataContext.Measurements.Local.FirstOrDefault(x => x.Id == measurement.Id);
                    }

                    DataContext.Measurements.Attach(measurement);
                    chart.Measurement = measurement;

                    foreach (var s in item.Series)
                    {
                        var serie = s.MapTo<DerArtifactSerie>();
                        var kpi = new Kpi { Id = s.KpiId };
                        if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id) == null)
                        {
                            DataContext.Kpis.Attach(kpi);
                        }
                        else
                        {
                            kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id);
                        }
                        serie.Kpi = kpi;
                        serie.Artifact = derArtifact;
                        chart.Series.Add(serie);
                    }

                    derArtifact.Charts.Add(chart);
                }

                derLayoutItem.Artifact = derArtifact;
                //DataContext.DerArtifacts.Add(derArtifact);
                DataContext.DerLayoutItems.Add(derLayoutItem);

                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Multiaxis has been configured";
                /*var charts = request.Artifact.MultiAxis.Charts.Select(x => new DerArtifactChart
                    {
                        FractionScale = x.FractionScale,
                        GraphicType = x.GraphicType,
                        IsOpposite = x.IsOpposite,
                        MaxFractionScale = x.MaxFractionScale,
                        Measurement = DataContext.Measurements.Single(x => x.)
                    })*/
                /* var series = request.Artifact.LineChart.Series.Select(x => new DerArtifactSerie
                {
                    Color = x.Color,
                    Kpi = DataContext.Kpis.FirstOrDefault(y => y.Id == x.KpiId),
                    Label = x.Label
                }).ToList();

                derArtifact.Series = series;
                DataContext.DerArtifacts.Add(derArtifact);
                derLayoutItem.Artifact = derArtifact;
                DataContext.DerLayoutItems.Add(derLayoutItem);*/

            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse UpdateMultiAxis(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var derLayoutItem = DataContext.DerLayoutItems
                   .Include(x => x.Artifact)
                   .Include(x => x.Artifact.Measurement)
                   .Include(x => x.Artifact.Series)
                   .Single(x => x.Id == request.Id);

                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derArtifact = new DerArtifact();
                derArtifact.GraphicType = request.Type;
                derArtifact.HeaderTitle = request.Artifact.HeaderTitle;
                /*var measurement = new Measurement { Id = request.Artifact.MeasurementId };
                DataContext.Measurements.Attach(measurement);
                derArtifact.Measurement = measurement;*/

                derArtifact.Charts = new List<DerArtifactChart>();
                foreach (var item in request.Artifact.MultiAxis.Charts)
                {
                    var chart = item.MapTo<DerArtifactChart>();

                    var measurement = new Measurement { Id = item.MeasurementId };
                    if (DataContext.Measurements.Local.FirstOrDefault(x => x.Id == measurement.Id) == null)
                    {
                        DataContext.Measurements.Attach(measurement);
                    }
                    else
                    {
                        measurement = DataContext.Measurements.Local.FirstOrDefault(x => x.Id == measurement.Id);
                    }

                    DataContext.Measurements.Attach(measurement);
                    chart.Measurement = measurement;

                    foreach (var s in item.Series)
                    {
                        var serie = s.MapTo<DerArtifactSerie>();
                        var kpi = new Kpi { Id = s.KpiId };
                        if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id) == null)
                        {
                            DataContext.Kpis.Attach(kpi);
                        }
                        else
                        {
                            kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id);
                        }
                        serie.Kpi = kpi;
                        serie.Artifact = derArtifact;
                        chart.Series.Add(serie);
                    }

                    derArtifact.Charts.Add(chart);
                }

                derLayoutItem.Artifact = derArtifact;
                //DataContext.DerArtifacts.Add(derArtifact);
                //DataContext.DerLayoutItems.Add(derLayoutItem);

                var oldArtifact = DataContext.DerArtifacts
                                             .Include(x => x.Charts)
                                             .Include(x => x.Charts.Select(y => y.Series))
                                             .Single(x => x.Id == request.Artifact.Id);

                foreach (var chart in oldArtifact.Charts.ToList())
                {

                    foreach (var series in chart.Series.ToList())
                    {
                        DataContext.DerArtifactSeries.Remove(series);
                    }
                    DataContext.DerArtifactCharts.Remove(chart);
                }

                DataContext.DerArtifacts.Remove(oldArtifact);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse SavePie(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var derLayoutItem = new DerLayoutItem();
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derArtifact = request.MapTo<DerArtifact>();
                derArtifact.ShowLegend = request.Artifact.ShowLegend;
                derArtifact.Is3D = request.Artifact.Is3D;
                derArtifact.Charts = new List<DerArtifactChart>();

                var measurement = new Measurement { Id = request.Artifact.MeasurementId };
                DataContext.Measurements.Attach(measurement);
                derArtifact.Measurement = measurement;
                var series = request.Artifact.Pie.Series.Select(x => new DerArtifactSerie
                {
                    Color = x.Color,
                    Label = x.Label,
                    Kpi = DataContext.Kpis.FirstOrDefault(y => y.Id == x.KpiId),
                    Artifact = derArtifact
                }).ToList();

                derArtifact.Series = series;
                DataContext.DerArtifacts.Add(derArtifact);
                derLayoutItem.Artifact = derArtifact;
                DataContext.DerLayoutItems.Add(derLayoutItem);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse UpdatePie(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var derLayoutItem = DataContext.DerLayoutItems
                   .Include(x => x.Artifact)
                   .Include(x => x.Artifact.Measurement)
                   .Include(x => x.Artifact.Series)
                   .Single(x => x.Id == request.Id);

                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derArtifact = new DerArtifact();
                derArtifact.ShowLegend = request.Artifact.ShowLegend;
                derArtifact.Is3D = request.Artifact.Is3D;
                derArtifact.HeaderTitle = request.Artifact.HeaderTitle;
                derArtifact.GraphicType = request.Type;
                derArtifact.Charts = new List<DerArtifactChart>();

                var measurement = new Measurement { Id = request.Artifact.MeasurementId };
                if (DataContext.Measurements.Local.FirstOrDefault(x => x.Id == measurement.Id) == null)
                {
                    DataContext.Measurements.Attach(measurement);
                }
                else
                {
                    measurement = DataContext.Measurements.Local.FirstOrDefault(x => x.Id == measurement.Id);
                }

                derArtifact.Measurement = measurement;
                var series = request.Artifact.Pie.Series.Select(x => new DerArtifactSerie
                {
                    Color = x.Color,
                    Label = x.Label,
                    Kpi = DataContext.Kpis.FirstOrDefault(y => y.Id == x.KpiId),
                    Artifact = derArtifact
                }).ToList();

                derArtifact.Series = series;
                DataContext.DerArtifacts.Add(derArtifact);
                derLayoutItem.Artifact = derArtifact;
                //DataContext.DerLayoutItems.Add(derLayoutItem);

                var oldArtifact = new DerArtifact { Id = request.Artifact.Id };
                if (DataContext.DerArtifacts.Local.FirstOrDefault(x => x.Id == oldArtifact.Id) == null)
                {
                    DataContext.DerArtifacts.Attach(oldArtifact);
                }
                else
                {
                    oldArtifact = DataContext.DerArtifacts.Local.FirstOrDefault(x => x.Id == oldArtifact.Id);
                }

                DataContext.DerArtifacts.Remove(oldArtifact);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse SaveTank(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {

                var derLayoutItem = new DerLayoutItem();
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derArtifact = request.MapTo<DerArtifact>();
                derLayoutItem.Artifact = derArtifact;
                derLayoutItem.Artifact.Tank = request.Artifact.Tank.MapTo<DerArtifactTank>();
                var volumeInventory = new Kpi { Id = request.Artifact.Tank.VolumeInventoryId };
                if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == volumeInventory.Id) == null)
                {
                    DataContext.Kpis.Attach(volumeInventory);
                }
                else
                {
                    volumeInventory = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == request.Artifact.Tank.VolumeInventoryId);
                }

                var daysToTankTop = new Kpi { Id = request.Artifact.Tank.DaysToTankTopId };
                if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == daysToTankTop.Id) == null)
                {
                    DataContext.Kpis.Attach(daysToTankTop);
                }
                else
                {
                    daysToTankTop = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == request.Artifact.Tank.DaysToTankTopId);
                }

                derLayoutItem.Artifact.Tank.VolumeInventory = volumeInventory;
                derLayoutItem.Artifact.Tank.DaysToTankTop = daysToTankTop;
                DataContext.DerArtifacts.Add(derArtifact);

                DataContext.DerLayoutItems.Add(derLayoutItem);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse UpdateTank(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();

            try
            {
                var derLayoutItem = DataContext.DerLayoutItems
                    .Include(x => x.Artifact)
                    .Include(x => x.Artifact.Tank)
                    .Single(x => x.Id == request.Id);

                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derArtifact = new DerArtifact();//request.MapTo<DerArtifact>();
                derArtifact.HeaderTitle = request.Artifact.HeaderTitle;
                derArtifact.GraphicType = request.Type;
                derLayoutItem.Artifact = derArtifact;
                derLayoutItem.Artifact.Tank = request.Artifact.Tank.MapTo<DerArtifactTank>();

                var volumeInventory = new Kpi { Id = request.Artifact.Tank.VolumeInventoryId };
                if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == volumeInventory.Id) == null)
                {
                    DataContext.Kpis.Attach(volumeInventory);
                }
                else
                {
                    volumeInventory = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == request.Artifact.Tank.VolumeInventoryId);
                }

                var daysToTankTop = new Kpi { Id = request.Artifact.Tank.DaysToTankTopId };
                if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == daysToTankTop.Id) == null)
                {
                    DataContext.Kpis.Attach(daysToTankTop);
                }
                else
                {
                    daysToTankTop = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == request.Artifact.Tank.DaysToTankTopId);
                }

                derLayoutItem.Artifact.Tank.VolumeInventory = volumeInventory;
                derLayoutItem.Artifact.Tank.DaysToTankTop = daysToTankTop;
                DataContext.DerArtifacts.Add(derArtifact);
                //DataContext.DerLayoutItems.Add(derLayoutItem);

                var oldArtifact = new DerArtifact { Id = request.Artifact.Id };
                if (DataContext.DerArtifacts.Local.FirstOrDefault(x => x.Id == oldArtifact.Id) == null)
                {
                    DataContext.DerArtifacts.Attach(oldArtifact);
                }
                else
                {
                    oldArtifact = DataContext.DerArtifacts.Local.FirstOrDefault(x => x.Id == oldArtifact.Id);
                }

                if (oldArtifact.Tank != null) DataContext.DerArtifactTanks.Remove(oldArtifact.Tank);
                DataContext.DerArtifacts.Remove(oldArtifact);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse SaveHighlight(SaveLayoutItemRequest request)
        {
            var response = new GetDerLayoutResponse();
            try
            {
                var derLayoutItem = new DerLayoutItem();
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derHiglight = new DerHighlight();
                var selectOption = new SelectOption { Id = request.Highlight.SelectOptionId };
                DataContext.SelectOptions.Attach(selectOption);
                derHiglight.SelectOption = selectOption;
                derLayoutItem.Highlight = derHiglight;
                DataContext.DerHighlights.Add(derHiglight);
                DataContext.DerLayoutItems.Add(derLayoutItem);


                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse UpdateHighlight(SaveLayoutItemRequest request)
        {
            var response = new GetDerLayoutResponse();
            try
            {
                var derLayoutItem = DataContext.DerLayoutItems.Include(x => x.Highlight).Include(x => x.Highlight.SelectOption).Single(x => x.Id == request.Id);
                var selectOption = new SelectOption { Id = request.Highlight.SelectOptionId };
                DataContext.SelectOptions.Attach(selectOption);
                derLayoutItem.Highlight.SelectOption = selectOption;
                DataContext.Entry(derLayoutItem).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse SaveDynamicHighlight(SaveLayoutItemRequest request)
        {
            var response = new GetDerLayoutResponse();
            try
            {
                var derLayoutItem = new DerLayoutItem();
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var derStaticHighlight = new DerStaticHighlight();
                derStaticHighlight.Type = request.Type;
                derLayoutItem.StaticHighlight = derStaticHighlight;
                DataContext.DerStaticHighlights.Add(derStaticHighlight);
                DataContext.DerLayoutItems.Add(derLayoutItem);

                DataContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse UpdateKpiInformations(SaveLayoutItemRequest request)
        {
            var response = new GetDerLayoutResponse();
            try
            {

                var derLayoutItem = DataContext.DerLayoutItems.Include(x => x.KpiInformations).Single(x => x.Id == request.Id);
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var kpiInformations = new List<DerKpiInformation>();
                foreach (var item in request.KpiInformations)
                {
                    var kpiInformation = DataContext.DerKpiInformations.SingleOrDefault(x => x.Id == item.Id);
                    if (kpiInformation != null)
                    {
                        DataContext.DerKpiInformations.Remove(kpiInformation);
                    }

                    if (item.KpiId > 0)
                    {
                        var kpi = new Kpi { Id = item.KpiId };
                        if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id) == null)
                        {
                            DataContext.Kpis.Attach(kpi);
                        }
                        else
                        {
                            kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id);
                        }
                        var newKpiInformation = item.MapTo<DerKpiInformation>();
                        newKpiInformation.Kpi = kpi;
                        kpiInformations.Add(newKpiInformation);
                    }
                    else if (item.HighlightId > 0)
                    {
                        var selectOption = new SelectOption { Id = item.HighlightId };
                        if (DataContext.SelectOptions.Local.FirstOrDefault(x => x.Id == selectOption.Id) == null)
                        {
                            DataContext.SelectOptions.Attach(selectOption);
                        }
                        else
                        {
                            selectOption =
                                DataContext.SelectOptions.Local.FirstOrDefault(x => x.Id == selectOption.Id);
                        }
                        kpiInformations.Add(new DerKpiInformation
                        {
                            SelectOption = selectOption,
                            Position = item.Position,
                            ConfigType = item.ConfigType
                        });
                    }

                    /*if (kpiInformation != null)
                    {
                        DataContext.DerKpiInformations.Remove(kpiInformation);
                        if (item.KpiId > 0)
                        {
                            var kpi = new Kpi { Id = item.KpiId };
                            if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id) == null)
                            {
                                DataContext.Kpis.Attach(kpi);
                            }
                            else
                            {
                                kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id);
                            }
                            var newKpiInformation = item.MapTo<DerKpiInformation>();
                            newKpiInformation.Kpi = kpi;
                            kpiInformations.Add(newKpiInformation);
                        }
                        else if (item.HighlightId > 0)
                        {
                            var selectOption = new SelectOption { Id = item.HighlightId };
                            if (DataContext.SelectOptions.Local.FirstOrDefault(x => x.Id == selectOption.Id) == null)
                            {
                                DataContext.SelectOptions.Attach(selectOption);
                            }
                            else
                            {
                                selectOption =
                                    DataContext.SelectOptions.Local.FirstOrDefault(x => x.Id == selectOption.Id);
                            }
                            kpiInformations.Add(new DerKpiInformation
                            {
                                SelectOption = selectOption,
                                Position = item.Position,
                                ConfigType = item.ConfigType
                            });
                        }
                    }
                    else
                    {
                        if (item.KpiId > 0)
                        {
                            var kpi = new Kpi { Id = item.KpiId };
                            if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id) == null)
                            {
                                DataContext.Kpis.Attach(kpi);
                            }
                            else
                            {
                                kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id);
                            }
                            var newKpiInformation = item.MapTo<DerKpiInformation>();
                            newKpiInformation.Kpi = kpi;
                            kpiInformations.Add(newKpiInformation);
                            //kpiInformations.Add(new DerKpiInformation { Kpi = kpi, Position = item.Position, ConfigType = item.ConfigType, KpiLabel = item.KpiLabel, KpiMeasurement = item.KpiMeasurement});
                        }
                        else if (item.HighlightId > 0)
                        {
                            var selectOption = new SelectOption { Id = item.HighlightId };
                            if (DataContext.SelectOptions.Local.FirstOrDefault(x => x.Id == selectOption.Id) == null)
                            {
                                DataContext.SelectOptions.Attach(selectOption);
                            }
                            else
                            {
                                selectOption =
                                    DataContext.SelectOptions.Local.FirstOrDefault(x => x.Id == selectOption.Id);
                            }
                            kpiInformations.Add(new DerKpiInformation
                            {
                                SelectOption = selectOption,
                                Position = item.Position,
                                ConfigType = item.ConfigType
                            });
                        }
                    }*/

                }
                derLayoutItem.KpiInformations = kpiInformations;
                //DataContext.DerLayoutItems.Add(derLayoutItem);
                DataContext.Entry(derLayoutItem).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Changes has been saved";
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse SaveKpiInformations(SaveLayoutItemRequest request)
        {
            var response = new GetDerLayoutResponse();
            try
            {

                var derLayoutItem = new DerLayoutItem();
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                var kpiInformations = new List<DerKpiInformation>();
                foreach (var item in request.KpiInformations)
                {
                    if (item.KpiId > 0)
                    {
                        var kpi = new Kpi { Id = item.KpiId };
                        if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id) == null)
                        {
                            DataContext.Kpis.Attach(kpi);
                        }
                        else
                        {
                            kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpi.Id);
                        }
                        kpiInformations.Add(new DerKpiInformation { Kpi = kpi, Position = item.Position, ConfigType = item.ConfigType });
                    }
                    else if (item.HighlightId > 0)
                    {
                        var selectOption = new SelectOption { Id = item.HighlightId };
                        if (DataContext.SelectOptions.Local.FirstOrDefault(x => x.Id == selectOption.Id) == null)
                        {
                            DataContext.SelectOptions.Attach(selectOption);
                        }
                        else
                        {
                            selectOption = DataContext.SelectOptions.Local.FirstOrDefault(x => x.Id == selectOption.Id);
                        }
                        kpiInformations.Add(new DerKpiInformation { SelectOption = selectOption, Position = item.Position, ConfigType = item.ConfigType });
                    }

                }

                derLayoutItem.KpiInformations = kpiInformations;
                DataContext.DerLayoutItems.Add(derLayoutItem);

                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Changes has been saved";
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse SaveUser(SaveLayoutItemRequest request)
        {
            var response = new GetDerLayoutResponse();
            try
            {
                var derLayoutItem = new DerLayoutItem();
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                derLayoutItem.SignedBy = DataContext.Users.Single(x => x.Id == request.SignedBy);

                DataContext.DerLayoutItems.Add(derLayoutItem);

                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Changes has been saved";
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        private BaseResponse UpdateUser(SaveLayoutItemRequest request)
        {
            var response = new GetDerLayoutResponse();
            try
            {
                var derLayoutItem = DataContext.DerLayoutItems.Include(x => x.KpiInformations).Single(x => x.Id == request.Id);
                var derLayout = new DerLayout { Id = request.DerLayoutId };
                DataContext.DerLayouts.Attach(derLayout);
                derLayoutItem.DerLayout = derLayout;
                derLayoutItem.Column = request.Column;
                derLayoutItem.Row = request.Row;
                derLayoutItem.Type = request.Type;
                derLayoutItem.SignedBy = DataContext.Users.Single(x => x.Id == request.SignedBy);

                DataContext.Entry(derLayoutItem).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Changes has been saved";
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        /* private BaseResponse SaveDafwc(SaveLayoutItemRequest request)
             {
                 var response = new GetDerLayoutResponse();
                 try
                 {
                     if (request.Id > 0)
                     {

                     }
                     else
                     {
                         var derLayoutItem = new DerLayoutItem();
                         var derLayout = new DerLayout { Id = request.DerLayoutId };
                         DataContext.DerLayouts.Attach(derLayout);
                         derLayoutItem.DerLayout = derLayout;
                         derLayoutItem.Column = request.Column;
                         derLayoutItem.Row = request.Row;
                         derLayoutItem.Type = request.Type;
                         DataContext.DerLayoutItems.Add(derLayoutItem);
                     }

                     DataContext.SaveChanges();
                     response.IsSuccess = true;
                 }
                 catch (Exception exception)
                 {
                     response.Message = exception.Message;
                 }

                 return response;
             }*/

        public GetDerResponse GetDerById(int id)
        {
            var der = DataContext.Ders.Single(x => x.Id == id);
            return new GetDerResponse
            {
                Id = der.Id,
                Title = der.Title
            };
        }

        public bool IsDerExisted(DateTime date, out int revision)
        {
            bool isExisted = false;
            var der = DataContext.Ders.FirstOrDefault(x => x.Date.Year == date.Year && x.Date.Month == date.Month && x.Date.Day == date.Day);
            revision = der != null ? der.Revision : 0;
            return isExisted = der != null;
        }

        public BaseResponse DeleteFilename(string filename, DateTime date)
        {
            var response = new CreateOrUpdateResponse();
            try
            {
                var existingDer = DataContext.Ders.FirstOrDefault(s => s.Date == date);
                var filenames = existingDer.Filename.Split(';').ToList();
                var fileToRemove = filenames.FirstOrDefault(x => x.Contains(filename));
                filenames.Remove(fileToRemove);
                if (filenames.Count == 0)
                {
                    DataContext.Ders.Remove(existingDer);
                    DataContext.SaveChanges();
                    response.IsSuccess = true;
                    response.Message = "File Attachment has been Deleted successfully";
                    return response;
                }
                var lastFile = filenames.Last();
                var regex = new Regex(@"_(\d+)\.pdf");
                Match match = regex.Match(lastFile);
                Regex versionRegex = new Regex(@"\d+");
                Match versionMatch = versionRegex.Match(match.Value);
                existingDer.Revision = int.Parse(versionMatch.Value);
                existingDer.Filename = string.Join(";", filenames);

                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "File Attachment has been Deleted successfully";
            }
            catch (Exception exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }

            return response;
        }

        private GetDersResponse GetCustomHighlights(GetDersRequest request, IList<Der> data)
        {
            var response = new GetDersResponse();
            var highlights = DataContext.Highlights.Include(x => x.HighlightType).AsQueryable();
            string trafficLight = string.Empty;
            if (!string.IsNullOrEmpty(request.Search) && !string.IsNullOrWhiteSpace(request.Search))
            {
                var srch = request.Search.Split('-');
                var month = int.Parse(srch[0].Trim());
                var year = int.Parse(srch[1].Trim());
                trafficLight = srch.Length > 2 ? srch[2].Trim() : string.Empty;
                if (month == 0)
                {
                    var lastDay = new DateTime(year, 1, 1);
                    highlights = highlights.Where(x => x.Date.Year == year || x.Date == lastDay);
                }
                else
                {
                    //var earlyNextMonth = new DateTime();
                    //if (month < 12)
                    //{
                    //    earlyNextMonth = new DateTime(year, month + 1, 1);
                    //}
                    //else if (month == 12)
                    //{
                    //    earlyNextMonth = new DateTime(year + 1, 1, 1);
                    //}

                    //var earlyThisMonth = new DateTime(year, month, 1);
                    highlights = highlights.Where(x => (x.Date.Year == year && x.Date.Month == month));
                }
            }
            int overallPerformanceId = 8;
            int dailyIndicatorId = 58;
            int marineCargoDeliveryId = 52;
            int qhseRemarkId = 18;
            int securityRemarkId = 13;
            int plantModeId = 69;
            var listHighlight = highlights.Where(x => x.HighlightType.Id == overallPerformanceId || x.HighlightType.Id == marineCargoDeliveryId
            || x.HighlightType.Id == qhseRemarkId || x.HighlightType.Id == securityRemarkId || x.HighlightType.Id == dailyIndicatorId || x.HighlightType.Id == plantModeId).ToList();
            var der = new GetDersResponse.Der();
            var activityDate = new DateTime();
            foreach (var item in data)
            {
                der = item.MapTo<GetDersResponse.Der>();
                activityDate = der.Date.AddDays(-1);
                der.OverallPerformance = listHighlight.Where(x => x.Date == activityDate && x.HighlightType != null && x.HighlightType.Id == overallPerformanceId).DefaultIfEmpty(new Highlight { Message = string.Empty }).First().Message;
                der.MarineCargoDelivery = listHighlight.Where(x => x.Date == activityDate && x.HighlightType != null && x.HighlightType.Id == marineCargoDeliveryId).DefaultIfEmpty(new Highlight { Message = string.Empty }).First().Message;
                der.Qhse = listHighlight.Where(x => x.Date == activityDate && x.HighlightType != null && x.HighlightType.Id == qhseRemarkId).DefaultIfEmpty(new Highlight { Message = string.Empty }).First().Message;
                der.Security = listHighlight.Where(x => x.Date == activityDate && x.HighlightType != null && x.HighlightType.Id == securityRemarkId).DefaultIfEmpty(new Highlight { Message = string.Empty }).First().Message;
                der.DailyIndicator = listHighlight.Where(x => x.Date == activityDate && x.HighlightType != null && x.HighlightType.Id == dailyIndicatorId).DefaultIfEmpty(new Highlight { Message = string.Empty }).First().Message;
                der.PlantMode = listHighlight.Where(x => x.Date == activityDate && x.HighlightType != null && x.HighlightType.Id == plantModeId).DefaultIfEmpty(new Highlight { Message = string.Empty }).First().Message;
                der.Date = activityDate;
                response.Ders.Add(der);
            }

            if (!string.IsNullOrEmpty(trafficLight))
            {
                response.Ders = response.Ders.Where(x => x.DailyIndicator.Trim() == trafficLight).ToList();
            }

            foreach (var sortOrder in request.SortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "DailyIndicator":
                        response.Ders = sortOrder.Value == SortOrder.Ascending
                            ? response.Ders.OrderBy(x => x.DailyIndicator).ToList()
                            : response.Ders.OrderByDescending(x => x.DailyIndicator).ToList();
                        break;
                }
            }

            return response;
        }

        public BaseResponse DeleteLayout(DeleteDerlayoutRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var derLayout = DataContext.DerLayouts
                .Include(x => x.Items)
                .Single(x => x.Id == request.Id);
                derLayout.IsDeleted = true;
                DataContext.Entry(derLayout).State = EntityState.Modified;
                DataContext.SaveChanges(action);
                response.IsSuccess = true;
            }
            catch (Exception exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }

            return response;
        }

        public DeleteDerLayoutItemResponse DeleteLayoutItem(DeleteDerLayoutItemRequest request)
        {
            var response = new DeleteDerLayoutItemResponse();
            var action = request.MapTo<BaseAction>();
            switch (request.Type.ToLowerInvariant())
            {
                case "highlight":
                    {
                        try
                        {
                            action.ActionName = string.Format("{0}-{1}", action.ActionName, request.Type.ToLowerInvariant());
                            var derLayoutItem = DataContext.DerLayoutItems
                                .Include(x => x.Highlight)
                                .Include(x => x.Highlight.SelectOption)
                                .Include(x => x.DerLayout)
                                .Single(x => x.Id == request.Id);
                            response.DerLayoutId = derLayoutItem.DerLayout.Id;
                            DataContext.DerHighlights.Remove(derLayoutItem.Highlight);
                            DataContext.DerLayoutItems.Remove(derLayoutItem);
                            DataContext.SaveChanges(action);
                            response.IsSuccess = true;
                        }
                        catch (Exception exception)
                        {
                            response.Message = exception.Message;
                        }
                        break;
                    }
                case "safety":
                case "security":
                case "job-pmts":
                case "avg-ytd-key-statistic":
                case "temperature":
                case "lng-and-cds":
                case "total-feed-gas":
                case "table-tank":
                case "mgdp":
                case "hhv":
                case "lng-and-cds-production":
                case "weekly-maintenance":
                case "critical-pm":
                case "procurement":
                case "indicative-commercial-price":
                case "plant-availability":
                case "economic-indicator":
                case "loading-duration":
                case "key-equipment-status":
                    {
                        try
                        {
                            action.ActionName = string.Format("{0}-{1}", action.ActionName, request.Type.ToLowerInvariant());
                            var derLayoutItem = DataContext.DerLayoutItems
                                .Include(x => x.KpiInformations)
                                .Include(x => x.DerLayout)
                                .Single(x => x.Id == request.Id);
                            //var kpiInformations = new DerKpiInformation();
                            foreach (var item in derLayoutItem.KpiInformations.ToList())
                            {
                                var kpiInformation = DataContext.DerKpiInformations.Single(x => x.Id == item.Id);
                                DataContext.DerKpiInformations.Remove(kpiInformation);
                            }
                            response.DerLayoutId = derLayoutItem.DerLayout.Id;
                            DataContext.DerLayoutItems.Remove(derLayoutItem);
                            DataContext.SaveChanges(action);
                            response.IsSuccess = true;
                        }
                        catch (Exception exception)
                        {
                            response.Message = exception.Message;
                        }
                        break;
                    }
                case "multiaxis":
                    {
                        try
                        {
                            action.ActionName = string.Format("{0}-{1}", action.ActionName, request.Type.ToLowerInvariant());
                            var derLayoutItem = DataContext.DerLayoutItems
                                .Include(x => x.Artifact)
                                .Include(x => x.DerLayout)
                                .Include(x => x.Artifact.Charts)
                                .Include(x => x.Artifact.CustomSerie)
                                .Single(x => x.Id == request.Id);
                            //var kpiInformations = new DerKpiInformation();
                            //foreach (var item in derLayoutItem.KpiInformations.ToList())
                            //{
                            //    var kpiInformation = DataContext.DerKpiInformations.Single(x => x.Id == item.Id);
                            //    DataContext.DerKpiInformations.Remove(kpiInformation);
                            //}
                            response.DerLayoutId = derLayoutItem.DerLayout.Id;
                            DataContext.DerLayoutItems.Remove(derLayoutItem);
                            DataContext.SaveChanges(action);
                            response.IsSuccess = true;
                        }
                        catch (Exception exception)
                        {
                            response.Message = exception.Message;
                        }
                        break;
                    }
            }

            return response;
        }

        public BaseResponse DeleteFilename(DeleteFilenameRequest request)
        {
            var response = new CreateOrUpdateResponse();
            try
            {
                var action = request.MapTo<BaseAction>();
                var existingDer = DataContext.Ders.FirstOrDefault(s => s.Date == request.date);
                var filenames = existingDer.Filename.Split(';').ToList();
                var fileToRemove = filenames.FirstOrDefault(x => x.Contains(request.filename));
                filenames.Remove(fileToRemove);
                if (filenames.Count == 0)
                {
                    DataContext.Ders.Remove(existingDer);
                    DataContext.SaveChanges(action);
                    response.IsSuccess = true;
                    response.Message = "File Attachment has been Deleted successfully";
                    return response;
                }
                var lastFile = filenames.Last();
                var regex = new Regex(@"_(\d+)\.pdf");
                Match match = regex.Match(lastFile);
                Regex versionRegex = new Regex(@"\d+");
                Match versionMatch = versionRegex.Match(match.Value);
                existingDer.Revision = int.Parse(versionMatch.Value);
                existingDer.Filename = string.Join(";", filenames);

                DataContext.SaveChanges(action);
                response.IsSuccess = true;
                response.Message = "File Attachment has been Deleted successfully";
            }
            catch (Exception exception)
            {
                response.IsSuccess = false;
                response.Message = exception.Message;
            }

            return response;
        }
    }
}
