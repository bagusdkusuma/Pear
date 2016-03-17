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

namespace DSLNG.PEAR.Services
{
    public class DerService : BaseService, IDerService
    {
        public DerService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public GetDersResponse GetDers()
        {
            var ders = DataContext.Ders.ToList();
            return new GetDersResponse
            {
                IsSuccess = true,
                Ders = ders.ToList().MapTo<GetDerResponse>()
            };
        }

        public CreateOrUpdateResponse CreateOrUpdate(CreateOrUpdateDerRequest request)
        {
            var response = new CreateOrUpdateResponse();
            try
            {
                if (request.Id > 0)
                {
                    var der = DataContext.Ders.Single(x => x.Id == request.Id);
                    der.IsActive = request.IsActive;
                    der.Title = request.Title;
                    DataContext.Entry(der).State = EntityState.Modified;
                }
                else
                {
                    var der = new Der();
                    der.IsActive = request.IsActive;
                    der.Title = request.Title;
                    DataContext.Ders.Add(der);
                }

                DataContext.SaveChanges();
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
                var der = DataContext.Ders
                    .Include(x => x.Items)
                    .First(x => x.IsActive);

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
                if (request.Id > 0)
                {
                    var derLayout = DataContext.DerLayouts.Single(x => x.Id == request.Id);
                    derLayout.IsActive = request.IsActive;
                    derLayout.Title = request.Title;
                    DataContext.Entry(derLayout).State = EntityState.Modified;

                }
                else
                {
                    DataContext.DerLayouts.Add(new DerLayout() { IsActive = request.IsActive, Title = request.Title });
                }

                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "DER Layout has been added successfully";
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }

            return response;
        }

        public GetDerLayoutsResponse GetDerLayouts()
        {
            var derLayouts = DataContext.DerLayouts.ToList();
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
            //rowAndColumns.Add(new RowAndColumns { Row = 0, Column = 2 });
            //rowAndColumns.Add(new RowAndColumns { Row = 0, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 1, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 1, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 1, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 1, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 2, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 2, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 2, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 3 });
            rowAndColumns.Add(new RowAndColumns { Row = 3, Column = 4 });
            rowAndColumns.Add(new RowAndColumns { Row = 4, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 4, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 4, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 5, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 5, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 6, Column = 2 });
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
            rowAndColumns.Add(new RowAndColumns { Row = 11, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 12, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 13, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 13, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 13, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 14, Column = 0 });
            rowAndColumns.Add(new RowAndColumns { Row = 14, Column = 1 });
            rowAndColumns.Add(new RowAndColumns { Row = 14, Column = 2 });

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
                    /*.Include(x => x.Artifact.Charts)
                    .Include(x => x.Artifact.Charts.Select(y => y.Series))*/
                    .Include(x => x.Artifact.Charts.Select(y => y.Series.Select(z => z.Kpi)))
                    .Include(x => x.Artifact.Charts.Select(y => y.Measurement))
                    .Include(x => x.Artifact.Tank)
                    .Include(x => x.Artifact.Tank.VolumeInventory)
                    .Include(x => x.Artifact.Tank.DaysToTankTop)
                    .Include(x => x.Artifact.Tank.VolumeInventory.Measurement)
                    .Include(x => x.Artifact.Tank.DaysToTankTop.Measurement)
                    .Include(x => x.Highlight)
                    .Include(x => x.Highlight.SelectOption)
                    .Include(x => x.KpiInformations)
                    .Include(x => x.KpiInformations.Select(y => y.Kpi.Measurement))
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
                case "avg-ytd-key-statistic":
                    {
                        try
                        {
                            var derLayoutItem = DataContext.DerLayoutItems
                                .Include(x => x.KpiInformations)
                                .Include(x => x.DerLayout)
                                .Single(x => x.Id == id);
                            var kpiInformations = new DerKpiInformation();
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
            }

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
                case "highlight":
                    {
                        baseResponse = request.Id > 0 ? UpdateHighlight(request) : SaveHighlight(request);
                        break;
                    }
                case "weather":
                case "alert":
                case "wave":
                    {
                        baseResponse = SaveDynamicHighlight(request);
                        break;
                    }
                case "avg-ytd-key-statistic":
                case "safety":
                case "lng-and-cds":
                case "security":
                case "job-pmts":
                    {
                        baseResponse = request.Id > 0 ? UpdateKpiInformations(request) : SaveKpiInformations(request);
                        break;
                    }
                case "dafwc":
                    {
                        baseResponse = SaveDafwc(request);
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
                        case "job-pmts" :
                            {
                                for (int i = 0; i <=2; i++)
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

        //private baseresponse deletehighlight(savelayoutitemrequest request)
        //{
        //    var response = new getderlayoutresponse();
        //    try
        //    {
        //        var derlayoutitem = datacontext.derlayoutitems.include(x => x.highlight).include(x => x.highlight.selectoption).single(x => x.id == request.id);
        //        var selectoption = new selectoption { id = request.highlight.selectoptionid };
        //        datacontext.selectoptions.attach(selectoption);
        //        derlayoutitem.highlight.selectoption = selectoption;
        //        datacontext.derlayoutitems.remove(derlayoutitem);
        //        datacontext.savechanges();
        //        response.issuccess = true;
        //    }
        //    catch (exception exception)
        //    {
        //        response.message = exception.message;
        //    }

        //    return response;
        //}

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
                    var kpiInformation = DataContext.DerKpiInformations.Single(x => x.Id == item.Id);
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
                        kpiInformations.Add(new DerKpiInformation { Kpi = kpi, Position = item.Position, IsOriginalData = item.IsOriginalData });
                    }
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
                        kpiInformations.Add(new DerKpiInformation { Kpi = kpi, Position = item.Position, IsOriginalData = item.IsOriginalData });
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

        private BaseResponse SaveDafwc(SaveLayoutItemRequest request)
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
        }
    }
}
