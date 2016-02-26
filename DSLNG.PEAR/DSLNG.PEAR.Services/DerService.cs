using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Entities.Der;
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
            rowAndColumns.Add(new RowAndColumns { Row = 0, Column = 2 });
            rowAndColumns.Add(new RowAndColumns { Row = 0, Column = 3 });
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
                /*var derLayoutItem = DataContext.DerLayoutItems
                    .Include(x => x.DerLayout)
                    .Include(x => x.DerLayout.Items)
                    .Include(x => x.DerLayout.Items.Select(y => y.Artifact))
                    .Include(x => x.DerLayout.Items.Select(y => y.Artifact.Measurement))
                    .Include(x => x.DerLayout.Items.Select(y => y.Artifact.Series))
                    .Include(x => x.DerLayout.Items.Select(y => y.Artifact.Series.Select(z => z.Kpi)))
                    .Include(x => x.DerLayout.Items.Select(y => y.Artifact.Charts))
                    .Include(x => x.DerLayout.Items.Select(y => y.Artifact.Charts.Select(z => z.Series)))
                    .Include(x => x.DerLayout.Items.Select(y => y.Artifact.Charts.Select(z => z.Series.Select(a => a.Kpi))))
                    .Include(x => x.DerLayout.Items.Select(y => y.Artifact.Charts.Select(z => z.Measurement)))
                    .Include(x => x.Artifact.Tank)
                    .Include(x => x.Artifact.Tank.VolumeInventory)
                    .Include(x => x.Artifact.Tank.DaysToTankTop)
                    .Single(x => x.Id == id);*/

                var d = DataContext.DerLayoutItems
                    .Include(x => x.Artifact)
                    .Include(x => x.Artifact.Tank)
                    .Include(x => x.Artifact.Tank.VolumeInventory)
                    .Include(x => x.Artifact.Tank.DaysToTankTop)
                    .Single(x => x.Id == id);

                var derLayoutItem = DataContext
                    .DerLayoutItems
                    .Include(x => x.DerLayout)
                    .Include(x => x.Artifact)
                    .Include(x => x.Artifact.Series)
                    .Include(x => x.Artifact.Series.Select(y => y.Kpi))
                    .Include(x => x.Artifact.Charts)
                    .Include(x => x.Artifact.Charts.Select(y => y.Series))
                    .Include(x => x.Artifact.Charts.Select(y => y.Series.Select(z => z.Kpi)))
                    .Include(x => x.Artifact.Tank)
                    .Include(x => x.Artifact.Tank.VolumeInventory)
                    .Include(x => x.Artifact.Tank.DaysToTankTop)
                    .Include(x => x.Artifact.Measurement)
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

        public SaveLayoutItemResponse SaveLayoutItem(SaveLayoutItemRequest request)
        {
            var baseResponse = new BaseResponse();
            switch (request.Type.ToLowerInvariant())
            {
                case "line":
                    {
                        baseResponse = SaveLineChart(request);

                        //DataContext.DerLayouts
                        //SaveLine();
                        break;
                    }
                case "multiaxis":
                    {
                        baseResponse = SaveMultiAxis(request);
                        break;
                    }
                case "pie":
                    {
                        baseResponse = SavePie(request);
                        break;
                    }
                case "tank":
                    {
                        baseResponse = SaveTank(request);
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
                    .Include(x => x.Items.Select(y => y.DerLayout.Items))
                    .Include(x => x.Items.Select(y => y.Artifact))
                    .Include(x => x.Items.Select(y => y.Artifact.Measurement))
                    .Include(x => x.Items.Select(y => y.Artifact.Series))
                    .Include(x => x.Items.Select(y => y.Artifact.Series.Select(z => z.Kpi)))
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

        private BaseResponse SaveLineChart(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
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
                            Label = x.Label
                        }).ToList();

                    derArtifact.Series = series;
                    DataContext.DerArtifacts.Add(derArtifact);
                    derLayoutItem.Artifact = derArtifact;
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

        private BaseResponse SaveMultiAxis(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
            try
            {
                if (request.Id > 0)
                {

                }
                else
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
                        Kpi = DataContext.Kpis.FirstOrDefault(y => y.Id == x.KpiId)
                    }).ToList();

                    derArtifact.Series = series;
                    DataContext.DerArtifacts.Add(derArtifact);
                    derLayoutItem.Artifact = derArtifact;
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

        private BaseResponse SaveTank(SaveLayoutItemRequest request)
        {
            var response = new BaseResponse();
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
