

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Services.Responses.OutputConfig;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using DSLNG.PEAR.Data.Entities;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class OutputConfigService : BaseService,IOutputConfigService
    {

        public OutputConfigService(IDataContext dataContext) : base(dataContext) { }

        public GetKpisResponse GetKpis(GetKpisRequest request)
        {
            return new GetKpisResponse
            {
                KpiList = DataContext.Kpis.Where(x => x.Name.Contains(request.Term) && x.IsEconomic == true).Take(20).ToList().MapTo<GetKpisResponse.Kpi>()
            };
        }


        public GetKeyAssumptionsResponse GetKeyAssumptions(GetKeyAssumptionsRequest request)
        {
            return new GetKeyAssumptionsResponse
            {
                KeyAssumptions = DataContext.KeyAssumptionConfigs.Where(x => x.Name.Contains(request.Term)).Take(20).ToList().MapTo<GetKeyAssumptionsResponse.KeyAssumption>()
            };
        }


        public SaveOutputConfigResponse Save(SaveOutputConfigRequest request)
        {
            try
            {
                var outputConfig = request.MapTo<KeyOutputConfiguration>();
                if (request.Id != 0)
                {
                    outputConfig = DataContext.KeyOutputConfigs.Include(x => x.Measurement)
                        .Include(x => x.Category)
                       .Include(x => x.Kpis)
                       .Include(x => x.KeyAssumptions)
                       .First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<KeyOutputConfiguration>(outputConfig);
                    foreach (var kpi in outputConfig.Kpis.ToList())
                    {
                        outputConfig.Kpis.Remove(kpi);
                    }
                    foreach (var assumption in outputConfig.KeyAssumptions.ToList())
                    {
                        outputConfig.KeyAssumptions.Remove(assumption);
                    }
                    if (request.MeasurementId != outputConfig.Measurement.Id)
                    {
                        var measurement = new Measurement { Id = request.MeasurementId };
                        DataContext.Measurements.Attach(measurement);
                        outputConfig.Measurement = measurement;
                    }
                    if (request.CategoryId != outputConfig.Category.Id)
                    {
                        var category = new KeyOutputCategory { Id = request.CategoryId };
                        DataContext.KeyOutputCategories.Attach(category);
                        outputConfig.Category = category;
                    }
                   
                }
                else
                {
                    var measurement = new Measurement { Id = request.MeasurementId };
                    DataContext.Measurements.Attach(measurement);
                    outputConfig.Measurement = measurement;
                    var category = new KeyOutputCategory { Id = request.CategoryId };
                    DataContext.KeyOutputCategories.Attach(category);
                    outputConfig.Category = category;

                    DataContext.KeyOutputConfigs.Add(outputConfig);
                }
                foreach (var kpiId in request.KpiIds)
                {
                    var kpi = new Kpi { Id = kpiId };
                    if (DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiId) == null)
                    {
                        DataContext.Kpis.Attach(kpi);
                    }
                    else {
                        kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiId);
                    }
                    outputConfig.Kpis.Add(kpi);
                }
                foreach (var assumptionId in request.KeyAssumptionIds)
                {
                    var assumption = new KeyAssumptionConfig { Id = assumptionId };
                    if (DataContext.KeyAssumptionConfigs.Local.FirstOrDefault(x => x.Id == assumptionId) == null)
                    {
                        DataContext.KeyAssumptionConfigs.Attach(assumption);
                    }
                    else {
                        assumption = DataContext.KeyAssumptionConfigs.Local.FirstOrDefault(x => x.Id == assumptionId);
                    }
                    outputConfig.KeyAssumptions.Add(assumption);
                }
                DataContext.SaveChanges();
                return new SaveOutputConfigResponse
                {
                    IsSuccess = true,
                    Message = "The item has been saved successfully"
                };
            }
            catch {
                return new SaveOutputConfigResponse
                {
                    IsSuccess = false,
                    Message = "An Error Occured please contact the administrator for further information"
                };
            }
        }


        public GetOutputConfigResponse Get(GetOutputConfigRequest request)
        {
            return DataContext.KeyOutputConfigs.Include(x => x.Measurement)
                .Include(x => x.Category)
                .Include(x => x.Kpis)
                .Include(x => x.KeyAssumptions)
                .FirstOrDefault(x => x.Id == request.Id).MapTo<GetOutputConfigResponse>();
        }
    }
}
